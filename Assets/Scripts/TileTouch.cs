using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class TileTouch : MonoBehaviour {

	public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
	private bool recentGUITouch;
	private bool inputStatus = true;
	//public Text debugText;
	// Use this for initialization
	protected virtual void OnEnable()
	{
		// Hook into the OnFingerTap event
		Lean.LeanTouch.OnFingerDown += OnFingerDown;
		Lean.LeanTouch.OnFingerTap += OnFingerTap;
		Lean.LeanTouch.OnFingerHeldDown += OnFingerHeldDown;
	}

	protected virtual void OnDisable()
	{
		// Unhook into the OnFingerTap event
		Lean.LeanTouch.OnFingerDown -= OnFingerDown;
		Lean.LeanTouch.OnFingerTap -= OnFingerTap;
		Lean.LeanTouch.OnFingerHeldDown -= OnFingerHeldDown;
	}

	public void OnFingerDown(Lean.LeanFinger finger){
		recentGUITouch = finger.IsOverGui;
	}

	public void InputStatus(bool b){
		inputStatus = b;
	}

	public void OnFingerTap(Lean.LeanFinger finger){
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (!inputStatus) return;

		if (recentGUITouch) { recentGUITouch = false; return; }

		var ray = finger.GetRay();
		var hit = default(RaycastHit);

		// Was this finger pressed down on a collider?
		if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask))
		{
			// Was that collider this one?
			if (hit.collider.tag == "Tile")
			{
				hit.collider.gameObject.SendMessage("TileTapped");
			}
		}	
	}

	public void OnFingerHeldDown(Lean.LeanFinger finger){
		if (EventSystem.current.IsPointerOverGameObject()) return;
		if (!inputStatus) return;
		//debugText.text = "finger Held";

		Ray ray = finger.GetRay();
		var hit = default(RaycastHit);

		if (Physics.Raycast (ray, out hit, float.PositiveInfinity, LayerMask)){

			if (hit.collider.tag == "Tile")
			{
				hit.collider.gameObject.SendMessage("TileHeld");
			}
		}
	}
}
