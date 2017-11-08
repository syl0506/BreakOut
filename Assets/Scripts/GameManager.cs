using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GameState{
	none, 
	getReady,
	inPlay,
	gameOver
}

public enum PowerState{
	none,
	widerPaddle, 
	strongBall, 

}

public class GameManager : MonoBehaviour {


	public int Number;
	private int _number;
		
	public GameState gameState = GameState.none;
	public string[] levels;
	public int currentLevelIndex;

	public GameObject brick;
	public GameObject ball;
	public GameObject paddle;
	public GameObject floor;

	private int totalBricks;

	private GameObject brickClone;
	public List <string> brickType = new List <string>(){ "brick", "multi", "power" };

	public List <Material> brickTex;
	public List <Material> powerTex;

	private bool powerMode = false;

	public int brickRows;
	public int brickCols;
	private int brickNum;

	public List <GameObject> brickList;

	private float BRICK_X_OFFSET = 11.0f;
	private float BRICK_Z_OFFSET = 8.0f;

	public int lifeNum = 3;
	public int score = 0;

	public Text readyTxt;
	public Text scoreTxt;
	public Text lifeTxt;
	public Text gameOverTxt;
	public Text gameWinTxt;
	public Text comboTxt;
	public Text powerTxt;
	public Text lifeUpTxt;

	public List <Image> life;


	public ballController ballControl;
	public paddleController paddleControl;


	// Use this for initialization
	void Start () {
		currentLevelIndex = 0;
		ResetGame ();
		powerTxt.GetComponent<Text> ().enabled = false;

	}

	void SetStage(int levelIndex){
		var multiBrickNum = 0;
		brickList = new List <GameObject> ();
		brickNum = 0;
		totalBricks = 0;

		brick.SetActive (true);
		ParseText (levels [levelIndex]);

		foreach (GameObject brick in brickList){
			if (brick.tag == "multi"){
				multiBrickNum ++;
			}
		}
		totalBricks += multiBrickNum;
		Debug.Log (totalBricks);
		brick.SetActive (false);
	}
		
	// Update is called once per frame
	void Update () {
		UpdateTexture ();
		if (gameState == GameState.gameOver) {
			if (Input.GetKey (KeyCode.R)) {
				ResetGame ();
			}
		}
	}

	void UpdateTexture(){
		foreach (GameObject brick in brickList){
			if (brick != null) {
				if (brick.tag == "brick") {
					brick.GetComponent<Renderer> ().material = brickTex [0];
				} else if (brick.tag == "multi") {
					brick.GetComponent<Renderer> ().material = brickTex [1];
				} else if (brick.tag == "power") {
					brick.GetComponent<Renderer> ().material = brickTex [2];
				}
			}
		}
	}

	void ResetGame(){
		DestroyBricks ();
		SetStage (0);
		gameState = GameState.getReady;
		StartCoroutine (GetReady ());

		for (var i = 0; i < life.Count; i++) {
			life [i].GetComponent<Image> ().enabled = false;
		}
		gameOverTxt.GetComponent<Text>().enabled = false;
		gameWinTxt.GetComponent<Text> ().enabled = false;
		comboTxt.GetComponent<Text> ().enabled = false;
		score = 0;
		lifeNum = 3;
		powerTxt.GetComponent<Text> ().enabled = false;
		lifeUpTxt.GetComponent<Text> ().enabled = false;
		scoreTxt.text = "Score : " + score;
		lifeTxt.text = "Life : " + lifeNum;

	}

	void DestroyBricks(){
		for (var i = 0; i < brickList.Count; i++) {
			if ( brickList[i] != null){
				Destroy(brickList[i]);
			}
		}
	}



	void GenerateBrick(int rowIdx, int colIdx, string typeStr){

		string tagStr = "";

		if (typeStr == "B")
			tagStr = brickType [0];
		else if (typeStr == "M")
			tagStr = brickType [1];
		else if (typeStr == "P")
			tagStr = brickType [2];
		else
			return;

		totalBricks++;
		float offsetX = colIdx * BRICK_X_OFFSET;
		float offsetZ = rowIdx * BRICK_Z_OFFSET;
			
		var tempObject = Instantiate(brick, brick.transform.position + new Vector3(offsetX, 0, -offsetZ), Quaternion.Euler(new Vector3(0, 180, 0)));
		tempObject.tag = tagStr;
		brickList.Add (tempObject);
	}

	void ParseText(string path){
		TextAsset textFile = Resources.Load<TextAsset> (path);
		string parsedText = textFile.text;

		string[] rows = parsedText.Split ('\n');

		for (int r = 0; r < rows.Length; r++) 
		{
			string[] colums = rows [r].Split (' ');

			for (int c = 0; c < colums.Length; c++) {
				GenerateBrick (r, c, colums [c]);
			}
		} 
	}

	public void ManageRounds(){
		lifeNum--;
		lifeTxt.text = "Life : " + lifeNum;
		ballControl.ResetBall (ball);
		if (lifeNum <= 0) {
			currentLevelIndex = 0; 
			gameState = GameState.gameOver;
			gameOverTxt.GetComponent<Text> ().enabled = true;

		} else {
			gameState = GameState.getReady;
			StopCoroutine (PowerBlink("wide"));
			StopCoroutine (PowerBlink("strong"));
			StartCoroutine (GetReady ());
		}

	}

	public void RegisterScore(bool comboActive, int comboNum){
		
		if (comboActive== false) {
			score++;
		} else {
			score += comboNum;
			StartCoroutine (ComboBlink ());
		}

		brickNum++;
		scoreTxt.text = "Score : " + score;


		if (brickNum >= totalBricks) {
			
			if (currentLevelIndex++ != levels.Length -1 ) {

				DestroyBricks ();
				gameState = GameState.getReady;
				SetStage (currentLevelIndex);
				StartCoroutine (GetReady ());

			} else {
			
				gameState = GameState.gameOver;
				gameWinTxt.GetComponent<Text> ().enabled = true;
			}
			ballControl.ResetBall (ball);
			SoundManager.Instance.backgroundMusic.Stop ();
			
		}

	}

	public void DropPowerBall(Vector3 position){

		GameObject pBall = GameObject.CreatePrimitive(PrimitiveType.Sphere);

		string[] powerType = { "wide", "life", "strong" };
		int randomIndex = Random.Range (0, 3);
		pBall.gameObject.tag = powerType [randomIndex];
		pBall.gameObject.GetComponent<Renderer> ().material = powerTex [randomIndex];



		pBall.transform.position = position;
		pBall.transform.localScale = new Vector3 (4.0f, 4.0f, 4.0f);
		pBall.AddComponent<PowerBallController>();
	}

	public void EnterPowerMode(string mode){
		
		if (powerMode == false && gameState == GameState.inPlay) {
			
			if (mode == "wide") {
				powerMode = true;
				StartCoroutine(ActivateWideMode());

			} else if (mode == "life") {
				lifeNum++;
				lifeTxt.text = "Life : " + lifeNum;
				StartCoroutine (LifeBlink ());

	
			} else if (mode == "strong") {
				powerMode = true;
				StartCoroutine (ActivateStrongMode ());

			} 
		}
	}

	public IEnumerator ActivateStrongMode(){
		StartCoroutine (PowerBlink ("strong"));
		foreach(GameObject brick in brickList){
			if (brick != null){
				brick.gameObject.GetComponent<Collider>().isTrigger = true;
			}
		}
		yield return new WaitForSeconds(5.0f);

		foreach(GameObject brick in brickList){
			if (brick != null){
				brick.gameObject.GetComponent<Collider>().isTrigger = false;
			}
		}
		powerMode = false;
	}

	public IEnumerator ActivateWideMode(){
		
		StartCoroutine (PowerBlink ("wide"));
		paddle.transform.localScale = new Vector3 (30.0f, 4.0f, 2.0f);
		paddleControl.RIGHT_BOUND = 34.0f;
		paddleControl.LEFT_BOUND = -34.0f;
		yield return new WaitForSeconds(3.0f);

		paddle.transform.localScale = new Vector3 (15.0f, 4.0f, 2.0f);
		paddleControl.RIGHT_BOUND = 41.5f;
		paddleControl.LEFT_BOUND = -41.5f;
		powerMode = false;



	}

	public IEnumerator PowerBlink(string mode){
		
		if (mode == "strong") {
			powerTxt.text = "Strong Mode";
		} else if (mode == "wide") {
			powerTxt.text = "Wide Mode" ;
		}
		for (var i = 0; i < 15; i++) {
			powerTxt.GetComponent<Text> ().enabled = true;
			floor.GetComponent<Renderer> ().material.color = Color.blue;

			yield return new WaitForSeconds (0.1f);

			powerTxt.GetComponent<Text> ().enabled = false;
			floor.GetComponent<Renderer> ().material.color = Color.red;
			yield return new WaitForSeconds (0.1f);

			floor.GetComponent<Renderer> ().material.color = Color.black;
		}
	}

	public IEnumerator GetReady(){

		for (var i = 0; i < 3; i++) {
			readyTxt.GetComponent<Text> ().enabled = true;
			yield return new WaitForSeconds (0.5f);

			readyTxt.text = levels[currentLevelIndex];
			yield return new WaitForSeconds (0.5f);
			readyTxt.GetComponent<Text> ().enabled = false;
			readyTxt.text = "Press Space to Start Ball!";

		}

		gameState = GameState.inPlay;

	}

	public IEnumerator ComboBlink(){

		for (var i = 0; i < 3; i++) {
			comboTxt.GetComponent<Text> ().enabled = true;
			yield return new WaitForSeconds (0.1f);

			comboTxt.GetComponent<Text> ().enabled = false;
			yield return new WaitForSeconds (0.1f);
		}

	}
	public IEnumerator LifeBlink(){

		for (var i = 0; i < 3; i++) {
			lifeUpTxt.GetComponent<Text> ().enabled = true;
			yield return new WaitForSeconds (0.1f);

			lifeUpTxt.GetComponent<Text> ().enabled = false;
			yield return new WaitForSeconds (0.1f);
		}

	}
		
}

