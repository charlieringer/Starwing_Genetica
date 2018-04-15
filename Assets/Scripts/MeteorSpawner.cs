using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour {

	public int numMeteors;

	public int spawnRange;
	public GameObject prefab;

	// Use this for initialization
	void Awake () {
		for(int i = 0; i < numMeteors; i++)
		{
			float randX = Random.Range (-spawnRange, spawnRange);
			float randY = Random.Range (-spawnRange, spawnRange);
			float randZ = Random.Range (-spawnRange, spawnRange);

			GameObject met = Instantiate (prefab, new Vector3(randX, randY, randZ), Quaternion.identity);
			int size = Random.Range(1,4);
			met.transform.localScale = new Vector3 (size, size, size);
			met.GetComponent<MeteorController>().meteorPrefab = prefab;
			met.SetActive (true);
		}
	}
}
