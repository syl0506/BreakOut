using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ballController : MonoBehaviour {

	public Rigidbody rb;
	public float thrust;
	private bool ballStart = false;
	public GameObject paddle;

	public GameManager gameManager;
	private Vector3 ballPos;

	private bool comboActive;
	private int comboNum;

	public GameObject ball;



	void Start(){
		ballPos = ball.transform.position;
		rb.isKinematic = true;
		comboActive = false;
		comboNum = 1;

	}
	// Update is called once per frame
	void Update () {
		if (ballStart == false) {
			ball.transform.position = new Vector3 (paddle.transform.position.x, ballPos.y, ballPos.z);
		
			if (Input.GetKey (KeyCode.Space) && gameManager.gameState == GameState.inPlay) {
				StartBall ();
			}
		} 
	
	}

	void StartBall(){
		
		rb = GetComponent<Rigidbody> ();
		rb.isKinematic = false;
		rb.AddForce (new Vector3(1.0f, 0.0f, 1.0f) * thrust);
		ballStart = true;
		SoundManager.Instance.backgroundMusic.Play ();
	}

	void OnTriggerEnter(Collider collision){
		if (gameManager.brickType.Contains (collision.gameObject.tag)) {
			SoundManager.Instance.brickSound.Play ();
			gameManager.RegisterScore (comboActive, comboNum);
			comboActive = true;
			comboNum++;
			for (var i = 0; i < gameManager.brickList.Count; i++) {
				if (gameManager.brickList [i] == collision.gameObject) {
					if (collision.gameObject.tag == "brick") {
						Destroy (collision.gameObject);
						gameManager.brickList [i] = null;
					} else if (collision.gameObject.tag == "power") {
						var powerPos = collision.gameObject.transform.position;
						Destroy (collision.gameObject);
						gameManager.brickList [i] = null;
						gameManager.DropPowerBall (powerPos);

					} else if (collision.gameObject.tag == "multi") {
						collision.gameObject.tag = "brick";
					}
				}
			}
		}
	}

	void OnCollisionEnter(Collision collision){
		if (gameManager.brickType.Contains(collision.gameObject.tag)) {
			SoundManager.Instance.brickSound.Play ();
			gameManager.RegisterScore (comboActive, comboNum);
			comboActive = true;
			comboNum++;
			for (var i = 0; i < gameManager.brickList.Count; i++) {
				if (gameManager.brickList[i] == collision.gameObject) {
					if (collision.gameObject.tag == "brick"){
						Destroy (collision.gameObject);
						gameManager.brickList [i] = null;
					}
					else if (collision.gameObject.tag == "power"){
						var powerPos = collision.gameObject.transform.position;
						Destroy (collision.gameObject);
						gameManager.brickList [i] = null;
						gameManager.DropPowerBall (powerPos);
					
					}
					else if ( collision.gameObject.tag == "multi"){
						collision.gameObject.tag = "brick";
					}
				}
			}

		} else if (collision.gameObject.tag == "bottomWall") {
			SoundManager.Instance.deathSound.Play ();
			gameManager.ManageRounds ();
			comboActive = false;
			comboNum = 1;
			SoundManager.Instance.backgroundMusic.Stop ();

			
		} else if (collision.gameObject.tag == "otherWall") {
			SoundManager.Instance.ballSound.Play ();
		
			
		} else if (collision.gameObject.tag == "paddle") {
			SoundManager.Instance.paddleSound.Play ();
			comboNum = 1;
			comboActive = false;
		}
	}



	public void ResetBall(GameObject ball){
		
		rb.isKinematic = true;
		ballStart = false;
		comboActive = false;
		ball.transform.position = new Vector3 (paddle.transform.position.x, ballPos.y, ballPos.z);
	}



}
