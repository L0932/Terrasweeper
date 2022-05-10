using UnityEngine;
using System.Collections;

public class SoundObject : MonoBehaviour {

	AudioSource audio;

	void Awake(){
		audio = GetComponent<AudioSource>();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable(){
		StartCoroutine("deactivateSoon");
	}

	IEnumerator deactivateSoon(){
		yield return new WaitForSeconds(audio.clip.length);
		gameObject.SetActive(false);
	}
}
