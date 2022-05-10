using UnityEngine;
using UnityEngine.EventSystems;
//using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	public int id;
	
	public GameObject displayFlag;	
	public GameObject explosive;
	private GameObject mineObject;
	private Vector3 flagOriginalScale;
	//public GameObject digSound;

	public TextMesh displayText; 
	private SpriteRenderer spRenderer;
	private List<Tile> adjacentTiles;
	private InputObject inputObject;

	public  int adjacentMines = 0;
	public bool isMine = false;
	private bool unminable;
	private int tilesPerRow;
	private string state;

	public Tile tileUpper,tileLower, tileLeft, tileRight;
	public Tile tileUpperRight, tileUpperLeft, tileLowerRight, tileLowerLeft;

	private bool flagIsSet;
	private bool gameInitialized = true;
	// Use this for initialization
	void Awake () {
		adjacentTiles = new List<Tile>();
		adjacentTiles.Add (this);
		spRenderer = transform.GetComponent<SpriteRenderer>();
		inputObject = InputManager.Instance.GetInputObject();
	}

	void Start(){
		Initialize();
	}

	void OnEnable(){
		GameManager.OnGameStateChanged += OnGameStateChanged;
	}

	void OnDisable(){
		GameManager.OnGameStateChanged -= OnGameStateChanged;
	}

	void Initialize(){
		state = "Idle";
		displayFlag.SetActive(false);
		displayText.gameObject.SetActive(false);
		
		adjacentMines = 0;
		isMine = false;
		unminable = false;
	}

	public void OnGameStateChanged(){
		gameInitialized = (GameManager.Instance.gameState == GameState.Initialized);
	}

	public void TileTapped(){
		ActivateTile(true, false);
	}

	public void TileHeld(){
		ActivateTile(false, true);
	}

	public void ActivateTile(bool tileTapped, bool tileHeld){

		//if(!gameInitialized)
			//return;
		if(Grid.Instance.State == "InGame"){
			if(this.state == "Idle"){
				if (tileTapped){ //(inputObject.InputPress()){
					UncoverTile();
				}
				else if(tileHeld){ //(inputObject.InputHold()){
					SetFlag ();
				}
			}else if(this.state == "Flagged")
				if(tileHeld){ //(inputObject.InputHold())
					SetFlag ();
			}
		}
		else if (Grid.Instance.State == "Begin"){
			if (tileTapped){ //(inputObject.InputPress()){
				StartMiningGrid ();
			}else if (tileHeld){ //(inputObject.InputHold()){
				SetFlag ();
			}
		}
	}

	private void StartMiningGrid(){
	
		adjacentTiles = adjacentTiles.Select( x => { x.unMinable = true; return x; }).ToList();

		Grid.Instance.SetMines(this);
		UncoverTile();
	}


	private bool InBounds(int count, int targetID){

		if(targetID < 0 || targetID >= count)
			return false;
		else
			return true;
	}

	public void assignNeighbors(){

		if(adjacentTiles.Count < 1)
			return;

		int tileCount = Grid.allTiles.Count;

		if(InBounds (tileCount, this.ID + tilesPerRow)) tileUpper = Grid.allTiles[this.ID + tilesPerRow];
		if(InBounds (tileCount, this.ID - tilesPerRow)) tileLower = Grid.allTiles[this.ID - tilesPerRow];
		if(InBounds (tileCount, this.ID - 1) && (this.ID % tilesPerRow != 0)) tileLeft = Grid.allTiles[this.ID - 1];
		if(InBounds (tileCount, this.ID + 1) && (this.ID + 1) % tilesPerRow != 0) tileRight = Grid.allTiles[this.ID + 1];


		if(InBounds (tileCount, this.ID + tilesPerRow + 1) && (this.ID + 1) % tilesPerRow != 0) tileUpperRight = Grid.allTiles[this.ID + tilesPerRow + 1];
		if(InBounds (tileCount, this.ID + tilesPerRow - 1) && (this.ID % tilesPerRow != 0)) tileUpperLeft = Grid.allTiles[this.ID + tilesPerRow - 1];
		if(InBounds (tileCount, this.ID - tilesPerRow + 1) && (this.ID + 1) % tilesPerRow != 0) tileLowerRight = Grid.allTiles[this.ID - tilesPerRow + 1];
		if(InBounds (tileCount, this.ID - tilesPerRow - 1) && (this.ID % tilesPerRow != 0)) tileLowerLeft = Grid.allTiles[this.ID - tilesPerRow - 1];

		if(tileUpper) adjacentTiles.Add(tileUpper);
		if(tileLower) adjacentTiles.Add(tileLower);
		if(tileLeft) adjacentTiles.Add(tileLeft);
		if(tileRight) adjacentTiles.Add(tileRight);
		if(tileUpperRight) adjacentTiles.Add(tileUpperRight);
		if(tileUpperLeft)  adjacentTiles.Add(tileUpperLeft);
		if(tileLowerRight) adjacentTiles.Add(tileLowerRight);
		if(tileLowerLeft)  adjacentTiles.Add(tileLowerLeft);
	}

	public void MakeMine(){
		isMine = true;
		//spRenderer.color = new Color(.5f,0f,0f,.5f);
	}
	public void ResetTile(){
		
		if(isMine){
			if(state == "Flagged"){
				StopCoroutine("ScaleFlag");
				ScaleFlagBack();
			}
			
			if(mineObject != null){
				ObjectPool.instance.PoolObject(mineObject);
				mineObject = null;
			}
			//ObjectPool.instance.PoolObject(digSound);
			//spRenderer.color = new Color(0f,0f,0f,0f);
		}
		Initialize();
		ClearAdjacentTiles();
	}
	
	private void ClearAdjacentTiles(){
		if(adjacentTiles.Count < 1)
			return;

		if(tileUpper) {adjacentTiles.Remove(tileUpper); tileUpper = null;}
		if(tileLower) {adjacentTiles.Remove(tileLower); tileLower = null;}
		if(tileLeft) {adjacentTiles.Remove(tileLeft); tileLeft = null;}
		if(tileRight) {adjacentTiles.Remove(tileRight); tileRight = null;}

		if(tileUpperRight) {adjacentTiles.Remove(tileUpperRight); tileUpperRight = null;}
		if(tileUpperLeft)  {adjacentTiles.Remove(tileUpperLeft); tileUpperLeft = null;}
		if(tileLowerRight) {adjacentTiles.Remove(tileLowerRight); tileLowerRight = null;}
		if(tileLowerLeft)  {adjacentTiles.Remove(tileLowerLeft); tileLowerLeft = null;}
	}
	
	public void SetFlag(){

		if(state == "Idle"){
			state = "Flagged";

			displayFlag.SetActive (true);
			Grid.Instance.MinesRemaining -= 1;

			if(isMine)
				Grid.Instance.CorrectFlags += 1;
			SoundManager.Instance.Play ("FlagIn");
		}
		else if (state == "Flagged"){
			state = "Idle";

			displayFlag.SetActive (false);
			Grid.Instance.MinesRemaining += 1;

			if(isMine)
				Grid.Instance.CorrectFlags -= 1;
			SoundManager.Instance.Play ("FlagOut");
			
		}
	}

	public void CountMines(){
		adjacentMines = 0;

		foreach(Tile currTile in adjacentTiles){
			if(currTile.isMine)
				adjacentMines += 1;
		}

		if(adjacentMines > 0){
			displayText.text = adjacentMines.ToString ();
			displayText.color = Grid.Instance.GetMineCountColor(adjacentMines);
		}
		else{
			displayText.text = "";
		}
	}

	public void UncoverTile(){

		if((!isMine)) {
			state = "Uncovered";
			displayText.gameObject.SetActive(true);
			SoundManager.Instance.Play("DigSound", Random.Range(1f,3f));

			Grid.Instance.TilesUncovered += 1;

			if(adjacentMines == 0)
				UncoverAdjacentTiles();

			DeformTerrain("sub", 0);
		}else{
			Explode();//StartCoroutine("Explode");
		}
	}

	public void UncoverMine(){

		//Debug.Log ("UncoverMine called");

		if(isMine){
			//Debug.Log ("IsMine true at UncoverMine");

			mineObject = ObjectPool.instance.GetObjectForType("Mine" ,false);
			mineObject.name = "Mine";
			mineObject.transform.position = transform.position;

			if(mineObject == null)
				Debug.Log ("NULL AT TILE " + id); 

			DeformTerrain("sub", 0);		
		}
	}

	public void UncoverFlaggedMine(){
		if(isMine){
			//SetFlag();
			//spRenderer.color = new Color(.2f,0f,0f,.3f);
			StartCoroutine("ScaleFlag");
		}
	}

	private void UncoverAdjacentTiles(){
		foreach(Tile currTile in adjacentTiles){

			if(!currTile.isMine && currTile.state == "Idle" && currTile.adjacentMines >= 0){
				currTile.UncoverTile();
			}
		}
	}

	public void DeformTerrain(string str, int idx){

		Vector3 v = new Vector3(transform.position.x, 10, transform.position.z);
		TerraCrater.Instance.DeformTerrain(v, str, idx);
	}


	private IEnumerator ScaleFlag(){
		float duration = 1.5f;
		flagOriginalScale = displayFlag.transform.localScale;
		Vector3 targetScale = flagOriginalScale + new Vector3(.7f,.7f,.7f);

		float startTime = Time.time;
		while(Time.time - startTime < 25f){

			float amount = (Time.time - startTime)/duration;
			displayFlag.transform.localScale = new Vector3(Mathf.PingPong(Time.time, 1) + 1, Mathf.PingPong(Time.time, 1) + 1, Mathf.PingPong(Time.time, 1) + 1);//Vector3.Lerp(originalScale, targetScale, amount);//new Vector3(1,1,1);
			yield return null;
		}

		displayFlag.transform.localScale = flagOriginalScale;
	}

	private void ScaleFlagBack(){
		displayFlag.transform.localScale = flagOriginalScale;
	}

	private void Explode(){

		//ObjectPool.instance.PoolObject(mineObject);
		isMine = false;
		//Grid.Instance.State = "GameOver";
		EndGameIn(GameState.Defeat);

		//spRenderer.color = new Color(.5f,0f,0f,.5f);
		//Vector3 v = new Vector3(transform.position.x, 8, transform.position.z);

		GameObject explosion = ObjectPool.instance.GetObjectForType("Explosion_02_Large", false);//Instantiate (explosive, v, Quaternion.identity);
		explosion.transform.position = transform.position;

		DeformTerrain ("sub", 1);
		//yield return new WaitForSeconds(1f);
		//ObjectPool.instance.PoolObject(explosion);
	}

	private void EndGameIn(GameState state){
		GameManager.SetEndGameState(state);
	}

	public List<Tile> AdjacentTiles{

		get{
			return adjacentTiles;
		}
	}

	public bool unMinable{
		get{
			return unminable;
		}
		set{
			unminable = value;
		}
	}

	public int ID{
		
		get{
			return id;
		}
		
		set{
			id = value;
		}
	}
	
	public string State{
		
		get{
			return state;
		}
		
		set{
			state = value;
		}
		
	}

	public int TilesPerRow{get{return tilesPerRow;} set{tilesPerRow = value;}}
}
