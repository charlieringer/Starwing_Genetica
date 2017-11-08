using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//#region Singleton Check: there's just one copy of a certain 
	private static GameManager gameManagerInstance = null;

	public static GameManager Instance {
		get { return gameManagerInstance; }
	}

	void Awake ()
	{
		if (gameManagerInstance != null && gameManagerInstance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			gameManagerInstance = this;
		}
		DontDestroyOnLoad (this.gameObject);
	}
	//#endregion


	//public EnemyBrain someEnemyBrain;


	// Use this for initialization
	void Start () {
		//someEnemyBrain.shooting ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}



