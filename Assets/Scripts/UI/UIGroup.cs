using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Ad-hoc Class to deal with parenting issues with Unity's UI system. Objects not children of this object will be activated/deactivated
public class UIGroup : MonoBehaviour {

	public List<GameObject> UIChildren;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		ToggleActiveChildren(true);
	}

	void OnDisable(){
		ToggleActiveChildren(false);
	}

	public void ToggleActiveChildren(bool b){
		foreach(GameObject UIChild in UIChildren){
			UIChild.SetActive(b);
		}
	}
}
