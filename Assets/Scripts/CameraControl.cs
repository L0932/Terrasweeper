using Lean;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

public class CameraControl : MonoBehaviour {

	public TileTouch tileTouchInput;

	public float ForceMultiplier = 0.01f;
	public float speed = 1.0f;
	public float smoothTime = 0.3F;

	private bool controlActive = false;


	private float MIN_X;
	private float MAX_X;

	private float MIN_Y;
	private float MAX_Y;

	private float MIN_Z;
	private float MAX_Z;

	void OnEnable(){
		LeanTouch.OnFingerDrag += OnFingerDrag;
		//Lean.LeanTouch.OnFingerUp += OnFingerUp;
	}

	void OnDisable(){
		LeanTouch.OnFingerDrag -= OnFingerDrag;
		//Lean.LeanTouch.OnFingerUp -= OnFingerUp;
	}

	public void ToggleControl(Toggle toggle){
		controlActive = toggle.isOn;
		tileTouchInput.InputStatus(!toggle.isOn);
	}

	void OnFingerDrag(LeanFinger finger){
		if (!controlActive) return;

		if(LeanTouch.Fingers.Count > 1){
			 
			LeanFinger fingerZero = LeanTouch.Fingers[0];
			LeanFinger fingerOne = LeanTouch.Fingers[1];

			float prevTouchDeltaMag = (fingerZero.LastScreenPosition - fingerOne.LastScreenPosition).magnitude;
			float touchDeltaMag = (fingerZero.ScreenPosition - fingerOne.ScreenPosition).magnitude;

			float deltaMagDiff = prevTouchDeltaMag - touchDeltaMag;
		
			transform.Translate(0, deltaMagDiff * speed,0);

			transform.position = new Vector3(transform.position.x, 
			                                 Mathf.Clamp(transform.position.y, MIN_Y, MAX_Y),
			                                 transform.position.z);
			/*
			transform.position = new Vector3(Mathf.Clamp (transform.position.x, MIN, MAX), 
			                                 Mathf.Clamp(transform.position.y, MIN, MAX),
			            					 Mathf.Clamp (transform.position.z, MIN, MAX));
			            					 */

		}
		else{
			//transform.Translate(-finger.DeltaScreenPosition.x * speed, 0, 0);
			transform.Translate(-finger.DeltaScreenPosition.x * speed, 0, -finger.DeltaScreenPosition.y * speed);

			transform.position = new Vector3(Mathf.Clamp (transform.position.x, MIN_X, MAX_X), 
			                                 transform.position.y,
			                                 Mathf.Clamp (transform.position.z, MIN_Z, MAX_Z));
		}
	}

	public void SetBoundaries(float minX, float maxX, float minY, float maxY, float minZ, float maxZ){
		MIN_X = minX;
	 	MAX_X = maxX;
		
		MIN_Y = minY;
		MAX_Y = maxY;
		
		MIN_Z = minZ;
		MAX_Z = maxZ;
	}


	/*
	void OnFingerUp(Lean.LeanFinger finger){
		if(finger.controlActive){
			controlActive = false;
			tileTouchInput.enabled = true;
		}
	}*/
}
