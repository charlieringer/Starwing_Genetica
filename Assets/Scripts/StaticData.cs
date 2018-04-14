using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData {

	public static float startingShipHealth = 1000;
	public static float startingShipDamage = 50;
	public static float startingShipSpeed = 500;
	public static int startShipSpecial;
	public static float score;

	public static bool settings_targetlock = true;
	public static int settings_musicVol = 0;
	public static int settings_sfxVol = 7;
	public static int settings_primColour = 7;
	public static int settings_secColour = 1;

	public static Color[] colours = {Color.black, Color.blue, Color.cyan, Color.gray, Color.green, Color.magenta, Color.red, Color.white, Color.yellow};

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
