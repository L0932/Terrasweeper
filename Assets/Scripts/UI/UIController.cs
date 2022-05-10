using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	public List<GameObject> outsideUI;
	// Use this for initialization

	void OnEnable(){
		foreach(GameObject uiObject in outsideUI){
			uiObject.SetActive(true);
		}
	}

	void OnDisable(){
		foreach(GameObject uiObject in outsideUI){
			uiObject.SetActive(false);
		}
	}
}
