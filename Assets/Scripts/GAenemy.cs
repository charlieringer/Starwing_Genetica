using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAenemy  {
    float[] genes;

    //used to calculate fitness
    float playerDamage;
    float lifeSpan;


    public GAenemy(float[] newGene, float _lifeSpam, float _damage)
    {
		genes = new float[newGene.Length];
        for(int i = 0; i < this.genes.Length; i++){ this.genes[i] = newGene[i]; }
        
        this.playerDamage = _damage;
        this.lifeSpan = _lifeSpam;

    }
    //create the enemy object
    //calculates fitness: sum of the player damage and the lifespam;
    public float getFitness()
    {
		//return playerDamage + lifeSpan;
		return playerDamage/lifeSpan;
    }

	public void setGenes(float[] g)
	{
		genes = g;
	}

	public void setData(float[] g, float d, float l)
	{
		genes = g;
		lifeSpan = l;
		playerDamage = d;
	}
		
    public float [] getGenes()
    {
        return this.genes;
    }
 
    public float getLifeSpan()
    {
        return this.lifeSpan;
    }
    public float getPlayerDamage()
    {
        return this.playerDamage;
    }
}


