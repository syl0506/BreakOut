using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance { get; private set;}

	public AudioSource ballSound;
	public AudioSource brickSound;
	public AudioSource paddleSound;
	public AudioSource deathSound;
	public AudioSource backgroundMusic;

	// Use this for initialization
	void Start () {
		Instance = this;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
