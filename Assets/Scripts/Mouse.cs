using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour {

	public static Vector3 inputPos; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		#if UNITY_STANDALONE_WIN
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if(Physics.Raycast (ray, out hit, 1000))
		{
			inputPos = hit.point;

			if(Input.GetMouseButtonDown(0)){
				//TerraCrater.instance.DeformTerrain(hit.point);
			}
		}

		#endif

		#if UNITY_ANDROID

		/*
		foreach(Touch touch in Input.touches){
			if(touch.phase == TouchPhase.Began){
				Ray ray2 = Camera.main.ScreenPointToRay(touch.position);
				RaycastHit hit;

				if (Physics.Raycast (ray2, out hit, 1000)){
					inputPos = hit.point;

					TerraCrater.instance.DeformTerrain(hit.point);
				}
			}

		}*/
		#endif 

	}
}
