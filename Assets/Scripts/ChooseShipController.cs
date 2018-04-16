using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChooseShipController : MonoBehaviour {

	int currentSelection = 1;
	UIfader[] selectionHighlights = new UIfader[3];
	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;

	public GameObject ship1;
	public GameObject ship2;
	public GameObject ship3;

    public AudioClip ButtonSound;
    public AudioSource source;

	public float randRotX;
	public float randRotY;
	public float randRotZ;

    // Use this for initialization
    void Awake () {
		selectionHighlights [0] = selection1.GetComponent<UIfader>();
		selectionHighlights [1] = selection2.GetComponent<UIfader>();
		selectionHighlights [2] = selection3.GetComponent<UIfader>();

		selectionHighlights [0].setActive(false);
		selectionHighlights [1].setActive(true);
		selectionHighlights [2].setActive(false);

		randRotX = Random.Range (-200, 200);
		randRotY = Random.Range (-200, 200);
		randRotZ = Random.Range (-200, 200);

        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftArrow) && currentSelection != 0) {
			selectionHighlights[currentSelection].setActive(false);
			currentSelection--;
			selectionHighlights[currentSelection].setActive(true);
            source.PlayOneShot(ButtonSound, 0.5f);
        }
		if (Input.GetKeyDown (KeyCode.RightArrow) && currentSelection != 2) {
			selectionHighlights[currentSelection].setActive(false);
			currentSelection++;
			selectionHighlights[currentSelection].setActive(true);
            source.PlayOneShot(ButtonSound, 0.5f);
        }

		if (Input.GetKeyDown (KeyCode.Escape)) {
			SceneManager.LoadScene ("Menu");
		}

		if(Input.GetKeyDown(KeyCode.Return))
		{
			if (currentSelection == 0) {
				StaticData.startingShipHealth = 1200; //700;
				StaticData.startingShipDamage = 25;//50;
				StaticData.startingShipSpeed = 400;
				StaticData.startShipSpecial = 1;
			} else if (currentSelection == 1) {
				StaticData.startingShipHealth = 800;//400;
				StaticData.startingShipDamage = 50;//25;
				StaticData.startingShipSpeed = 600;
				StaticData.startShipSpecial = 2;
			} else if (currentSelection == 2) {
				StaticData.startingShipHealth = 1000;//550;
				StaticData.startingShipDamage = 75;//75;
				StaticData.startingShipSpeed = 300;
				StaticData.startShipSpecial = 0;
			}
			SceneManager.LoadScene ("Main Scene");
            source.PlayOneShot(ButtonSound, .1f);
        }

		if (currentSelection == 0) {
			ship1.transform.transform.Rotate(randRotX*Time.deltaTime,randRotY*Time.deltaTime, randRotZ*Time.deltaTime);
		}
		if (currentSelection == 1) {
			ship2.transform.transform.Rotate(randRotX*Time.deltaTime,randRotY*Time.deltaTime, randRotZ*Time.deltaTime);
		}
		if (currentSelection == 2) {
			ship3.transform.transform.Rotate(randRotX*Time.deltaTime,randRotY*Time.deltaTime, randRotZ*Time.deltaTime);
		}
	}
		
}
