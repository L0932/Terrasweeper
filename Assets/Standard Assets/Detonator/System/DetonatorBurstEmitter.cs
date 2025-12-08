using UnityEngine;
using System.Collections;

/*
    DetonatorBurstEmitter modernized for Shuriken Particle System

    - Replaces Legacy ParticleEmitter / ParticleRenderer / ParticleAnimator
      with ParticleSystem + ParticleSystemRenderer.
    - Uses EmitParams to spawn bursts (one-shot style).
*/

public class DetonatorBurstEmitter : DetonatorComponent
{
    private ParticleSystem _particleSystem;
    private ParticleSystemRenderer _particleRenderer;

    private float _baseDamping = 0.1300004f;  // approximated via velocity limiting or left unused
    private float _baseSize = 1f;
    private Color _baseColor = Color.white;

    public float damping = 1f;                // not 1:1 with legacy, kept for compatibility
    public float startRadius = 1f;
    public float maxScreenSize = 2f;
    public bool explodeOnAwake = false;
    public bool oneShot = true;
    public float sizeVariation = 0f;
    public float particleSize = 1f;
    public float count = 1;
    public float sizeGrow = 20f;
    public bool exponentialGrowth = true;
    public float durationVariation = 0f;
    public bool useWorldSpace = true;
    public float upwardsBias = 0f;
    public float angularVelocity = 20f;
    public bool randomRotation = true;

    // NEW: use Shuriken render mode enum
    public ParticleSystemRenderMode renderMode = ParticleSystemRenderMode.Billboard;

    public bool useExplicitColorAnimation = false;
    public Color[] colorAnimation = new Color[5];

    private bool _delayedExplosionStarted = false;
    private float _explodeDelay;

    public Material material;

    // unused
    override public void Init()
    {
        Debug.Log("UNUSED");
    }

    public void Awake()
    {
        // Ensure there is a ParticleSystem
        _particleSystem = gameObject.GetComponent<ParticleSystem>();
        if (_particleSystem == null)
            _particleSystem = gameObject.AddComponent<ParticleSystem>();

        _particleRenderer = _particleSystem.GetComponent<ParticleSystemRenderer>();

        _particleSystem.hideFlags = HideFlags.HideAndDontSave;
        _particleRenderer.hideFlags = HideFlags.HideAndDontSave;

        // MAIN MODULE
        var main = _particleSystem.main;
        main.startLifetime = 1.0f;           // overridden per-particle on Emit
        main.startSize = particleSize;       // overridden per-particle on Emit
        main.startColor = Color.white;
        main.simulationSpace = useWorldSpace
            ? ParticleSystemSimulationSpace.World
            : ParticleSystemSimulationSpace.Local;

        // EMISSION: we only emit manually via Emit()
        var emission = _particleSystem.emission;
        emission.enabled = false;

        // SHAPE: off by default since we position particles manually
        var shape = _particleSystem.shape;
        shape.enabled = false;

        // COLOR OVER LIFETIME – we configure per explosion
        var col = _particleSystem.colorOverLifetime;
        col.enabled = false;

        // FORCE OVER LIFETIME – we’ll set this from 'force'
        var forceOverLifetime = _particleSystem.forceOverLifetime;
        forceOverLifetime.enabled = false;

        // RENDERER
        _particleRenderer.maxParticleSize = maxScreenSize;
        _particleRenderer.material = material;
        _particleRenderer.renderMode = renderMode;

        if (explodeOnAwake)
        {
            Explode();
        }
    }

    private float _emitTime;
    private float speed = 3.0f;
    private float initFraction = 0.1f;
    static float epsilon = 0.01f;

    void Update()
    {
        // The legacy system used ParticleAnimator.sizeGrow and updated it per frame
        // Here we *approximate* exponential growth by updating SizeOverLifetime.
        if (exponentialGrowth)
        {
            float elapsed = Time.time - _emitTime;
            if (elapsed > 0f)
            {
                // Build/update a curve that approximates SizeFunction over the particle lifetime.
                var main = _particleSystem.main;
                float life = Mathf.Max(main.startLifetime.constant, 0.01f);

                var sizeOverLifetime = _particleSystem.sizeOverLifetime;
                sizeOverLifetime.enabled = true;

                AnimationCurve curve = new AnimationCurve();
                // Sample the size function at a few points across the lifetime
                const int steps = 4;
                for (int i = 0; i <= steps; i++)
                {
                    float t = i / (float)steps;            // 0..1 normalized
                    float time = t * life;
                    float value = SizeFunction(time);      // 0..1-ish
                    curve.AddKey(t, value);
                }

                sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);
            }
        }
        else
        {
            // If not exponential, we can either turn SizeOverLifetime off or keep a simple linear growth
            var sizeOverLifetime = _particleSystem.sizeOverLifetime;
            sizeOverLifetime.enabled = false;
        }

        // delayed explosion timing
        if (_delayedExplosionStarted)
        {
            _explodeDelay -= Time.deltaTime;
            if (_explodeDelay <= 0f)
            {
                Explode();
            }
        }
    }

    private float SizeFunction(float elapsedTime)
    {
        float divided = 1 - (1 / (1 + elapsedTime * speed));
        return initFraction + (1 - initFraction) * divided;
    }

    public void Reset()
    {
        size = _baseSize;
        color = _baseColor;
        damping = _baseDamping;
    }

    // temp working variables
    private float _tmpParticleSize;
    private Vector3 _tmpPos;
    private Vector3 _tmpDir;
    private Vector3 _thisPos;
    private float _tmpDuration;
    private float _tmpCount;
    private float _scaledDuration;
    private float _scaledDurationVariation;
    private float _scaledStartRadius;
    private float _randomizedRotation;
    private float _tmpAngularVelocity;

    override public void Explode()
    {
        if (!on) return;

        // Ensure simulation space is correct at explosion time
        var main = _particleSystem.main;
        main.simulationSpace = useWorldSpace
            ? ParticleSystemSimulationSpace.World
            : ParticleSystemSimulationSpace.Local;

        _scaledDuration = timeScale * duration;
        _scaledDurationVariation = timeScale * durationVariation;
        _scaledStartRadius = size * startRadius;

        _particleRenderer.renderMode = renderMode;
        _particleRenderer.material = material;

        if (!_delayedExplosionStarted)
        {
            _explodeDelay = explodeDelayMin + (Random.value * (explodeDelayMax - explodeDelayMin));
        }

        if (_explodeDelay <= 0f)
        {
            // ----- COLOR OVER LIFETIME -----
            // Convert the "5 color slots" to a Gradient
            Color[] modifiedColors = new Color[5];

            if (useExplicitColorAnimation && colorAnimation != null && colorAnimation.Length >= 5)
            {
                modifiedColors[0] = colorAnimation[0];
                modifiedColors[1] = colorAnimation[1];
                modifiedColors[2] = colorAnimation[2];
                modifiedColors[3] = colorAnimation[3];
                modifiedColors[4] = colorAnimation[4];
            }
            else // auto fade
            {
                modifiedColors[0] = new Color(color.r, color.g, color.b, (color.a * 0.7f));
                modifiedColors[1] = new Color(color.r, color.g, color.b, (color.a * 1.0f));
                modifiedColors[2] = new Color(color.r, color.g, color.b, (color.a * 0.5f));
                modifiedColors[3] = new Color(color.r, color.g, color.b, (color.a * 0.3f));
                modifiedColors[4] = new Color(color.r, color.g, color.b, (color.a * 0.0f));
            }

            Gradient grad = new Gradient();
            grad.SetKeys(
                new GradientColorKey[]
                {
                    new GradientColorKey(modifiedColors[0], 0.0f),
                    new GradientColorKey(modifiedColors[1], 0.25f),
                    new GradientColorKey(modifiedColors[2], 0.5f),
                    new GradientColorKey(modifiedColors[3], 0.75f),
                    new GradientColorKey(modifiedColors[4], 1.0f)
                },
                new GradientAlphaKey[]
                {
                    new GradientAlphaKey(modifiedColors[0].a, 0.0f),
                    new GradientAlphaKey(modifiedColors[1].a, 0.25f),
                    new GradientAlphaKey(modifiedColors[2].a, 0.5f),
                    new GradientAlphaKey(modifiedColors[3].a, 0.75f),
                    new GradientAlphaKey(modifiedColors[4].a, 1.0f)
                }
            );

            var col = _particleSystem.colorOverLifetime;
            col.enabled = true;
            col.color = new ParticleSystem.MinMaxGradient(grad);

            // FORCE OVER LIFETIME (replaces ParticleAnimator.force)
            var forceOverLifetime = _particleSystem.forceOverLifetime;
            forceOverLifetime.enabled = true;
            forceOverLifetime.x = force.x;
            forceOverLifetime.y = force.y;
            forceOverLifetime.z = force.z;

            _tmpCount = count * detail;
            if (_tmpCount < 1) _tmpCount = 1;

            if (useWorldSpace)
            {
                _thisPos = transform.position;
            }
            else
            {
                _thisPos = Vector3.zero;
            }

            // Emit particles manually with EmitParams
            for (int i = 1; i <= _tmpCount; i++)
            {
                // position inside sphere
                _tmpPos = Vector3.Scale(
                    Random.insideUnitSphere,
                    new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius)
                );
                _tmpPos = _thisPos + _tmpPos;

                // velocity inside given range
                _tmpDir = Vector3.Scale(
                    Random.insideUnitSphere,
                    new Vector3(velocity.x, velocity.y, velocity.z)
                );
                _tmpDir.y = (_tmpDir.y + (2 * (Mathf.Abs(_tmpDir.y) * upwardsBias)));

                if (randomRotation)
                {
                    _randomizedRotation = Random.Range(-1f, 1f);
                    _tmpAngularVelocity = Random.Range(-1f, 1f) * angularVelocity;
                }
                else
                {
                    _randomizedRotation = 0f;
                    _tmpAngularVelocity = angularVelocity;
                }

                _tmpDir *= size;

                _tmpParticleSize = size * (particleSize + (Random.value * sizeVariation));
                _tmpDuration = _scaledDuration + (Random.value * _scaledDurationVariation);

                var emitParams = new ParticleSystem.EmitParams
                {
                    position = _tmpPos,
                    velocity = _tmpDir,
                    startSize = _tmpParticleSize,
                    startLifetime = _tmpDuration,
                    startColor = color,
                    rotation3D = new Vector3(0f, 0f, _randomizedRotation * Mathf.Deg2Rad),
                    angularVelocity3D = new Vector3(0f, 0f, _tmpAngularVelocity * Mathf.Deg2Rad)
                };

                _particleSystem.Emit(emitParams, 1);
            }

            _emitTime = Time.time;
            _delayedExplosionStarted = false;
            _explodeDelay = 0f;
        }
        else
        {
            // tell Update() to start reducing the delay and call Explode again when it's zero
            _delayedExplosionStarted = true;
        }
    }
}
