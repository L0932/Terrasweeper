using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using TerraPlay;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GoogPlay : MonoBehaviour {
	

	private static GoogPlay _instance;

	// list of achievements we know we have unlocked (to avoid making repeated calls to the API)
	private Dictionary<string, bool> mUnlockedAchievements = new Dictionary<string, bool>();

	// achievement increments we are accumulating locally, waiting to send to the games API
	private Dictionary<string,int> mPendingIncrements = new Dictionary<string, int>();

    private bool mAuthenticating = false;
	public List<GameObject> buttons;
	private bool mWaitingForAuth = false;
	private Text buttonText;

	public static GoogPlay Instance
	{
		get{
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<GoogPlay>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			return _instance;
		}
	}

	void Awake(){

		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			
			//enables saving game progress.
			//.EnableSavedGames()
			
			// registers a callback to handle game invitations received while the game is not running.
			//.WithInvitationDelegate(<callback method>)
			
			// registers a callback for turn based match notifications received while the
			// game is not running.
			//.WithMatchDelegate(<callback method>)
			
			.Build();
		
		PlayGamesPlatform.InitializeInstance(config);
		
		
		PlayGamesPlatform.DebugLogEnabled = true;
		
		PlayGamesPlatform.Activate();
	}

	// Use this for initialization
	void Start () {
		buttonText = buttons[0].GetComponentInChildren<Text>();
	}
	
	public bool Authenticating
	{
		get
		{
			return mAuthenticating;
		}
	}
	
	public bool Authenticated
	{
		get
		{
			return Social.Active.localUser.authenticated;
		}
	}

	public void GPlaySignIn(Action<bool> action){

		if (Authenticated || mAuthenticating)
		{
			Debug.LogWarning("Ignoring repeated call to Authenticate().");
			//action(false);
			return;
		}

		mAuthenticating = true;

		Social.localUser.Authenticate((bool success) => {
			mAuthenticating = false;

			/*
			if(success){
				//buttonText.text = "Welcome " + Social.localUser.userName;

				//string token = GooglePlayGames.PlayGamesPlatform.Instance.GetToken();
				//Debug.Log(token);
				//buttonText.text = "Signed In";
				//buttons[1].SetActive(true); // Show Achievements
			}
			else{
				//buttonText.text = "Sign In Failed";
				Debug.Log ("Authentication Failed");
			}*/

			action(success);
		});

		return;
	}

	public bool GPlayShowAchievements(){
		if(Authenticated)
			Social.ShowAchievementsUI();
		return Authenticated;
	}
			
	public void SignOut()
	{
		((PlayGamesPlatform)Social.Active).SignOut();
	}

	void ReportAllProgress(){
		FlushAchievements();
	}

	public void RestartGame(){
		ReportAllProgress();
	}

	/*
	public void GPlayUnlockAchievement(){
		Social.ReportProgress(GPGSIds.achievement_challenge_complete, 100.0f, (bool success) => {
			// handle success or failure
		});
	}*/

	public void GPlayUnlockAchievement(string achID){
		if(Authenticated && !mUnlockedAchievements[achID])
		{
			Social.ReportProgress(achID, 100.0f, (bool success) => {
			});
			mUnlockedAchievements[achID] = true;
		}
	}

	public void GPlayIncrementAchievement(string achId, int steps)
	{
		if (mPendingIncrements.ContainsKey(achId))
		{
			steps += mPendingIncrements[achId];
		}
		mPendingIncrements[achId] = steps;
	}

	public void FlushAchievements()
	{
		if (Authenticated)
		{
			foreach (string ach in mPendingIncrements.Keys)
			{
				// incrementing achievements by a delta is a feature
				// that's specific to the Play Games API and not part of the
				// ISocialPlatform spec, so we have to break the abstraction and
				// use the PlayGamesPlatform rather than ISocialPlatform
				PlayGamesPlatform p = (PlayGamesPlatform)Social.Active;
				p.IncrementAchievement(ach, mPendingIncrements[ach], (bool success) =>
				                  {
				});
			}
			mPendingIncrements.Clear();
		}
	}
		
	/*
	public void GPlayIncrementAchievement(){
		PlayGamesPlatform.Instance.IncrementAchievement(
			GPGSIds.achievement_mining_level_expert, 5, (bool success) => {
			// handle success or failure
		});
	}*/

	public void GPlayPostScore(){
		Social.ReportScore(12, GPGSIds.leaderboard_terrasweeper_leaderboard, (bool success) => {
			// handle success or failure
		});
	}

	public void GPlayShowLeaderboardGeneric(){
		Social.ShowLeaderboardUI();
	}

	public void GPlayShowLeaderboardSpecfic(){
		PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_terrasweeper_leaderboard);
	}

	public void GPlaySignOut(){
		PlayGamesPlatform.Instance.SignOut();
	}
}
