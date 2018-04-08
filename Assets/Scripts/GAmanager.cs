using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GAmanager : MonoBehaviour {

    //int populationSize=40;
    private bool isTesting = false;
    
    StreamWriter writer;

    //create a new game object to store the list of enemy clones
    public GameObject enemyManager;
    public List<GameObject> enemyClones;

  
        // Use this for initialization
    void Start () {
       
    }
	
	void Update () {
    }

    private void OnDestroy()
    {
        if(isTesting) writer.Close();
    }

	public IDictionary<int, GAenemy>  getNextWavePopulation(List<GameObject> oldEnemies)
	{
        
		//create population with a list of game objects
		GApopulation population = new GApopulation(oldEnemies);


        if (isTesting)
        {  for (int index = 0; index < population.getDictionary().Count; index++)
            {
                PrintCollectionToFile("initial individual with fitness: "+population.getDictionary()[index].getFitness()+ "damage: " + population.getDictionary()[index].getPlayerDamage(),
                    population.getDictionary()[index].GetGene());
            }
        }

        //generate the next generation for the next wave
        population.generateNextGeneration();

        if (isTesting)
        {
            for (int index = 0; index < population.getDictionary().Count; index++)
            {
                PrintCollectionToFile("next individual with fitness: "+population.getDictionary()[index].getFitness()+"damage: "+ population.getDictionary()[index].getPlayerDamage(),
                    population.getDictionary()[index].GetGene());
            }
        }
        //add the next generation to a dictionary 
        IDictionary<int, GAenemy> nextGen = population.getDictionary ();

        //return the dictionary containing the next generation for the next wave
		return nextGen;
	}
    
    public void PrintCollectionToFile<T>(string note, IEnumerable<T> col)
    {
        writer.WriteLine(note);

        string currentInfo ="next: ";

        int index = 0;
        foreach (var item in col)
        {
            Debug.Log(index+"-"+item);
            currentInfo = currentInfo + index +">>"+ item.ToString()+ "; ";
            index++;
        }

        
        writer.WriteLine(currentInfo);
        
    }
    
}
