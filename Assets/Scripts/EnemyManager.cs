using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour {
    //public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    public GameObject player;
    public GameObject enemy;                // The enemy prefab to be spawned.
	public GameObject GAManager;
	//public float spawnTime = 3f;          // How long between each spawn.
	public int waveSize;					// how many enemies are spawning each wave
    public List<GameObject> enemies=new List<GameObject>();        //dinamic list of enemies
    public List<GameObject> deadEnemies=new List<GameObject>();        //dinamic list of dead enemies used to create the GA population and next generation

    public Text shipsRemainingText;
    public Text waveCompleteText;
    //check health. make the enemy die;

    private float timeTillNextWave;
    private bool atEndOfWave = false;
    public GameObject waveCompleteWrapper;

    void Start ()
	{
        //GameObject privateEnemy = enemy;
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        //InvokeRepeating ("Spawn", privateEnemy, spawnTime, 0);//spawnTime);
        SpawnInital(enemy);
	}

    private void Update()
    {
        
        //print("enemies.Count:" + enemies.Count);
        //check the enemy health and if it's lower than 0, distroy the player
       if (enemies.Count > 0)
        {
            for(int enemyIndex=enemies.Count-1; enemyIndex>=0; enemyIndex--)
            {
                //print("enemyIndex:" + enemyIndex + " enemies[enemyIndex].GetComponent<EnemyBrain>().health:" + enemies[enemyIndex].GetComponent<EnemyBrain>().health);
                if (enemies[enemyIndex].GetComponent<EnemyBrain>().health <=0)
                {
					enemies[enemyIndex].GetComponent<Rigidbody>().useGravity = true;
					//enemies[enemyIndex].transform.Rotate (20, 0, 20);
                    //enemies.RemoveAt(enemyIndex);
                    //print("destroied an enemy");
                }
				if(enemies[enemyIndex].transform.position.y < -200)
				{
                    //add it to the dead enemy list
                    deadEnemies.Add(enemies[enemyIndex]);
                    //destroy the game object and remove it from the list
                    Destroy(enemies[enemyIndex]);
                    enemies.RemoveAt(enemyIndex);
				}
            }
            writeShipsRemianing();
        } else {

            updateEndOfWave();
			//SceneManager.LoadScene ("NextWave");
		}
    }

    void SpawnInital (GameObject privateEnemy)
	{
		for (int i = 0; i < waveSize; i++) { 
            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject newEnemy = Instantiate(privateEnemy, GenerateRandomTransform(), privateEnemy.GetComponent<Rigidbody>().rotation);
			newEnemy.GetComponent<EnemyBrain>().player = this.player;
			newEnemy.GetComponent<EnemyBrain>().otherEnemies = this.enemies;

			float[] rawGenes = new float[7];

			for(int j = 0; j < rawGenes.Length; j++) rawGenes[j] = Random.Range(0,10);

			newEnemy.GetComponent<EnemyBrain> ().setGenoPheno (rawGenes);
				
            enemies.Add(newEnemy); //adding all enemies created to the list
		}
	}

    public void SpawnGA(IDictionary<int, GAenemy> newGAPopulation)
    {//privateEnemy gameObj (see the other Spawn function) is replaced straight with the public enemy gameobject;
        for (int i = 0; i < waveSize; i++)
        {
            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject newEnemy = Instantiate(enemy, GenerateRandomTransform(), enemy.GetComponent<Rigidbody>().rotation);
            newEnemy.GetComponent<EnemyBrain>().player = this.player;
			newEnemy.GetComponent<EnemyBrain>().otherEnemies = this.enemies;
			newEnemy.GetComponent<EnemyBrain> ().setGenoPheno (newGAPopulation [i].GetGene());

            enemies.Add(newEnemy); //adding all enemies created to the list
        }
    }

    Vector3 GenerateRandomTransform(){
        Vector3 pos;
        float x = Random.Range(player.transform.position.x-4000, player.transform.position.x + 4000);
        float y = 0f;
        float z = Random.Range(player.transform.position.z - 4000, player.transform.position.z + 4000);
        pos = new Vector3(x, y, z);
        transform.position = pos;

        return pos;
    }

    private void writeShipsRemianing()
    {
        shipsRemainingText.text = "Ships Remaining: " + enemies.Count + "/" + waveSize;
    }

    private void updateEndOfWave()
    {
        if (!atEndOfWave)
        {
            atEndOfWave = true;
            timeTillNextWave = 3f;
            waveCompleteWrapper.SetActive( true);
        } 

        if(timeTillNextWave < 0.0001)
        {
            //Spawn(enemy);//this needs to be removed. the function will be probably called from inside GAmanager script to feed in the new enemy values
			IDictionary<int, GAenemy> newPop = GAManager.GetComponent<GAmanager>().getNextWavePopulation(enemies);
			SpawnGA(newPop);
            atEndOfWave = false;
            waveCompleteWrapper.SetActive(false);
        }
        displayWaveComplete();
        timeTillNextWave -= Time.deltaTime;
    }

    private void displayWaveComplete()
    {
        waveCompleteText.text = ("Next Wave in " + timeTillNextWave);
    }

}



