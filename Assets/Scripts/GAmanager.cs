using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAmanager : MonoBehaviour {

    int populationSize=40;

    //create a new game object to store the list of enemy clones
    public GameObject enemyManager;
    public List<GameObject> enemyClones;

    // Use this for initialization
    void Start () {
        enemyClones = enemyManager.GetComponent<EnemyManager>().enemies;

        testGA();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void testGA()
    {
        //Debug.Log("Start the testGA");
        //create population
        GApopulation testPopulation = new GApopulation(enemyClones); //this will also create a random population of enemies;
        //Debug.Log(2);

        PrintFitness(testPopulation);
        //Debug.Log("3");

        testPopulation.nextGeneration();
        PrintFitness(testPopulation);


        testPopulation.nextGeneration();
        PrintFitness(testPopulation);



        //next generation
        //next generation
    }

    //for test only
    public void PrintFitness(GApopulation p)
    {
        //Debug.Log("PrintCalled1");
        for (int index = 0; index < p.getDictionary().Count; index++)
        {
           Debug.Log(index + " fitness: " + p.getDictionary()[index].getFitness()+"dic size:"+ p.getDictionary().Count);
            //Debug.Log("");
        }
    }
}
