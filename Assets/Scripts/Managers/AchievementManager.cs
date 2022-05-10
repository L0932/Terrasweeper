using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AchievementManager : MonoBehaviour {

	private static AchievementManager _instance;
	//private List<string> achievementIds = new List<string>();
	//private List<Achievement> achievements;

	private Queue<Achievement> achievementQueue = new Queue<Achievement>();

	public static AchievementManager Instance
	{
		get{
			
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<AchievementManager>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	public void EnqueueAchievement(Achievement achievement){
		achievementQueue.Enqueue(achievement);
	}

	public void HandleAchievementUnlocks(){
		if(achievementQueue.Count > 0)
			UnlockNextAchievement();
	}

	private void UnlockNextAchievement(){
		if(achievementQueue.Count > 0){
			Achievement achievement = achievementQueue.Dequeue();
			achievement.UnlockAchievement(UnlockNextAchievement);
		}
	}
}
