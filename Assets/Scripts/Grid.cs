using UnityEngine;
using TerraPlay;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

	public int gridSizeX = 6;
	public int gridSizeY = 6;
	private int gridSize;

	public GameObject tile;
	public int mineNumber;

	private string state;
	private int tileDistance = 20;

	//private GameObject[] allTiles;

	[HideInInspector]
	public static List<Tile> allTiles;
	private static List<Tile> tilesMined;
	private static List<Tile> tilesUnmined;

	public static int tilesPerRow = 0;
	public static int tilesUncovered, minesRemaining, correctFlags, tileNumber;
	public static bool firstTileActivated = false;
	public static bool allMinesFlagged = false;

	public enum AdjacentMineCount {One = 1, Two, Three, Four, Five, Six, Seven, Eight}
	public Dictionary<AdjacentMineCount, Color> adjacentMineCountColors;
	//public Dictionary<AdjacentMineCount, byte[]> adjacentMineCountColors;
	private bool gameActive = true;

	public delegate void MineDataChange();
	public static event MineDataChange OnMineDataChanged;

	void Awake(){
		allTiles = new List<Tile>(gridSize);
		tilesMined = new List<Tile>();
		tilesUnmined = new List<Tile>(gridSize);
		SetMineCountColors();
	}

	// Use this for initialization
	void Start () {
		InitializeGrid();
		CreateTiles ();
	}
	
	private static Grid _instance;
	
	
	public static Grid Instance
	{
		get{
			
			if(_instance == null)
			{
				
				_instance = GameObject.FindObjectOfType<Grid>();
				DontDestroyOnLoad(_instance.gameObject);
			}
			
			return _instance;
		}
	}
		
	void InitializeGrid(){
		state = "Begin";//"inGame";
	
		tileNumber = gridSizeX * gridSizeY;
		minesRemaining = mineNumber;
		correctFlags = 0;
		tilesUncovered = 0;
		tilesPerRow = gridSizeX;
		allMinesFlagged = false;
	}

	void OnEnable(){
		GameManager.OnGameStateChanged += OnGameStateChanged;
	}
	
	void OnDisable(){
		GameManager.OnGameStateChanged -= OnGameStateChanged;
	}

	public void OnGameStateChanged(){
		gameActive = (GameManager.Instance.gameState == GameState.Active);
	}
	
	private void SetMineCountColors(){

		adjacentMineCountColors = new Dictionary<AdjacentMineCount, Color >();
		adjacentMineCountColors.Add (AdjacentMineCount.One, new Color(0f/255f, 0f/255f, 255f/255f)); // Blue
		adjacentMineCountColors.Add (AdjacentMineCount.Two, new Color(11f/255f, 77f/255f, 11f/255f)); // Dark Green
		adjacentMineCountColors.Add (AdjacentMineCount.Three, new Color(200f/255f, 0f/255f, 0f/255f)); // Light Dark Red
		adjacentMineCountColors.Add (AdjacentMineCount.Four, new Color(37f/255f, 37f/255f, 94f/255f)); // Light Dark Red
		adjacentMineCountColors.Add (AdjacentMineCount.Five, new Color(100f/255f, 0f/255f, 0f/255f)); // Dark Red
		adjacentMineCountColors.Add (AdjacentMineCount.Six, new Color(0f/255f, 255f/255f, 255f/255f)); // Cyan
		adjacentMineCountColors.Add (AdjacentMineCount.Seven, new Color(160f/255f, 0f, 255f/255f)); // Purple
		adjacentMineCountColors.Add (AdjacentMineCount.Eight, new Color(0f/255f, 0f/255f, 0f/255f)); // Black
	}

	public Color GetMineCountColor(int mineCount){
		AdjacentMineCount adjacentMineCount = (AdjacentMineCount)mineCount;
		return adjacentMineCountColors[adjacentMineCount];
	}

		
	public void CreateTiles(){
		gridSize = gridSizeX * gridSizeY;

		int xOffset = 0, zOffset = 0, tileCount = 1;
	
		for(int tilesCreated = 0; tilesCreated < gridSize; tilesCreated++){

			xOffset += tileDistance;

			if(tilesCreated % tilesPerRow == 0){
				zOffset += tileDistance;
				xOffset = 0;
			}

			GameObject tileGO = ObjectPool.instance.GetObjectForType("Tile" ,false);

			tileGO.transform.position = new Vector3(transform.position.x + xOffset, 11, transform.position.z + zOffset);
			tileGO.transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
			//Instantiate (tile, new Vector3(transform.position.x + xOffset, 11, transform.position.z + zOffset), Quaternion.Euler(new Vector3(90,0,0))) as GameObject;
			tileGO.name = "Tile";// + tilesCreated;
			tileGO.transform.SetParent(transform);
			Tile tileObject = tileGO.GetComponent<Tile>();
			tileObject.ID = tilesCreated; //Start ID at one
			tileObject.TilesPerRow = tilesPerRow;
			tileObject.DeformTerrain("add", 2);

			allTiles.Add(tileObject);
		}

		SetNeighbors();
	}

	void SetNeighbors(){

		foreach(Tile tileScript in allTiles){
			tileScript.assignNeighbors();
		}
	}

	public void SetMines(Tile firstTile){

		System.Random rand = new System.Random();
		tilesUnmined = allTiles.Where(x => x.unMinable != true).ToList();
		Tile mine = null;

		for (int i = 0; i < mineNumber; i++){
			mine = tilesUnmined[rand.Next(tilesUnmined.Count)];
			mine.MakeMine();

			tilesMined.Add(mine);
			tilesUnmined.Remove(mine);
		}


		tilesUnmined = allTiles.Where(x => x.isMine != true).ToList();
		tilesUnmined.ForEach(x => x.CountMines());
	
		state = "InGame";
		GameManager.Instance.gameState = GameState.Active;
		GameManager.Instance.ActivateGameStateChange();
	}

	// Update is called once per frame
	void Update () {
		if(gameActive){
			if((minesRemaining == 0 && correctFlags == mineNumber) || (tilesUncovered == tileNumber - mineNumber)){
				allMinesFlagged = correctFlags == mineNumber;
				EndGameIn(GameState.Victory);
			}
		}
	}

	private void EndGameIn(GameState state){
		GameManager.SetEndGameState(state);
	}

	public void FinalizeGame(){

		foreach(Tile currTile in tilesMined){
			if(currTile.State != "Flagged"){
				currTile.UncoverMine();
			}else{
				currTile.UncoverFlaggedMine();
			}
		}
	}

	public void EndSessionQuit(){
		ClearGrid();
		InitializeGrid();
	}

	public void ResetGrid(){
		InitializeGrid();
		ClearGrid();
		OnMineDataChanged();
		//CreateTiles ();
	}

	public void ClearGrid(){

		foreach(Tile tile in allTiles){
			tile.ResetTile();
			ObjectPool.instance.PoolObject(tile.gameObject);
		}
		
		allTiles.Clear ();
		tilesMined.Clear ();
		tilesUnmined.Clear ();
	}


	public string State{

			get{
				return state;
			}

			set{
				state = value;
			}
	}

	public int MinesRemaining{
		get{
			return minesRemaining;
		}

		set{
			minesRemaining = value;
			OnMineDataChanged();
		}
	}

	public int CorrectFlags{

		get{
			return correctFlags;
		}
		
		set{
			correctFlags = value;
		}
	}
	
	public int TilesUncovered{
		get{
			return tilesUncovered;
		}

		set{
			tilesUncovered = value;
		}
		
	}
}
