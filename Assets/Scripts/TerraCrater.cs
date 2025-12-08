using UnityEngine;
using System.Collections;

public class TerraCrater : MonoBehaviour {

	private TerrainData terrDataInstance;
	//private TerrainData terrData;
	private float[,] saved;
	
	public float tileSize = 10.0f;
	
	public Texture2D craterTex;

	public Texture2D[] textures;

	private float[,] heightmap;
	private int xRes, yRes;
	private Color[] craterData;
	
	private static TerraCrater _instance;
	public static TerraCrater Instance
	{
		get{
			
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<TerraCrater>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}

	void Awake(){
	
		if(_instance == null){
			_instance = this;
			InitializeTerrain();
			DontDestroyOnLoad(this);
		}
		else{
			
			if(this != _instance)
				Destroy (this.gameObject);
		}
	}
	
	
	// Use this for initialization
	void Start () {

	}

	void InitializeTerrain(){
		terrDataInstance = Instantiate(Resources.Load("Terrain_01") as TerrainData); 
		GameObject terrain = Terrain.CreateTerrainGameObject(terrDataInstance);

		//terrData = Terrain.activeTerrain.terrainData;
		xRes = terrDataInstance.heightmapResolution;
		yRes = terrDataInstance.heightmapResolution;
		
		saved = terrDataInstance.GetHeights (0,0,xRes, yRes);
		//Terrain.activeTerrain.heightmapMaximumLOD = 0;
		Terrain.activeTerrain.heightmapPixelError = 1;
		//Terrain.activeTerrain.basemapDistance = 0;
	}

	void OnApplicationQuit(){
		ResetTerrain();
	}
	
	public void DeformTerrain(Vector3 pos, string str, int idx){

		craterData = textures[idx].GetPixels();

		int x = (int)Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, terrDataInstance.size.x, pos.x));
		int z = (int)Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, terrDataInstance.size.z, pos.z));
		
		x = Mathf.Clamp(x, textures[idx].width/2, xRes-textures[idx].width/2);
		z = Mathf.Clamp(z, textures[idx].height/2, yRes-textures[idx].height/2);

		
		//x = Mathf.Clamp(x, craterTex.width/2, xRes-craterTex.width/2);
		//z = Mathf.Clamp(z, craterTex.height/2, yRes-craterTex.height/2);

		float[,] terrHeights = terrDataInstance.GetHeights(x-textures[idx].width/2, z-textures[idx].height/2, textures[idx].width, textures[idx].height);
		
		
		for (int i = 0; i < textures[idx].height; i++) {
			for (int j = 0; j < textures[idx].width; j++) {
				if (str == "sub"){
					terrHeights [i,j] = (float)(terrHeights [i,j] - craterData[i*textures[idx].width+j].a*0.01);
				}
				else if(str == "add")
					terrHeights [i,j] = (float)(terrHeights [i,j] + craterData[i*textures[idx].width+j].a*0.01);
			}
		}		
		terrDataInstance.SetHeights(x-textures[idx].width/2, z-textures[idx].height/2, terrHeights);	

		if(str == "sub")
			TextureChanger.Instance.DeformTerrain(pos);
	}

	void RandomizeTerrain(){
		
		
		float[,] terrArea = terrDataInstance.GetHeights (0, 0, xRes, yRes);
		float r = Random.Range (-1000.0f, 1000.0f);
		
		for(int x = 0 ; x < xRes; x++){
			for(int y = 0; y < yRes; y++){
				terrArea[x,y] = Mathf.PerlinNoise((((float)x + r)/(float)xRes) * tileSize,(((float)y + r)/(float)yRes) * tileSize) / tileSize;//1;// (float)Random.Range(0.0f,2.0f) % 1;// ((float)y/1000f);
			}
			
			terrDataInstance.SetHeights(0, 0, terrArea);
		}
	}

	public void ResetTerrain(){
		if(saved != null){
			terrDataInstance.SetHeights(0,0,saved);
		}
		else{
			Destroy (terrDataInstance);
			terrDataInstance = Instantiate(Resources.Load("Terrain_01") as TerrainData); 
			saved = terrDataInstance.GetHeights (0,0,xRes, yRes);
		}
	}

	public TerrainData TerraDataInstance{
		get{
			return terrDataInstance;
		}
	}
}
