using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputTouch : InputObject {

	public override bool InputPress(){
		List<Lean.LeanFinger> fingers = Lean.LeanTouch.Fingers;

		if(fingers.Count > 0)
			return fingers[0].Tap;

		Debug.Log ("Finger count is not greater than 0");
		return false;
	}

	public override bool InputHold(){
		List<Lean.LeanFinger> fingers = Lean.LeanTouch.Fingers;
	
		if(fingers.Count > 0)
			return fingers[0].HeldDown;

		return false;
	}
}
