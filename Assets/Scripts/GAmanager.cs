using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAmanager : MonoBehaviour {

    int populationSize=40;

    //create a new game object to store the list of enemy clones
    public GameObject enemyManager;
    public List<GameObject> enemyClones;

    //stupid way of making the testGA run one sigle 
    //time after the list of enemyclones has been created (that's why it can not be placed in Start() )
    //creates a bool variable that is set to true in start when the game starts. it's set to false when the function is first call
    //and in LateUpdate it's used in if statement to call/not call the testGA function
    public bool callFunction;
        
        // Use this for initialization
    void Start () {
        callFunction = true;
        
    }
	
	// Update is called once per frame
	void LateUpdate () {
        enemyClones = enemyManager.GetComponent<EnemyManager>().enemies;
        if (callFunction) testGA();
    }


    public void testGA()
    {
        callFunction = false;
        
        //Debug.Log("Start the testGA");
        //create population
        GApopulation testPopulation = new GApopulation(enemyClones); //this will also create a random population of enemies;
        Debug.Log("Count: "+testPopulation.getDictionary().Count);
        Debug.Log("clonesCount: "+ enemyClones.Count);

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
