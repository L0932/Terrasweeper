using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	void Awake(){
		gameObject.name = "Explosion_02_Large";
	}

	// Use this for initialization
	void Start () {
	}

	void OnEnable(){
		StartCoroutine("Explode");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator Explode(){
		yield return new WaitForSeconds(2f);
		ObjectPool.instance.PoolObject(gameObject);
	}

}
