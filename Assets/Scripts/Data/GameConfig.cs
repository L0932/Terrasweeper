using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {

	public Grid gameGrid;
	public Transform gameCamera, cameraController;

	public static bool ConfigurationChange;
	public static Difficulty difficulty;
	public enum Difficulty {Beginner, Intermediate, Advanced} 

	// Use this for initialization
	void Start () {
	
	}

	private static GameConfig _instance;
	
	public static GameConfig Instance{
		
		get{
			
			if(_instance==null){
				_instance = GameObject.FindObjectOfType<GameConfig>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	public void ChangeDifficulty(int i){
		Difficulty diffValue = (Difficulty)i;
		
		if(difficulty != diffValue){
			difficulty = diffValue;
			GameManager.ConfigurationChange = true;
			ConfigurationChange = true;
			//Debug.Log ("difficulty is: " + difficulty);
		}else{
			ConfigurationChange = false;
		}
	}
	
	public void ConfigurateGame(){

		switch(difficulty){
			case Difficulty.Beginner:
				SetGridParameters(6, 6, 7, 200f, 10f, 180f); // 6 x 6 with 7 mines at world pos x y z
				SetCameraParameters(250f, 76f, 159f, new Vector3(42f, 0f, 0f));
				SetControllerParameters(225f, 275f, 40f, 65f, 165f, 240f);
				SetPrefsKeyTo("BeginnerTime");
				break;
			case Difficulty.Intermediate:
				SetGridParameters(10, 10, 20, 140f, 10f, 145f); // 10 x 10 with 20 mines at world pos x y z
				SetCameraParameters(230f,140f,117f, new Vector3(50f, 0f,0f));
				SetControllerParameters(180f, 280f, 40f, 140f, 117f, 300f);
				SetPrefsKeyTo("IntermediateTime");
				break;
			case Difficulty.Advanced:
				SetGridParameters(12, 12, 40, 130.4f, 10f, 137f); // 12 x 12 with 40 mines at world pos x y z
				SetCameraParameters(240f, 155.9f, 100.8f, new Vector3(49.40f, 0f, 0f));
				SetControllerParameters(150f, 320f, 40f, 140f, 110f, 320f);
				SetPrefsKeyTo("AdvancedTime");
				break;
		}
	}

	void SetGridParameters(int sizeX, int sizeY, int mineCount, float posX, float posY, float posZ){
		gameGrid.gridSizeX = sizeX;
		gameGrid.gridSizeY = sizeY;
		gameGrid.mineNumber = mineCount;
		gameGrid.transform.position = new Vector3(posX, posY, posZ);
	}

	void SetCameraParameters(float posX, float posY, float posZ, Vector3 rotation){
		cameraController.position = new Vector3(posX, posY, posZ);
		gameCamera.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
	}

	void SetControllerParameters(float minX, float maxX, float minY, float maxY, float minZ, float maxZ){
		cameraController.GetComponent<CameraControl>().SetBoundaries(minX, maxX, minY, maxY, minZ, maxZ);
	}

	void SetPrefsKeyTo(string key){
		GameManager.Instance.PrefsKey = key;
		//Debug.Log ("Pref Key set to " + key);
	}
}
