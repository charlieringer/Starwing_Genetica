using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour {
	public bool isPaused = false;
	public GameObject pausedPanel;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		if (isPaused && Input.GetKeyDown (KeyCode.Escape)) {
            SceneManager.LoadScene ("Menu");
		}

		if (Input.GetKeyDown (KeyCode.P))
        {
			isPaused = !isPaused;
			pausedPanel.SetActive (isPaused);
		}
	}
}
