using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOveHandler : MonoBehaviour {

    public AudioClip ButtonSound;
    public AudioSource source;
	public Text scoreText;

	// Use this for initialization
	void Start () {
        source = GetComponent<AudioSource>();
		scoreText.text = "Score: " + StaticData.score.ToString ("N0");
	}

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			StaticData.score = 0;
			SceneManager.LoadScene ("Main Scene");
            source.PlayOneShot(ButtonSound, 0.5f);
        }

		if (Input.GetKeyDown (KeyCode.Escape)) {
			StaticData.score = 0;
			SceneManager.LoadScene ("Menu");
            source.PlayOneShot(ButtonSound, 0.5f);
        }
		
	}
}
