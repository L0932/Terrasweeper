using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {

	public List<GameObject> soundObjects;
	private Dictionary<string, GameObject> soundCollection;

	private static SoundManager _instance;
	
	public static SoundManager Instance{
		
		get{
			
			if(_instance==null){
				_instance = GameObject.FindObjectOfType<SoundManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	void Awake(){
		// First we check if there are any other instances conflicting
		if(Instance != null && Instance != this)
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		soundCollection = new Dictionary<string, GameObject>();

		if(soundObjects.Count > 0){
			soundCollection.Add ("WinSound", soundObjects[0]);
			soundCollection.Add ("LoseSound", soundObjects[1]);
			soundCollection.Add ("DigSound", soundObjects[2]);
			soundCollection.Add ("FlagIn", soundObjects[3]);
			soundCollection.Add ("FlagOut", soundObjects[4]);
		}else{
			Debug.LogError("Assign sound objects in the Sound Manager.");
		}
	}
	

	// Update is called once per frame
	void Update () {
	
	}

	public void Play(string trackName){
		if(soundCollection.ContainsKey(trackName))
			soundCollection[trackName].SetActive(true);
		else
			Debug.LogError ("TrackName " + trackName + ", does not exist. Check Sound Manager");
	}

	public void Play(string trackName, float pitch){
		if(soundCollection.ContainsKey(trackName)){
			soundCollection[trackName].GetComponent<AudioSource>().pitch = pitch;
			soundCollection[trackName].SetActive(true);
		}
		else
			Debug.LogError ("TrackName " + trackName + ", does not exist. Check Sound Manager");
	}

	public void Stop(string trackName){
		if(soundCollection.ContainsKey(trackName))
			soundCollection[trackName].SetActive(false);
		else
			Debug.LogError ("TrackName " + trackName + ", does not exist. Check Sound Manager");
	}
}
