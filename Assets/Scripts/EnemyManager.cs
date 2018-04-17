using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class EnemyManager : MonoBehaviour {
    public GameObject player;
    public GameObject enemy;                // The enemy prefab to be spawned.
	public GameObject GAManager;

	public GameObject pauseManager;
	public int waveSize;			
    public List<GameObject> enemies=new List<GameObject>();
    public List<GameObject> deadEnemies=new List<GameObject>();
    public Text shipsRemainingText;
    public Text waveCompleteText;
	public Text waveStartText;
	public Text playerScoreText;
	public GameObject deadEnemyExplosion;

    private float timeTillNextWave;
    private bool atEndOfWave = false;
    public GameObject waveCompleteWrapper;
	public GameObject startWaveWrapper;
	public float playerScore;

    public int currentWave = 0;

    public AudioClip WaveCompletedSound;
    public AudioSource source;

	public GameObject ShieldPowerup;
	public GameObject SpeedPowerup;
	public GameObject WeaponPowerup;

	int enemiesKilled = 0;

    void Start ()
	{
        source = GetComponent<AudioSource>();
		player.GetComponent<PlayerControls> ().enemies = enemies;
	}

    private void Update()
    {
		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;
		if (currentWave == 0) {
			updateInitalWave();
			writeShipsRemaining();
			writePlayerScore ();
			return;
		}
       if (enemies.Count > 0)
        {
            for(int enemyIndex=enemies.Count-1; enemyIndex>=0; enemyIndex--)
            {
                if (enemies[enemyIndex].GetComponent<EnemyBrain>().health <=0)
				{
					enemiesKilled++;
					float[] genes = enemies [enemyIndex].GetComponent<EnemyBrain> ().getGenes ();
					BoosterDrop(genes, enemies[enemyIndex].gameObject.transform.position);
					foreach (float gene in genes) {
						playerScore += gene;

					}
					StaticData.score = playerScore;
                    //add it to the dead enemy list
                    deadEnemies.Add(enemies[enemyIndex]);


                    //destroy the game object and remove it from the list
					enemies[enemyIndex].SetActive(false);
					var ex = Instantiate(deadEnemyExplosion, enemies [enemyIndex].transform.position, Quaternion.identity);
					ex.GetComponent<ParticleSystem> ().Play ();
					Destroy (ex, 0.5f);
                    enemies.RemoveAt(enemyIndex);
				}
            }
            writeShipsRemaining();
			writePlayerScore ();
        } else {
            updateEndOfWave();
		}
    }

    void SpawnInital (GameObject privateEnemy)
	{
		for (int i = 0; i < waveSize; i++) {
            makeNewRandomEnemy(privateEnemy);
        }
    }

	public void spawnGA(List<float[]> newGAPopulation)
	{
        currentWave++;
        for (int i = 0; i < waveSize; i++)
        {
            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject newEnemy = Instantiate(enemy, GenerateRandomTransform(), enemy.GetComponent<Rigidbody>().rotation);
            newEnemy.GetComponent<EnemyBrain>().player = this.player;
			newEnemy.GetComponent<EnemyBrain>().otherEnemies = this.enemies;
			newEnemy.GetComponent<EnemyBrain>().setGenoPheno (newGAPopulation [i]);
			newEnemy.GetComponent<EnemyBrain>().pauseManager = pauseManager;
            enemies.Add(newEnemy); //adding all enemies created to the list

            //deadEnemies.Add(newEnemy);
        }
		for (int i = deadEnemies.Count - 1; i >= 0; i--) {
			Destroy (deadEnemies [i]);
		}
        deadEnemies = new List<GameObject>();
    }

    void makeNewRandomEnemy(GameObject privateEnemy)
    {
        // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
        GameObject newEnemy = Instantiate(privateEnemy, GenerateRandomTransform(), privateEnemy.GetComponent<Rigidbody>().rotation);
        newEnemy.GetComponent<EnemyBrain>().player = this.player;
        newEnemy.GetComponent<EnemyBrain>().otherEnemies = this.enemies;
		newEnemy.GetComponent<EnemyBrain>().pauseManager = pauseManager;

        float[] rawGenes = new float[2];
		for (int i = 0; i < rawGenes.Length; i++) {
			rawGenes [i] = Random.value*10;
		}
		newEnemy.GetComponent<EnemyBrain> ().setGenoPheno (rawGenes);
        enemies.Add(newEnemy);
    }

    Vector3 GenerateRandomTransform(){
        Vector3 pos;
        float x = Random.Range(player.transform.position.x-4000, player.transform.position.x + 4000);
		float y = Random.Range(player.transform.position.y-4000, player.transform.position.y + 4000);
        float z = Random.Range(player.transform.position.z - 4000, player.transform.position.z + 4000);
        pos = new Vector3(x, y, z);
        return pos;
    }

    private void writeShipsRemaining()
    {
        shipsRemainingText.text = enemies.Count + "/" + waveSize;
    }

	private void writePlayerScore()
	{
		playerScoreText.text = playerScore.ToString("N0");
	}

	private void updateInitalWave()
	{
		if (!startWaveWrapper.activeSelf)
		{
			timeTillNextWave = 3f;
			startWaveWrapper.SetActive( true);
		} 

		if(timeTillNextWave < 0.0001)
		{
			SpawnInital (enemy);
			currentWave = 1;
			startWaveWrapper.SetActive(false);
		}
		waveStartText.text = timeTillNextWave.ToString("N0");
		timeTillNextWave -= Time.deltaTime;
	}

    private void updateEndOfWave()
    {
        if (!atEndOfWave)
        {
            source.PlayOneShot(WaveCompletedSound, 2.0f);
            atEndOfWave = true;
            timeTillNextWave = 3f;
            waveCompleteWrapper.SetActive( true);
        } 

        if(timeTillNextWave < 0.0001)
        {
            scaleFitnessVariables();
			List<float[]> newPop = GAManager.GetComponent<GAmanager>().getNextWavePopulation(deadEnemies);

            spawnGA(newPop);
            if (currentWave % 2 == 1)
            {
//                waveSize += 4;
//                makeNewRandomEnemy(enemy);
//                makeNewRandomEnemy(enemy);
//				makeNewRandomEnemy(enemy);
//				makeNewRandomEnemy(enemy);

				waveSize += 2;
				makeNewRandomEnemy(enemy);
				makeNewRandomEnemy(enemy);

            }
            atEndOfWave = false;
            waveCompleteWrapper.SetActive(false);
        }
        displayWaveComplete();
        timeTillNextWave -= Time.deltaTime;
    }

    private void displayWaveComplete()
    {
		waveCompleteText.text = timeTillNextWave.ToString("N0");
    }

    private void scaleFitnessVariables()
    {
		float damageTotal = 0;
		float timeTotal = 0;
		float fitnessTotal = 0;
        //getting the max value
        float timeMax = 0;
        float damageMax = 0;
        foreach (GameObject enemy in deadEnemies)
        {
           // if (enemy.GetComponent<EnemyBrain>().timeAliveTimer > timeMax) timeMax = enemy.GetComponent<EnemyBrain>().timeAliveTimer;
            //if (enemy.GetComponent<EnemyBrain>().damageDealt > damageMax) damageMax = enemy.GetComponent<EnemyBrain>().damageDealt;
			damageTotal += enemy.GetComponent<EnemyBrain>().damageDealt;
			timeTotal += enemy.GetComponent<EnemyBrain>().timeAliveTimer;
			fitnessTotal += (enemy.GetComponent<EnemyBrain> ().damageDealt / enemy.GetComponent<EnemyBrain> ().timeAliveTimer);
        }

        //mapping from 0 to 1
        //foreach (GameObject enemy in deadEnemies)
        //{
            //enemy.GetComponent<EnemyBrain>().timeAliveTimer = map(enemy.GetComponent<EnemyBrain>().timeAliveTimer, timeMax, 1);
            //enemy.GetComponent<EnemyBrain>().damageDealt = map(enemy.GetComponent<EnemyBrain>().damageDealt, damageMax, 1);
			//fitnessTotal += map(enemy.GetComponent<EnemyBrain>().timeAliveTimer, timeMax, 1);
			//fitnessTotal += map(enemy.GetComponent<EnemyBrain>().damageDealt, damageMax, 1);

        //}

		float average = fitnessTotal / (float)deadEnemies.Count;
		float averageD = damageTotal / (float)deadEnemies.Count;
		float averageT = timeTotal / (float)deadEnemies.Count;
		print ("Average Fitness: " + fitnessTotal + " Average damage: " + averageD + " Average time: " + averageT);
    }

	public void BoosterDrop(float[] genes, Vector3 position)
	{
		float pBooster = Random.value;
		if (pBooster > 0.3) return; //70% of the time there is no drop

//		List<float> booster = new List< float > ();
//		for (int i = 0; i < 3; i++) booster.Add(genes[i]);
		int boosterType = 0;

		//if (pBooster <= 0.1) boosterType = booster.IndexOf(booster.Max()); //10% we drop based on the type
		if (pBooster <= 0.1) boosterType = Random.Range(0, 3); //10% we drop based on the type
		else if (pBooster <= 0.2) boosterType = Random.Range(0, 3); //10% we drop random 
		//The other 10% we do nothing (so drop a shield)
			
		if (boosterType == 0) {
			GameObject boosterDrop = Instantiate(ShieldPowerup, position, transform.rotation);
			//boosterDrop.GetComponent<Booster>().boostAmount = booster[boosterType];
			boosterDrop.GetComponent<Booster>().boostAmount = 5;
		} else if (boosterType == 1) {
			GameObject boosterDrop = Instantiate(SpeedPowerup, position, transform.rotation);
			//boosterDrop.GetComponent<Booster>().boostAmount = booster[boosterType];
			boosterDrop.GetComponent<Booster>().boostAmount = 5;
		} else {
			GameObject boosterDrop = Instantiate(WeaponPowerup, position, transform.rotation);
			//boosterDrop.GetComponent<Booster>().boostAmount = booster[boosterType];
			boosterDrop.GetComponent<Booster>().boostAmount = 5;
		}
	}
		
    private static float map(float initialVal, float initialHigh, float targetHigh)
    {
        return ((initialVal * targetHigh) / initialHigh);
    }
}



