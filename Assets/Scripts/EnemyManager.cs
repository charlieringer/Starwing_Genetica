using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
    //public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    public GameObject player;
    public GameObject enemy;                // The enemy prefab to be spawned.
	public float spawnTime = 3f;            // How long between each spawn.
	public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
	public int waveSize;					// how many enemies are spawning each wave


	//check health. make the enemy die;

	void Start ()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, 0);//spawnTime);
	}


	void Spawn ()
	{
		//TO ADD the player health!
		// If the player has no health left...
		//if(playerHealth.currentHealth <= 0f)
		//{
			// ... exit the function.
	//		return;
	//	}

		for (int i = 0; i < waveSize; i++) {

			// Find a random index between zero and one less than the number of spawn points.
			int spawnPointIndex = Random.Range (0, spawnPoints.Length);
			print("--->" + spawnPoints.Length);

			// Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			Instantiate (enemy, GenerateRandomTransform(), spawnPoints[spawnPointIndex].rotation);
		}
	}
       
	Vector3 GenerateRandomTransform(){
        Vector3 pos;
        float x = Random.Range(player.transform.position.x-1000, player.transform.position.x + 1000);
        float y = 0f;
        float z = Random.Range(player.transform.position.z - 1000, player.transform.position.z + 1000);
        pos = new Vector3(x, y, z);
        transform.position = pos;

        return pos;
    }

}



