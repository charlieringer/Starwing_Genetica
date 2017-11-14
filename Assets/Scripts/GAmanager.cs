using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GAmanager : MonoBehaviour {

    //int populationSize=40;
    //string path = "D:/Uni/IGGI/courses/Goldsmiths module 1/IGGI_Game/Assets/test.txt";
    //StreamWriter writer;

    //create a new game object to store the list of enemy clones
    public GameObject enemyManager;
    public List<GameObject> enemyClones;

  
        // Use this for initialization
    void Start () {
        
       //writer = System.IO.File.AppendText(path);
       // writer = new StreamWriter(path, true);
    }
	
	void Update () {
    }

    private void OnDestroy()
    {
        //writer.Close();
    }

	public IDictionary<int, GAenemy>  getNextWavePopulation(List<GameObject> oldEnemies)
	{

		//create population with a list of game objects
		GApopulation population = new GApopulation(oldEnemies);

        //generate the next generation for the next wave
		population.generateNextGeneration();


        //add the next generation to a dictionary 
        IDictionary<int, GAenemy> nextGen = population.getDictionary ();

        //return the dictionary containing the next generation for the next wave
		return nextGen;
	}

    //for test only
    /*
    public void PrintFitness(GApopulation p)
    {
        //Debug.Log("PrintCalled1");
        for (int index = 0; index < p.getDictionary().Count; index++)
        {
            Debug.Log(index + " gene: " );
            PrintCollectionToFile(p.getDictionary()[index].GetGene());

            //print(System.String.Join("; ", p.getDictionary()[index].GetGene()));
            //Debug.Log(index + " fitness: " + p.getDictionary()[index].getFitness()+"dic size:"+ p.getDictionary().Count);
            //Debug.Log("");
        }
    }
    */

    /*
    public void PrintCollectionToFile<T>(IEnumerable<T> col)
    {
        string currentInfo="next: ";

        int index = 0;
        foreach (var item in col)
        {
            Debug.Log(index+"-"+item);
            currentInfo = currentInfo + index +":"+ item.ToString()+ "; ";
            index++;
        }

        
        //writer.WriteLine(currentInfo);
        
    }
    */
}
