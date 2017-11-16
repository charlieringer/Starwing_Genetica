﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseScriptController : MonoBehaviour {

	int currentSelection = 1;
	GameObject[] selectionHighlights = new GameObject[3];
	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;

    public AudioClip ButtonSound;
    public AudioSource source;

    // Use this for initialization
    void Awake () {
		selectionHighlights [0] = selection1;
		selectionHighlights [1] = selection2;
		selectionHighlights [2] = selection3;

        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftArrow) && currentSelection != 0) {
			selectionHighlights[currentSelection].SetActive(false);
			currentSelection--;
			selectionHighlights[currentSelection].SetActive(true);
            source.PlayOneShot(ButtonSound, 0.5f);
        }
		if (Input.GetKeyDown (KeyCode.RightArrow) && currentSelection != 2) {
			selectionHighlights[currentSelection].SetActive(false);
			currentSelection++;
			selectionHighlights[currentSelection].SetActive(true);
            source.PlayOneShot(ButtonSound, 0.5f);
        }

		if(Input.GetKeyDown(KeyCode.Return))
		{
			SceneManager.LoadScene ("Main Scene");
            source.PlayOneShot(ButtonSound, .1f);
        }
	}
		
}