using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GApopulation {

    public IDictionary<int, GAenemy> population;// = new IDictionary<int, GAenemy>();
    public int populationSize;
    public System.Random r = new System.Random();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //constructor:creates the dict
    public GApopulation(int size)
    {
        this.populationSize = size;
        this.population = new Dictionary<int, GAenemy>();
        createPopulation();
    }


    //creates population
    public void createPopulation()
    {
        for (int index = 0; index < this.populationSize; index++)
        {
            GAenemy e = new GAenemy(randomValues(), randomValues(), randomValues(), randomValues(),
                randomValues(), randomValues(), randomValues(), randomValues());
            this.population[index] = e;

        }
    }

    //selection: tournament selection with prob of 0.7
    public void selection()
    {
        int selectionPropbability = 70;
        int initialPopulationSize = this.populationSize;
        //IDictionary <int, GAenemy> selectedPopulation = this.population;
        IDictionary<int, GAenemy> selectedPopulation = new Dictionary<int, GAenemy>();

        int newPopulationIndex = 0;

        while (selectedPopulation.Count < initialPopulationSize / 2)
        {

            GAenemy e1 = this.population[r.Next(0, this.population.Count)];
            GAenemy e2 = this.population[r.Next(0, this.population.Count)];

            if (r.Next(0, 100) > selectionPropbability)
            {   if (e1.getFitness() > e2.getFitness())
                {
                    selectedPopulation[newPopulationIndex] = e2;
                }else
                {
                    selectedPopulation[newPopulationIndex] = e1;
                }

            }
            else
            {
                if (e1.getFitness() > e2.getFitness())
                {
                    selectedPopulation[newPopulationIndex] = e1;
                }
                else
                {
                    selectedPopulation[newPopulationIndex] = e2;
                }

            }

            newPopulationIndex++;
        }

        //clear all pop from the initial dict
        this.population.Clear();
        //add all selected pop to the init dict
        for (int i = 0; i < selectedPopulation.Count; i++)
        {
            this.population[i] = selectedPopulation[i];
        }
        
        // this.population = selectedPopulation;//not sure if this works!!!!


    }

    //crossover (returns a offspring)
    public void crossOver()
    {
        IDictionary<int, GAenemy> offspringPopulation=new Dictionary<int, GAenemy>();
        int newPopulationSize = 0; //= this.population.Count * 2; //two times the original population. The parents are added to the population

        while (this.population.Count < newPopulationSize)
        {
            //randomly pick 2 individuals from pop and crossover them
            GAenemy e1 = this.population[r.Next(0, this.population.Count)];
            GAenemy e2 = this.population[r.Next(0, this.population.Count)];

            //for each gene in the chromozone (NOTE: ADD all the genes in an array for easier acces) 0,2
            GAenemy newOffspring = newChild(e1, e2);
            offspringPopulation[newPopulationSize] = newOffspring;
            newPopulationSize++;
        }

        //add the offspring to the whole population
        int initialPopulationSize = this.population.Count;
        for (int i =0; i < offspringPopulation.Count; i++)
        {
            this.population[initialPopulationSize + i] = offspringPopulation[i]; 
        }
    }

    //mutation (returns a offspring)
    public void mutation()
    {
        //for all the genes in the chormosome, add a gausian.
        for (int index =0; index < this.population.Count; index++)
        {
            this.population[index].setHealth(this.population[index].getHealth() + Gaussian(0, 0.8));//gaussian is called with mean and sddev (sddev:0.7 to 1.5)
            this.population[index].setSpeed(this.population[index].getSpeed() + Gaussian(0, 0.8));
            this.population[index].setBulletSpeed(this.population[index].getBulletSpeed() + Gaussian(0, 0.8));
            this.population[index].setBulletDamage(this.population[index].getBulletDamage() + Gaussian(0, 0.8));

            this.population[index].setPlayerFleeBuffer(this.population[index].getPlayerFleeBuffer() + Gaussian(0, 0.8));
            this.population[index].setPlayerFleeDistance(this.population[index].getPlayerFleeDistance() + Gaussian(0, 0.8));
            this.population[index].setPlayerSeekDistance(this.population[index].getPlayerSeekDistance() + Gaussian(0, 0.8));
            this.population[index].setBulletFleeDistance(this.population[index].getBulletFleeDistance() + Gaussian(0, 0.8));

        }
    }

    public IDictionary <int, GAenemy> getDictionary()
    {
        return this.population;
    }
    //next generation
    public void nextGeneration()
    {
        //after the population is created, do selection, crossover, mutation 
        selection();
        crossOver();
        mutation();
    } 

    //sorting 
    public static void fitnessQuickSort(IDictionary<int, GAenemy> currentPopulation, int lowerIndex, int higherIndex)
    {

        //storing the lower and upper boundaries 
        int i = lowerIndex;
        int j = higherIndex;

        float p = currentPopulation[lowerIndex + (higherIndex - lowerIndex) / 2].getFitness();

        while (i <= j)
        {

            while (currentPopulation[i].getFitness() < p)
            {
                i++;
            }
            while (currentPopulation[j].getFitness() > p)
            {
                j--;
            }
            if (i <= j)
            {
                //swap in the dict.
                GAenemy temp = currentPopulation[i];
                currentPopulation[i] = currentPopulation[j];
                //currentPopulation.put(i, map.get(j));
                currentPopulation[j] = temp;
                i++;
                j--;
            }
        }
        // call quickSort recursively
        if (lowerIndex < j)
            fitnessQuickSort(currentPopulation, lowerIndex, j);
        if (i < higherIndex)
            fitnessQuickSort(currentPopulation, i, higherIndex);
    }

    //OTHER
    //temp function for rand values
    private int randomValues()
    {
        return r.Next(0, 10);
    }

    GAenemy newChild(GAenemy e1, GAenemy e2)
    {
        //health
        float newHealth;
        if (r.Next(0, 2) == 0)
        {
            newHealth = e1.getHealth();
        }
        else
        {
            newHealth = e2.getHealth();

        }
        //speed
        float newSpeed;
        if (r.Next(0, 2) == 0)
        {
            newSpeed = e1.getSpeed();
        }
        else
        {
             newSpeed = e2.getSpeed();

        }
        //BulletDamage
        float newBulletDamage;
        if (r.Next(0, 2) == 0)
        {
             newBulletDamage = e1.getBulletDamage();
        }
        else
        {
             newBulletDamage = e2.getBulletDamage();

        }
        //BulletSpeed
        float newBulletSpeed;
        if (r.Next(0, 2) == 0)
        {
             newBulletSpeed = e1.getBulletSpeed();
        }
        else
        {
             newBulletSpeed = e2.getBulletSpeed();

        }

        //getBulletFleeDistance
        float newBulletFleeDistance;
        if (r.Next(0, 2) == 0)
        {
             newBulletFleeDistance = e1.getBulletFleeDistance();
        }
        else
        {
             newBulletFleeDistance = e2.getBulletFleeDistance();

        }
        //getPlayerFleeBuffer
        float newPlayerFleeBuffer;
        if (r.Next(0, 2) == 0)
        {
             newPlayerFleeBuffer = e1.getPlayerFleeBuffer();
        }
        else
        {
            newPlayerFleeBuffer = e2.getPlayerFleeBuffer();
        }
        //getPlayerFleeDistance
        float newPlayerFleeDistance;
        if (r.Next(0, 2) == 0)
        {
             newPlayerFleeDistance = e1.getPlayerFleeDistance();
        }
        else
        {
             newPlayerFleeDistance = e2.getPlayerFleeDistance();
        }
        //newPlayerSeekDistance
        float newPlayerSeekDistance;
        if (r.Next(0, 2) == 0)
        {
             newPlayerSeekDistance = e1.getPlayerSeekDistance();
        }
        else
        {
             newPlayerSeekDistance = e2.getPlayerSeekDistance();
        }

        GAenemy newChild = new GAenemy(newHealth, newSpeed, newBulletSpeed, newBulletDamage, newPlayerSeekDistance,
            newPlayerFleeDistance, newPlayerFleeBuffer, newBulletFleeDistance);

    
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

    /*
     public void PrintFitness()
    {
        for (int index = 0; index < this.population.Count; index++)
        {
            print(index+" fitness: "+this.population[index].getFitness());
            print("");
}
    }
    */

    
}
