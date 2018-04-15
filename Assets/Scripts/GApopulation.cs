using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GApopulation {

    public List<GAenemy> population;// = new IDictionary<int, GAenemy>();
    public int populationSize;
    public System.Random r = new System.Random();

 

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //constructor:creates the dict
    public GApopulation(List<GameObject> enemyClones)
    {
        this.populationSize = enemyClones.Count;
        this.population = new List<GAenemy>();
        createPopulation(enemyClones);
    }
		
    //creates population
    public void createPopulation(List<GameObject> enemyClones)
    {
        int currentPopulationIndex = 0;
        foreach (GameObject enemyClone in enemyClones)
        {
			float[] gene = enemyClone.GetComponent<EnemyBrain>().getGenes();

            GAenemy e = new GAenemy(gene,enemyClone.GetComponent<EnemyBrain>().timeAliveTimer, enemyClone.GetComponent<EnemyBrain>().damageDealt);
			this.population.Add(e);

            currentPopulationIndex++;
        }
    }

    //selection: tournament selection with prob of 0.7
    public void selection()
    {
        int selectionPropbability = 70;
        int initialPopulationSize = this.populationSize;
        //IDictionary <int, GAenemy> selectedPopulation = this.population;
        List<GAenemy> selectedPopulation = new List<GAenemy>();

        int newPopulationIndex = 0;

        while (selectedPopulation.Count < initialPopulationSize / 2)
        {
            int enemyOneIndex = r.Next(0, population.Count);
            int enemyTwoIndex = r.Next(0, population.Count);

            GAenemy e1 = new GAenemy(population[enemyOneIndex].getGenes(),
                population[enemyOneIndex].getLifeSpan(),
                this.population[enemyOneIndex].getPlayerDamage());

            GAenemy e2 = new GAenemy(this.population[enemyTwoIndex].getGenes(),
                this.population[enemyTwoIndex].getLifeSpan(),
                this.population[enemyTwoIndex].getPlayerDamage());

            if (r.Next(0, 100) > selectionPropbability)
            {   
				if (e1.getFitness() > e2.getFitness()) selectedPopulation.Add(e2);
                else selectedPopulation.Add (e1);
            }
            else
            {
				if (e1.getFitness() > e2.getFitness()) selectedPopulation.Add(e1);
				else selectedPopulation.Add(e2);
            }

            newPopulationIndex++;
        }
		this.population = selectedPopulation;
    }

    public void crossOver()
    {
        List<GAenemy> offspringPopulation=new List<GAenemy>();
		int offspringSize = this.population.Count; //

		while (offspringPopulation.Count <= offspringSize)
        {
            GAenemy e1 = population[r.Next(0, population.Count)];
            GAenemy e2 = population[r.Next(0, population.Count)];

            GAenemy newOffspring = newChild(e1, e2);
			offspringPopulation.Add(newOffspring);
        }
		this.population.AddRange(offspringPopulation);
    }
		
    public void mutation()
	{
        //for all the genes in the chormosome, add a gausian.
        for (int index =0; index < this.population.Count; index++)
        {
			float[] genes = population [index].getGenes ();
			for (int i = 0; i < genes.Length; i++) {
				genes [i] += Gaussian (0, 0.8);
				genes [i] = genes [i] % 10;
			}
			population[index].setData(genes,0f,0f);//the lifespam and damage dealt is set  to 0 as there won't be any fitness comparison and these values are going to be updated in-game
        }
    }

	public List<float[]> getPopulationOfGenes() { 
		List<float[]> allGenes = new List<float[]> ();
		for (int i = 0; i < population.Count; i++) allGenes.Add (population [i].getGenes());
		return allGenes; 
	}
    
    //next generation
    public void generateNextGeneration()
    {
        //after the population is created, do selection, crossover, mutation 
        selection();
        crossOver();
        mutation();
    } 

    GAenemy newChild(GAenemy e1, GAenemy e2)
    {
        int geneLen = e1.getGenes().Length; //store the lenght of a gene
		float[] newGenes = new float[geneLen]; //create another array to store the new gene values

        //iterate and randomly decide which parent pass on its gene
        for (int geneIndex=0; geneIndex<e1.getGenes().Length; geneIndex++)
        {
            if (r.Next(0, 2) == 0) newGenes[geneIndex] = e1.getGenes()[geneIndex];
            else newGenes[geneIndex] = e2.getGenes()[geneIndex];
        }

        //create and return the child
        GAenemy newChild = new GAenemy(newGenes, 0, 0);
        return newChild;
    }

    public static float Gaussian( double mean, double stddev)
    {
        // The method requires sampling from a uniform random of (0,1]
        // but Random.NextDouble() returns a sample of [0,1).
        System.Random r = new System.Random();
        double x1 = 1 - r.NextDouble();
        double x2 = 1 - r.NextDouble();

        double y1 = System.Math.Sqrt(-2.0 * System.Math.Log(x1)) * System.Math.Cos(2.0 * System.Math.PI * x2);
        return (float) (y1 * stddev + mean);
    }  
}
