using UnityEngine;
using System.Collections;

public class TextureChanger : MonoBehaviour {

	private float[,,] saved;
	private TerrainData tData;
	public Texture2D craterTex;
	private Color[] craterData;
	private int xRes, yRes;
	private int layers;


	
	private static TextureChanger _instance;
	
	public static TextureChanger Instance
	{
		get{
			
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<TextureChanger>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
	
	// Use this for initialization
	void Start () {
		tData = Terrain.activeTerrain.terrainData;
		yRes = tData.alphamapHeight;
		xRes = tData.alphamapWidth;
		layers = tData.alphamapLayers;
		craterData = craterTex.GetPixels ();
		saved = tData.GetAlphamaps(0,0,xRes, yRes);
	}
	
	public void DeformTerrain(Vector3 pos){

		//craterData = textures[idx].GetPixels();
		
		int x = (int)Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, tData.size.x, pos.x));
		int z = (int)Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, tData.size.z, pos.z));
		
		x = Mathf.Clamp(x, craterTex.width/2, xRes-craterTex.width/2);
		z = Mathf.Clamp(z, craterTex.height/2, yRes-craterTex.height/2);
		
		
		//x = Mathf.Clamp(x, craterTex.width/2, xRes-craterTex.width/2);
		//z = Mathf.Clamp(z, craterTex.height/2, yRes-craterTex.height/2);
		
		float[,,] areaT = tData.GetAlphamaps(x-craterTex.width/2, z-craterTex.height/2, craterTex.width, craterTex.height);
		
		
		for (int i = 0; i < craterTex.height; i++) {
			for (int j = 0; j < craterTex.width; j++) {
				for(int q = 0; q < layers; q++){
					if(q == 1){
						areaT[i,j,q] += craterData[i * craterTex.width + j].a;
					}
					else{
						areaT[i,j,q] -= craterData[i * craterTex.width + j].a;
					}
				}

			}
		}
		tData.SetAlphamaps(x-craterTex.width/2, z-craterTex.height/2, areaT);	
	}

	public void ResetTerrain(){
		
		if(saved != null){
			tData.SetAlphamaps(0,0,saved);
		}
		else{
			Debug.Log ("saved is null at TextureChanger."); //Temp
		}
	}

	void OnApplicationQuit(){
		tData.SetAlphamaps(0, 0, saved);
	}


}
