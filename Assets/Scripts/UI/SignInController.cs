using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SignInController : MonoBehaviour {
	public GameObject signInScreen;
	public GameObject prevScreen;
	public GameObject viewAchievementsButton, signInButton;
	public Text signInStatus;

	//TEMP IMPLEMENTATIONS
	void Start(){
		if(GoogPlay.Instance.Authenticated)
			signInStatus.text = "Signed In";
	}

	public void GPlaySignIn(){
		signInStatus.text = "Signing In..";
		GoogPlay.Instance.GPlaySignIn(SignInStatus);
	}
	
	public void GPlayShowAchievements(){

		if(!GoogPlay.Instance.GPlayShowAchievements()){ //Shows Achievements if true.
			//Handle case where user is not signed in.

			prevScreen.SetActive(false);
			signInScreen.SetActive(true);
			viewAchievementsButton.SetActive(false);

			signInStatus.text = "You must sign in to view Achievements.";		
		}
	}

	private void SignInStatus(bool status){
		if(status){
			signInStatus.text = "Signed In.";
			if(viewAchievementsButton) viewAchievementsButton.SetActive(true);
		}else{
			signInStatus.text = "Failed to Sign in";
		}
	}
}
