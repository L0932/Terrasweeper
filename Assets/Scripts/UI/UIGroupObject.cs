using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIGroupObject : MonoBehaviour {

	public string name;
	public GameObject firstChild;
	public List<GameObject> UIChildren;
	public List<GameObject> UIObjectsToHide;
	private Button button;
	private bool active = false;

	void Awake(){
		button = gameObject.GetComponent<Button>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetInteractableTo(bool boolValue){
		button.interactable = boolValue;
	}

	public void ToggleInteractable(){
		button.interactable = button.IsInteractable() ? false : true;
	}

	public void SetChildrenTo(bool boolValue){
		foreach(GameObject UIChild in UIChildren){
			UIChild.SetActive(boolValue);
		}
		active = boolValue;
	}

	public void ToggleActiveChildren(){
		active = active ? false : true;
		if(active){
			ActivateFirstMenuItem();
			SetUIObjectsToHide(false);
		}
		else{
			DeactivateMenuItems();
			SetUIObjectsToHide(true);
		}
	}

	private void ActivateFirstMenuItem(){
		foreach(GameObject UIChild in UIChildren){
			if(UIChild == firstChild)
				UIChild.SetActive(true);
			else
				UIChild.SetActive(false);
		}
	}

	private void SetUIObjectsToHide(bool hideStatus){
		foreach(GameObject UIObject in UIObjectsToHide){
			UIObject.SetActive(hideStatus);
		}
	}

	private void DeactivateMenuItems(){
		foreach(GameObject UIChild in UIChildren){
			UIChild.SetActive(false);
		}
	}
	
	public bool Active{

		get{
			return active;
		}

		set{
			active = value;
		}
	}
}
