using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FrontEndMenuController : MonoBehaviour {

	int currentSelection = 0;
	int currentSettingSelection = 0;
	UIfader[] selectionHighlights = new UIfader[4];
	UIfader[] settingsHighlights = new UIfader[5];
	public GameObject selection1;
	public GameObject selection2;
	public GameObject selection3;
	public GameObject selection4;

	public GameObject settingSelection1;
	public GameObject settingSelection2;
	public GameObject settingSelection3;
	public GameObject settingSelection4;
	public GameObject settingSelection5;

	public Text targetLock;
	public Text musicVol;
	public Text sfxVol;
	public GameObject primCol;
	public GameObject secCol;

	public Material primColMat;
	public Material secColMat;

	public GameObject menu;
	public GameObject controls;
	public GameObject credits;
	public GameObject settings;


	public AudioClip ButtonSound;
	public AudioSource source;

	int mode = 0;

	// Use this for initialization
	void Awake () {
		selectionHighlights [0] = selection1.GetComponent<UIfader>();
		selectionHighlights [1] = selection2.GetComponent<UIfader>();
		selectionHighlights [2] = selection3.GetComponent<UIfader>();
		selectionHighlights [3] = selection4.GetComponent<UIfader>();

		settingsHighlights [0] = settingSelection1.GetComponent<UIfader>();
		settingsHighlights [1] = settingSelection2.GetComponent<UIfader>();
		settingsHighlights [2] = settingSelection3.GetComponent<UIfader>();
		settingsHighlights [3] = settingSelection4.GetComponent<UIfader>();
		settingsHighlights [4] = settingSelection5.GetComponent<UIfader>();

		selectionHighlights [0].setActive(true);
		selectionHighlights [1].setActive(false);
		selectionHighlights [2].setActive(false);
		selectionHighlights [3].setActive(false);

		settingsHighlights [0].setActive (false);
		settingsHighlights [1].setActive (false);
		settingsHighlights [2].setActive (false);
		settingsHighlights [3].setActive (false);
		settingsHighlights [4].setActive (false);

		source = GetComponent<AudioSource>();

		if (StaticData.settings_targetlock == true) {
			targetLock.text = "ON";
		} else {
			targetLock.text = "OFF";
		}
		musicVol.text = StaticData.settings_musicVol.ToString();
		sfxVol.text = StaticData.settings_sfxVol.ToString();

		primCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_primColour];
		primColMat.color = StaticData.colours [StaticData.settings_primColour];

		secCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_secColour];
		secColMat.color = StaticData.colours [StaticData.settings_secColour];
		AudioListener.volume = ((float)StaticData.settings_musicVol) / 10f;
	}
		

	// Update is called once per frame
	void Update () {
		if (mode == 0) {
			if (Input.GetKeyDown (KeyCode.DownArrow) && currentSelection != 3) {
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
					settings.SetActive (true);
					menu.SetActive (false);
					settingsHighlights [0].setActive (true);
					source.PlayOneShot (ButtonSound, .1f);
				} else if (currentSelection == 3) {
					mode = 3;
					credits.SetActive (true);
					menu.SetActive (false);
					source.PlayOneShot (ButtonSound, .1f);
				}
			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.Quit ();
			}
		} else if (mode == 2){
			if (Input.GetKeyDown (KeyCode.DownArrow) && currentSettingSelection != 4) {
				settingsHighlights [currentSettingSelection].setActive (false);
				currentSettingSelection++;
				settingsHighlights [currentSettingSelection].setActive (true);
				source.PlayOneShot (ButtonSound, 0.5f);
			}
			if (Input.GetKeyDown (KeyCode.UpArrow) && currentSettingSelection != 0) {
				settingsHighlights [currentSettingSelection].setActive (false);
				currentSettingSelection--;
				settingsHighlights [currentSettingSelection].setActive (true);
				source.PlayOneShot (ButtonSound, 0.5f);
			}
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (currentSettingSelection == 0) {
					if (StaticData.settings_targetlock == true) {
						StaticData.settings_targetlock = false;
						targetLock.text = "OFF";
					} else {
						StaticData.settings_targetlock = true;
						targetLock.text = "ON";
					}
				}
				if (currentSettingSelection == 1) {
					if (StaticData.settings_musicVol > 0) {
						StaticData.settings_musicVol--;
						AudioListener.volume = ((float)StaticData.settings_musicVol) / 10f;
						musicVol.text = StaticData.settings_musicVol.ToString ();
					}
						
				}
				if (currentSettingSelection == 2) {
					if (StaticData.settings_sfxVol > 0) {
						StaticData.settings_sfxVol--;
						sfxVol.text = StaticData.settings_sfxVol.ToString ();
					}
				}
				if (currentSettingSelection == 3) {
					StaticData.settings_primColour = (StaticData.settings_primColour - 1)  < 0 ?  8 : (StaticData.settings_primColour - 1);;
					primCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_primColour];
					primColMat.color = StaticData.colours [StaticData.settings_primColour];
				}
				if (currentSettingSelection == 4) {
					StaticData.settings_secColour = (StaticData.settings_secColour - 1)  < 0 ?  8 : (StaticData.settings_secColour - 1);
					secCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_secColour];
					secColMat.color = StaticData.colours [StaticData.settings_secColour];
				}
			}
			if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (currentSettingSelection == 0) {
					if (StaticData.settings_targetlock == true) {
						StaticData.settings_targetlock = false;
						targetLock.text = "OFF";
					} else {
						StaticData.settings_targetlock = true;
						targetLock.text = "ON";
					}
				}
				if (currentSettingSelection == 1) {
					if (StaticData.settings_musicVol < 10) {
						StaticData.settings_musicVol++;
						musicVol.text = StaticData.settings_musicVol.ToString ();
						AudioListener.volume = ((float)StaticData.settings_musicVol) / 10f;
					}
				}
				if (currentSettingSelection == 2) {
					if (StaticData.settings_sfxVol < 10) {
						StaticData.settings_sfxVol++;
						sfxVol.text = StaticData.settings_sfxVol.ToString ();
					}
				}
				if (currentSettingSelection == 3) {
					StaticData.settings_primColour = (StaticData.settings_primColour + 1) % 9;
					primCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_primColour];
					primColMat.color = StaticData.colours [StaticData.settings_primColour];
				}
				if (currentSettingSelection == 4) {
					StaticData.settings_secColour = (StaticData.settings_secColour + 1) % 9;
					secCol.GetComponent<Image> ().color = StaticData.colours [StaticData.settings_secColour];
					secColMat.color = StaticData.colours [StaticData.settings_secColour];
				}

			}
			if (Input.GetKeyDown (KeyCode.Escape)) {
				mode = 0;
				currentSettingSelection = 0;
				settings.SetActive (false);
				menu.SetActive (true);
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
