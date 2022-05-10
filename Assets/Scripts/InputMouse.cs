using UnityEngine;
using System.Collections;

public class InputMouse : InputObject{

	public override bool InputPress(){
		return Input.GetMouseButtonDown(0);
	}

	public override bool InputHold(){
		return Input.GetMouseButtonDown (1);
	}
}
