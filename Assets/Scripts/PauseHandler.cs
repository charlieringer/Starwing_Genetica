using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseHandler : MonoBehaviour {
	public bool isPaused = false;
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

		if (Input.GetAxis ("Pause") ==1)
        {
			isPaused = !isPaused;
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
