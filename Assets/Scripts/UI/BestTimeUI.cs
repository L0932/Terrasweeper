using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BestTimeUI : MonoBehaviour {

	public Text difficultyLevel, bestTime;

	void Start(){
		GameManager.OnHighScoreChanged += OnHighScoreChanged;
	}

	void OnEnable(){
		HandleText();
	}
	
	void HandleText(){

		float highScore = GameManager.Instance.HighScore;

		if(highScore == -1)
			bestTime.text = "N/A";
		else
			bestTime.text = string.Format ("{0:000}", highScore);

		difficultyLevel.text = GameConfig.difficulty.ToString();

		//Debug.Log("Handle Text has been called. Level is " + difficultyLevel.text);
	}

	void OnHighScoreChanged ()
	{
		HandleText();
	}
}
