using UnityEngine;
using System.Collections;

public class TestInput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Lean.LeanTouch.GuiInUse){
			Debug.Log ("GUI IS IN USE BRAH");
		}
	}
}
