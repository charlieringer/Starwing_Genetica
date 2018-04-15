using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GAmanager : MonoBehaviour {

    //create a new game object to store the list of enemy clones
    public GameObject enemyManager;
    public List<GameObject> enemyClones;

	public List<float[]>  getNextWavePopulation(List<GameObject> oldEnemies)
	{
		//create population with a list of game objects
		GApopulation population = new GApopulation(oldEnemies);

        //generate the next generation for the next wave
        population.generateNextGeneration();

        //return the dictionary containing the next generation for the next wave
		return population.getPopulationOfGenes ();
	} 
}
