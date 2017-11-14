using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAenemy  {

    float fitness;

    //out of 10.
    float[] gene = new float[6];
    /// <summary>
    /// 0: health, 1:speed, 2:bulletSpeed (& bulletDamage) 
    /// 3:playerSeekDistance, 4:playerFleeDistance, 5:EnemiesAvoidDistance
    /// POSSIBLE player projection
    /// </summary>

    //used to calculate fitness
    float playerDamage;
    float lifeSpam;
    float bulletDamage;

    System.Random r = new System.Random();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GAenemy(float[] newGene, float _lifeSpam, float _damage)
    {

        for(int i = 0; i < this.gene.Length; i++){ this.gene[i] = newGene[i]; }

        //this.playerDamage = r.Next(0,9)+1;
        //this.lifeSpam = r.Next(0,9)+1;
        this.bulletDamage = System.Math.Abs(gene[2]-10);

    }
    //create the enemy object
    //calculates fitness: sum of the player damage and the lifespam;
    private void Fitness()
    {
        this.fitness = playerDamage + lifeSpam;
    }

    public float getFitness()
    {
        Fitness();
        return this.fitness;
    }

    //getters 
    public float [] GetGene()
    {
        return this.gene;
    }

    public float getHealth() //index 0
    {
        return this.gene[0];
    }

    public float getSpeed()
    {
        return this.gene[1];
    }

    public float getBulletDamage()
    {
        return this.bulletDamage;// System.Math.Abs(this.gene[2]-10);
    }

    public float getBulletSpeed()
    {
        return this.gene[2];
    }


    //_______3:playerSeekDistance, 4:playerFleeDistance, 5:enemiesAvoidDistance

    public float getPlayerSeekDistance()
    {
        return this.gene[3];
    }
    public float getPlayerFleeDistance()
    {
        return this.gene[4];
    }
    public float getEnemiesAvoidDistance()
    {
        return this.gene[5];
    }
 

    //__________________
    public float getLifeSpam()
    {
        return this.lifeSpam;
    }
    public float getPlayerDamage()
    {
        return this.playerDamage;
    }

    //setters
    public void SetGene(float [] newGene)
    {
        for (int geneIndex = 0; geneIndex < this.gene.Length; geneIndex++)
        {
            this.gene[geneIndex] = newGene[geneIndex];
        }
    }
    public void setHealth(float health)
    {
        this.gene[0]= health;
    }

    public void setSpeed(float speed)
    {
        this.gene[1] = speed;
    }

    public void setBulletDamage(float bulletDamage)
    {
        this.bulletDamage = bulletDamage;
        this.gene[2]= System.Math.Abs(bulletDamage - 10);
    }

    public void setBulletSpeed(float bulletSpeed)
    {
        this.gene[2] = bulletSpeed;
        this.bulletDamage = System.Math.Abs(this.gene[2] - 10); ;
    }


    //_______

    public void setPlayerSeekDistance(float playerSeekDistance)
    {
        this.gene[3] = playerSeekDistance;
    }

    public void setPlayerFleeDistance(float playerFleeDistance)
    {
        this.gene[4] = playerFleeDistance;
    }

    public void setEnemiesAvoidDistance(float enemiesAvoidDistance)
    {
        this.gene[5] = enemiesAvoidDistance;
    }
  


    //__________________
    public void ChangeLifeSpamRand()
    {
         this.lifeSpam = r.Next(0,100);
    }
    public void ChangePlayerDamageRand()
    {
        this.playerDamage=r.Next(0,100);
    }
}


