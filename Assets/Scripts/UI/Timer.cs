using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

	private bool gameActive = false;
	private static Timer instance;
	private float seconds;

	public static Timer Instance {

		get{
			if (instance == null){
				instance = GameObject.FindObjectOfType<Timer>();
				DontDestroyOnLoad(instance.gameObject);
			}

			return instance;	
		}
	}
	
	public Text timerText;

	//private bool sessionEnded;
	private int minesLeft;
	public static float currentTime;


	// Use this for initialization
	void Start () {
		timerText = GetComponent<Text>();
		minesLeft = Grid.minesRemaining;

		GameManager.OnGameStateChanged += OnGameStateChanged;
		Grid.OnMineDataChanged += OnMineDataChanged;
		
		Display(0f);
	}

	/*
	void OnEnable(){
		GameManager.OnGameStateChanged += OnGameStateChanged;
		Grid.OnMineDataChanged += OnMineDataChanged;
	}*/

	/*
	void OnDisable(){

	}*/

	void OnApplicationQuit(){
		GameManager.OnGameStateChanged -= OnGameStateChanged;
		Grid.OnMineDataChanged -= OnMineDataChanged;
	}

	public void OnGameStateChanged(){
		gameActive = (GameManager.Instance.gameState == GameState.Active);
	}
	// Update is called once per frame
	void Update () {
	
		if(gameActive){
			currentTime += Time.deltaTime;

			seconds = currentTime % 999; //Use the euclidean division for the seconds.
		}

		Display (seconds);
	}

	private void Display(float seconds){
		timerText.text = string.Format ("{0:000}", minesLeft) + "          " + string.Format ("{0:000}", seconds);
	}

	public float GetCurrentTime(){
		return currentTime;
	}

	public void EndSessionQuit(){ //renaming soon.
		currentTime = 0f;
	}
	
	public void ResetTimerObject(){
		currentTime = 0f;
		seconds = 0f;
	}

	private void OnMineDataChanged(){
		minesLeft = Grid.minesRemaining;
	}
}
