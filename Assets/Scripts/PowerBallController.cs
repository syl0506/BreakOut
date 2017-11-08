using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBallController : MonoBehaviour {
	Vector3 ballDir = new Vector3( 0.0f, 0.0f, -1.0f);
	float speed = 0.5f;





	// Use this for initialization

	void Start () {
		
		this.gameObject.AddComponent<Rigidbody> ();
		this.gameObject.GetComponent<Rigidbody> ().useGravity = false;
		this.gameObject.GetComponent<Collider> ().isTrigger = true;

	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate (ballDir*speed);
		
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.tag == "bottomWall" || collider.gameObject.tag == "paddle") {
			Destroy (this.gameObject);
		} 

	}
}
