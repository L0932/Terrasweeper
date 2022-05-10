using System;
using UnityEngine;
using System.Collections;
using GooglePlayGames;
using TerraPlay;

public class Achievement {

	private Func<bool> achievementCheck;
	public string id;
	private GooglePlayGames.BasicApi.Achievement achievement;


	public Achievement(Func<bool> check, string gpgsID){
		achievementCheck = check;
		id = gpgsID;
	}

	public void CompletionCheck(){

		if(achievement == null){
			PlayGamesPlatform p = (PlayGamesPlatform)Social.Active;
			achievement = p.GetAchievement(id);
		}

		if(achievement.IsUnlocked){ 
			return;
		}

		if (achievementCheck.Invoke()){
			AchievementManager.Instance.EnqueueAchievement(this); //All conditions to unlock achievement have been met, add to queue.
		}
	}

	public void UnlockAchievement(Action nextAchievement){
		Social.ReportProgress(achievement.Id, 100.0f, (bool success) => {
			nextAchievement();
			if(success)
				DebugMessenger.DebugMessage("success");
		});
	}
} 