using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour {
	public bool isPaused = false;
	public GameObject pausedPanel;
	public GameObject startWave;
	public GameObject newWave;

	bool startWaveWasOn = false;
	bool newWaveWasOn = false;

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
			if (isPaused) {
				Time.timeScale = 0;
				if (startWave.activeSelf) {
					startWave.SetActive (false);
					startWaveWasOn = true;
				}
				if (newWave.activeSelf) {
					newWave.SetActive (false);
					newWaveWasOn = true;
				}
			} else {
				Time.timeScale = 1;
				if (startWaveWasOn) {
					startWave.SetActive (true);
					startWaveWasOn = false;
				}
				if (newWaveWasOn) {
					newWave.SetActive (true);
					newWaveWasOn = false;
				}
			}
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
