using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugMessenger : MonoBehaviour {
	
	public Text debugText;
	private static DebugMessenger _instance;
	
	
	public static DebugMessenger Instance
	{
		get{
			
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<DebugMessenger>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	public static void DebugMessage(string str){
		DebugMessenger.Instance.debugText.text += str + "| ";
	}
}
