using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	List<GameObject> enemies = new List<GameObject>();
	public int populationSize;

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < populationSize; i++) {
			//enemies.Add (spawnEnemy());
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	//GameObject spawnEnemy()
	//{
	//}
}
