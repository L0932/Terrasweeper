using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using TerraPlay;
using GooglePlayGames;

public enum GameState {Initialized, Active, Paused, Victory, Defeat}

public class GameManager : MonoBehaviour {

	public GameObject grid;
	public GameObject startCamera;
	public GameObject gameCamera;
	public GameObject pauseCamera;
	public GameObject inputManager;

	public GameObject timerObject;
	public Button pauseUI;
	
	public GameObject[] mainMenuUI;
	public GameObject[] gameUI;

	private static bool configurationChange;
	private string prefsKey;
	private float highScore;
	private float currentScore;
	private bool paused = false;
	private int consecutiveWinCounter = 0;
	private int consecutiveLoseCounter = 0;
	private static bool resettingGame = false;


	//public GameObject[] gearOptions;
	private Difficulty difficulty;
	public enum Difficulty {Beginner, Intermediate, Advanced}

	public GameState gameState, savedState;
	
	public delegate void StateTransition();
	public static event StateTransition OnGameStateChanged;

	public delegate void HighScoreChange();
	public static event HighScoreChange OnHighScoreChanged;

	private List<Achievement> achievements = new List<Achievement>();
	
	void LoadAchievements(){
		//PlayGamesPlatform p = (PlayGamesPlatform)Social.Active;
		achievements.Add (new Achievement(()=> {return (Grid.correctFlags == 0) && (gameState == GameState.Victory);}, GPGSIds.achievement_working_memory));
		achievements.Add (new Achievement(()=> {return (Timer.currentTime <= 30f) && (Grid.allMinesFlagged) && (gameState == GameState.Victory);}, GPGSIds.achievement_just_in_time));
		achievements.Add (new Achievement(()=> {return (consecutiveWinCounter == 5) && (GameConfig.difficulty == GameConfig.Difficulty.Advanced) && (gameState == GameState.Victory);}, GPGSIds.achievement_challenge_seeker));
		achievements.Add (new Achievement(()=> {return (consecutiveLoseCounter == 3) && (gameState == GameState.Defeat);}, GPGSIds.achievement_bad_luck));
		achievements.Add (new Achievement(()=> {return (consecutiveWinCounter == 1) && (gameState == GameState.Victory);}, GPGSIds.achievement_welcome_to_terrasweeper));
	}

	public void StartGameState(){
		grid.SetActive(true);
		GameConfig.Instance.ConfigurateGame();

		GetLastHighScore(); 
			
		/*
		if(highScore == -1){//-1){
			bestTime.text = "N/A";
		}else{
			bestTime.text = string.Format ("{0:000}", highScore);
		}*/

		UIManager.Instance.SetUIGroupActivityTo("PreGameUI", false);
		UIManager.Instance.SetUIGroupActivityTo("InGameUI", true);
		UIManager.Instance.SetUIGroupActivityTo("Timer", true);
		
		SetGameCameraTo(true);
		SetStartCameraTo(false);
		SetPauseCameraTo(false);
		configurationChange = false;

		inputManager.SetActive(true);

		LoadAchievements();
	}
	
	private static GameManager _instance;
	
	public static GameManager Instance{
		
		get{
			
			if(_instance==null){
				_instance = GameObject.FindObjectOfType<GameManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	private void GetLatestScores(){
		GetLastHighScore();
		GetCurrentScore();
	}

	private void GetLastHighScore(){ //TEMP
		highScore = SecurePlayerPrefs.GetFloat(prefsKey, -1, "secPass");
		//Debug.Log ("best time is " + highScore + " for "+ prefsKey);
	}

	private void GetCurrentScore(){
		currentScore = Timer.Instance.GetCurrentTime();
	}

	public static void SetEndGameState(GameState endState){
		Grid.Instance.State = "GameOver";
		Instance.EndGameSession(endState);
	}
	
	public void EndGameSession(GameState endState){
		//if(Grid.Instance.State == "Gamewon" && Timer.Instance.CurrentScore < )
		gameState = endState;
		OnGameStateChanged();

		GetLatestScores();
		Grid.Instance.FinalizeGame();

		if(endState == GameState.Victory){
			SoundManager.Instance.Play("WinSound");

			consecutiveWinCounter += 1;
			consecutiveLoseCounter = 0;

			if (currentScore < highScore || highScore == -1){
				highScore = currentScore;
				//bestTime.text = string.Format ("{0:000}", highScore);

				// SECURE PLAYERPREFS
				SecurePlayerPrefs.SetFloat(prefsKey, highScore, "secPass"); 
				PlayerPrefs.Save();

				UIManager.Instance.SetUIGroupActivityTo("PostGameWinScore", true);

				if(OnHighScoreChanged != null)
					OnHighScoreChanged();

				//DEBUG PURPOSES
				float score = SecurePlayerPrefs.GetFloat(prefsKey, -1, "secPass"); //Old Key: "secHighScoreKey"
				Debug.Log ("new highScore is: " + score);
			}else
			{
				UIManager.Instance.SetUIGroupActivityTo("PostGameWin", true);
			}
		}
		else{
			UIManager.Instance.SetUIGroupActivityTo("PostGameLoss", true);
			consecutiveWinCounter = 0;
			consecutiveLoseCounter += 1;
			//SoundManager.Instance.Play("LoseSound");
		}

		if(GoogPlay.Instance.Authenticated)
			CheckAchievementsStatus();
	}

	void CheckAchievementsStatus(){

		foreach(Achievement achievement in achievements){
			achievement.CompletionCheck();
		}
		AchievementManager.Instance.HandleAchievementUnlocks();
	}

	public void ResumeGame(){

		if(gameState != GameState.Active){
			gameState = savedState;

			//OnGameStateChanged();
			SetGameCameraTo(true);
			SetPauseCameraTo(false);
			SetTimerTo (true);
		}
	}

	public void PauseGame(){
		if(gameState != GameState.Paused){
			savedState = gameState;
			gameState = GameState.Paused;

			//OnGameStateChanged();
			SetGameCameraTo(false);
			SetPauseCameraTo(true);
			SetTimerTo(false);
		}
	}

	public void ActivateGameStateChange(){
		OnGameStateChanged();
	}

	public void SetGameCameraTo(bool b){
		gameCamera.SetActive(b);
	}

	public void SetStartCameraTo(bool b){
		startCamera.SetActive (b);
	}

	public void SetPauseCameraTo(bool b){
		pauseCamera.SetActive (b);
	}

	public void SetTimerTo(bool b){
		timerObject.SetActive(b);
	}
	
	public void ResetHighScore(){
		highScore = -1;
		SecurePlayerPrefs.SetFloat(prefsKey, -1, "secPass");  
		PlayerPrefs.Save();

		Debug.Log ("High Score resetting to -1 for " + prefsKey);

		if(OnHighScoreChanged != null)
			OnHighScoreChanged();
	}

	public void QuitGameSession(){
		Application.Quit();
	}

	public void RestartGame(){
		GameConfig.Instance.ConfigurateGame();

		if(configurationChange){
			//GameConfig.Instance.ConfigurateGame();
			ResetAchievementProgress();
			configurationChange = false;
		}

		Grid.Instance.ResetGrid();

		TextureChanger.Instance.ResetTerrain();
		TerraCrater.Instance.ResetTerrain();

		if(!timerObject.activeSelf)
			timerObject.SetActive(true);

		Grid.Instance.CreateTiles();


		SetPauseCameraTo(false);
		SetGameCameraTo(true);

		Timer.Instance.ResetTimerObject();
		UIManager.Instance.ResetPostGameUI();

		gameState = GameState.Initialized;
		OnGameStateChanged();
	}

	public void ResetAchievementProgress(){
		consecutiveWinCounter = 0;
		consecutiveLoseCounter = 0;
	}
	
	public void ExitGameApplication(){
		Application.Quit();
	}

	public bool ResettingGame{
		get { 
			return resettingGame;
		}
	}

	public bool Paused{
		get{ 
			return paused;
		}
		set { paused = value;}
	}

	public string PrefsKey{
		get{ 
			return prefsKey;
		}
		set{ 
			prefsKey = value;
			GetLastHighScore();
		}
	}
	
	public float HighScore{
		get{
			return highScore;
		}
		set { highScore = value;}
	}
	
	public float CurrentScore{
		get{
			return currentScore;
		}
		set{
			currentScore = value;
		}
	}
	
	public void TogglePause() {
		paused = paused ? false : true;
	}

	public static bool ConfigurationChange { get {return configurationChange;} set{ configurationChange=value;}}
}
