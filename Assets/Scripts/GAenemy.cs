using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GAenemy  {

    float fitness;

    //out of 10.
    float health;
    float speed;
    float bulletSpeed;
    float bulletDamage;

    float playerSeekDistance; 
    float playerFleeDistance;
    float playerFleeBuffer;
    float bulletFleeDistance;

    //used to calculate fitness
    float playerDamage;
    float lifeSpam;

    System.Random r = new System.Random();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public GAenemy (float h, float s, float bulletS, float bulletD, 
        float playerSeekD, float playerFleeD, float playerFleeB, float bulletFleeD)
    {
        this.health = h;
        this.speed = s;
        this.bulletSpeed = bulletS;
        //the bullet damage will be replaced with |bulletSpeed-10|
        this.bulletDamage = bulletD;
        this.playerSeekDistance = playerSeekD;
        this.playerFleeDistance = playerFleeD;
        this.playerFleeBuffer = playerFleeB;
        this.bulletFleeDistance = bulletFleeD;
        this.bulletDamage = bulletD;
        this.playerDamage = r.Next(0,10);
        this.lifeSpam = r.Next(0,10);

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
    public float getHealth()
    {
        return this.health;
    }

    public float getSpeed()
    {
        return this.speed;
    }

    public float getBulletDamage()
    {
        return this.bulletDamage;
    }

    public float getBulletSpeed()
    {
        return this.bulletSpeed;
    }


    //_______
    public float getPlayerFleeBuffer()
    {
        return this.playerFleeBuffer;
    }
    public float getPlayerFleeDistance()
    {
        return this.playerFleeDistance;
    }
    public float getPlayerSeekDistance()
    {
        return this.playerSeekDistance;
    }
    public float getBulletFleeDistance()
    {
        return this.bulletFleeDistance;
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
    public void setHealth(float health)
    {
        this.health= health;
    }

    public void setSpeed(float speed)
    {
        this.speed = speed;
    }

    public void setBulletDamage(float bulletDamage)
    {
        this.bulletDamage = bulletDamage;
    }

    public void setBulletSpeed(float bulletSpeed)
    {
        this.bulletSpeed = bulletSpeed;
    }


    //_______

    public void setPlayerFleeBuffer(float playerFleeBuffer)
    {
        this.playerFleeBuffer = playerFleeBuffer;
    }

    public void setPlayerFleeDistance(float playerFleeDistance)
    {
        this.playerFleeDistance = playerFleeDistance;
    }

    public void setPlayerSeekDistance(float playerSeekDistance)
    {
        this.playerSeekDistance = playerSeekDistance;
    }

    public void setBulletFleeDistance(float bulletFleeDistance)
    {
        this.bulletFleeDistance = bulletFleeDistance;
    }

 
}


