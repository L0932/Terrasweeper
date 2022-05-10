using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchThresholdSlider : MonoBehaviour {

	public Lean.LeanTouch leanTouchScript;
	Slider slider; 

	void Awake(){
		slider = GetComponent<Slider>();
		slider.value = PlayerPrefs.GetFloat ("holdThreshold", 1f);
	}

	void OnEnable(){
		if(slider){
			slider.onValueChanged.AddListener(OnValueChanged);
			OnValueChanged(slider.value);
		}
	}

	void OnDisable(){
		if(slider){
			slider.onValueChanged.RemoveAllListeners();
		}
	}


	public void OnValueChanged(float value){
		if(slider){
			leanTouchScript.HeldThreshold = value;
			PlayerPrefs.SetFloat("holdThreshold", value);
			PlayerPrefs.Save();
		}
	}
}
