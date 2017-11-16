using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOveHandler : MonoBehaviour {

	public Text scoreText;
	// Use this for initialization
	void Start () {
		scoreText.text = "Score: " + StaticData.score;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			StaticData.score = 0;
			SceneManager.LoadScene ("Main Scene");
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			StaticData.score = 0;
			SceneManager.LoadScene ("Menu");
		}
		
	}
}
