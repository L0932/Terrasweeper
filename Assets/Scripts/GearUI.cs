using UnityEngine;
using System.Collections;

public class GearUI : MonoBehaviour {

	public GameObject[] gearOptions;

	private bool active = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetObjectsActiveness(){
		active = active ? false : true;
	
		foreach(GameObject element in gearOptions){
			element.SetActive(active);
		}
	}
}
