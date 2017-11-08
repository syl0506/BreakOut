using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class paddleController : MonoBehaviour {
	[SerializeField]
	private Camera mainCamera;


	public float RIGHT_BOUND = 41.5f;
	public float LEFT_BOUND = -41.5f;
	private Vector3 temp = new Vector3 ();

	public GameManager gameManager;



	// Update is called once per frame
	void Update () {
		MovePaddle ();
	}

	void MovePaddle(){
		//if (Input.GetKey (KeyCode.RightArrow) && this.transform.position.x < RIGHT_BOUND) {

		float initialZ = transform.position.z;
		temp = Input.mousePosition;
		temp.z = mainCamera.transform.position.y;
		temp = mainCamera.ScreenToWorldPoint (temp);
		temp.z = initialZ;
		temp.x = Mathf.Clamp (temp.x, LEFT_BOUND, RIGHT_BOUND);
		transform.position = temp;
	}

	void OnTriggerEnter(Collider collider){
		gameManager.EnterPowerMode (collider.gameObject.tag);
	}


}
