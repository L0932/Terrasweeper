using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class UIManager : MonoBehaviour {

	public Timer timerScript;
	public GameObject clickMask;
	public Toggle cameraControlToggle;
	public List<UIGroupObject> UIButtonGroupObjects; 

	public static bool activeUIGroup;
	
	private UIGroupObject chosenGroup = null;
	private readonly float delayTime = 0f;

	public GameObject[] UIGroups;
	private Dictionary<string, GameObject> UIDictionary;

	private string saveState;

	private static UIManager instance;
	public static UIManager Instance {
		
		get{
			if (instance == null){
				instance = GameObject.FindObjectOfType<UIManager>();
				DontDestroyOnLoad(instance.gameObject);
			}
			
			return instance;	
		}
	}

	// Use this for initialization
	void Start () {
		InitializeUIDictionary();
	}

	private void InitializeUIDictionary(){

		UIDictionary = new Dictionary<string, GameObject>();
		
		UIDictionary.Add("PreGameUI", UIGroups[0]);
		UIDictionary.Add("InGameUI", UIGroups[1]);
		UIDictionary.Add ("PostGameUI", UIGroups[2]);
		UIDictionary.Add ("PostGameWin", UIGroups[3]);
		UIDictionary.Add ("PostGameLoss", UIGroups[4]);
		UIDictionary.Add ("PostGameWinScore", UIGroups[5]);
	}

	public void ResetPostGameUI(){
		SetUIGroupActivityTo("PostGameWin", false);
		SetUIGroupActivityTo("PostGameLoss", false);
		ResetToggleHack();
	}

	public void ResetToggleHack(){
		cameraControlToggle.isOn = false;
	}
			            
	public void ToggleUIGroup(UIGroupObject uiGroupArg){
		foreach(UIGroupObject uiGroup in UIButtonGroupObjects){
			
			if(uiGroup == uiGroupArg){
				chosenGroup = uiGroup;
				chosenGroup.ToggleActiveChildren();
			}
			else {
				
				if(uiGroup.Active)
					uiGroup.SetChildrenTo(false);
			}
		}

		if(chosenGroup.Active){
			GameManager.Instance.PauseGame();
			SetUIGroupObjectActivityTo("PostGameUI", false);
		}
		else{
			GameManager.Instance.ResumeGame();
			SetUIGroupObjectActivityTo("PostGameUI", true);
		}
	}

	public void SetUIGroupActivityTo(string key, bool b){
		GameObject temp = null;
		
		if(UIDictionary.TryGetValue (key, out temp) && UIDictionary[key] != null){
			temp.SetActive(b);
			temp.SetChildrenTo(b);
		}
	}

	private void SetUIGroupObjectActivityTo(string key, bool b){
		GameObject temp = null;
	
		if(UIDictionary.TryGetValue (key, out temp) && UIDictionary[key] != null){
			temp.SetActive(b);
		}
	}
	
	public void ToggleActiveUIGroup(){
		if(chosenGroup != null && chosenGroup.Active)
			ToggleUIGroup(chosenGroup);
	}

	public void SetActiveUIGroupTo(bool boolValue){
		chosenGroup.SetChildrenTo(boolValue);
	}

	private void ToggleClickMask(){
		if(clickMask != null && chosenGroup != null)
			clickMask.SetActive(chosenGroup.Active);
	}
}
