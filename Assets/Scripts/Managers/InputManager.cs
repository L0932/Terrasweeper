using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputManager : MonoBehaviour {

	//public Text testText;

	private static InputManager _instance;
	InputObject inputObject;
	
	public static InputManager Instance{
		
		get{
			if(_instance==null){
				_instance = GameObject.FindObjectOfType<InputManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}	
			return _instance;
		}
	}

	void Awake(){

		if(Application.isMobilePlatform){
			//testText.text = "Mobile";
			inputObject = new InputTouch();
			//Debug.Log ("MOBILE");
		}
		else{// if(!Application.isConsolePlatform){
			//testText.text = "PC";
			//inputObject = new InputMouse();
			inputObject = new InputTouch();
		}
	}

	public InputObject GetInputObject(){
		return inputObject;
	}

	//public void SetDebugText(string str){
		//testText.text = str;
	//}
}
