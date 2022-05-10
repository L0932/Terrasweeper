using UnityEngine;
using System.Collections;

public class UIMask : MonoBehaviour {

	RectTransform maskRectTransform;
	

	void Awake(){
		maskRectTransform = GetComponent<RectTransform>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if(!RectTransformUtility.RectangleContainsScreenPoint(maskRectTransform, Input.mousePosition, Camera.main)){
			Debug.Log ("Touch outside");
		}
	}
}
