using UnityEngine;
using System.Collections;

public class CamRotate : MonoBehaviour {





	// Use this for initialization
	void Start () {

		//int num = (int)Category.Ones;
		//Debug.Log ("ENUM VAL: " + num);



	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(new Vector3(0,1,0), 10.0f * Time.deltaTime);
	}




}
