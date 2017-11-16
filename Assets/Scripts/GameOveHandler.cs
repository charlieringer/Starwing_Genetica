using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOveHandler : MonoBehaviour {

    public AudioClip ButtonSound;
    public AudioSource source;

    // Use this for initialization
    void Start () {
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return)) {
			SceneManager.LoadScene ("Main Scene");
            source.PlayOneShot(ButtonSound, 0.5f);
        }

		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Menu");
            source.PlayOneShot(ButtonSound, 0.5f);
        }
		
	}
}
