using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour {
	public bool isPaused = false;
	public bool canPauseUnpause = true;
	public GameObject pausedPanel;

    // Use this for initialization
    void Start () {
		Cursor.lockState = CursorLockMode.Locked;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != CursorLockMode.Locked);
    }
	
	// Update is called once per frame
	void Update () {
		if (isPaused && Input.GetAxis ("Cancel") ==1) {
            SceneManager.LoadScene ("Menu");
		}

		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown("joystick button 9"))
        {
			isPaused = !isPaused;
			if (isPaused) Time.timeScale = 0;
			else Time.timeScale = 1;
			pausedPanel.SetActive (isPaused);

			if (isPaused)
				Cursor.lockState = CursorLockMode.None;
			else
				Cursor.lockState = CursorLockMode.Locked;
			// Hide cursor when locking
			Cursor.visible = (CursorLockMode.Locked != CursorLockMode.Locked);
		}
	}
}
