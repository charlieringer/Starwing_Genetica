using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FrontEndMenuController : MonoBehaviour {

	int currentSelection = 0;
	UIfader[] selectionHighlights = new UIfader[3];
	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;

	public GameObject menu;
	public GameObject controls;
	public GameObject credits;


	public AudioClip ButtonSound;
	public AudioSource source;

	int mode = 0;

	// Use this for initialization
	void Awake () {
		selectionHighlights [0] = selection1.GetComponent<UIfader>();
		selectionHighlights [1] = selection2.GetComponent<UIfader>();
		selectionHighlights [2] = selection3.GetComponent<UIfader>();

		source = GetComponent<AudioSource>();
	}

	void Start () {
		selectionHighlights [0].setActive(true);
		selectionHighlights [1].setActive(false);
		selectionHighlights [2].setActive(false);

	}

	// Update is called once per frame
	void Update () {
		if (mode == 0) {
			if (Input.GetKeyDown (KeyCode.DownArrow) && currentSelection != 2) {
				selectionHighlights [currentSelection].setActive (false);
				currentSelection++;
				selectionHighlights [currentSelection].setActive (true);
				source.PlayOneShot (ButtonSound, 0.5f);
			}
			if (Input.GetKeyDown (KeyCode.UpArrow) && currentSelection != 0) {
				selectionHighlights [currentSelection].setActive (false);
				currentSelection--;
				selectionHighlights [currentSelection].setActive (true);
				source.PlayOneShot (ButtonSound, 0.5f);
			}

			if (Input.GetKeyDown (KeyCode.Return)) {
				if (currentSelection == 0) {
					SceneManager.LoadScene ("ChooseShip");
					source.PlayOneShot (ButtonSound, .1f);
				} else if (currentSelection == 1) {
					mode = 1;
					controls.SetActive (true);
					menu.SetActive (false);
					source.PlayOneShot (ButtonSound, .1f);
				} else if (currentSelection == 2) {
					mode = 2;
					credits.SetActive (true);
					menu.SetActive (false);
					source.PlayOneShot (ButtonSound, .1f);
				}
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit ();
			}
		} else {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				mode = 0;
				credits.SetActive (false);
				controls.SetActive (false);
				menu.SetActive (true);
			}
		}
	}

}
