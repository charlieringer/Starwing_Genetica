using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class EnemyBrain : MonoBehaviour {

	private float[] gene = new float[7];

	public float health;

	public StateMachine<EnemyBrain> stateMachine;
	
	public GameObject player = null;
	public GameObject bulletPreFab;
	public GameObject pauseManager = null;
    public GameObject ShieldPowerup;
    public GameObject SpeedPowerup;
    public GameObject WeaponPowerup;


    public float playerSeekDistance;
	public float playerFleeDistance;
	public float playerFleeBuffer;
	public float fireSpeed;
	public float bulletSpeed;
	public float bulletDamage;
	public float enemiesAvoidDistance;
	public float arriveDampingOffset;
	public float arriveDampingDistance;
	public float playerPathPredictionAmount;
	public float damageDealt = 0f;
	public float decel;

	public float maxSpeed;
	public float maxTurn;
	protected static float NEARBY = 50f;
	protected static System.Random random = new System.Random();

	private Vector3 target;
	public Vector3 currentVelocity = new Vector3(0,0,0);
	private float timeLastFired = 0;

	public List<GameObject> otherEnemies;

	public float timeAliveTimer;
	private bool timerStarted;

    private bool hasTriggeredDrop = false;

	private int activeModel;

    void Awake () {
		stateMachine = new StateMachine<EnemyBrain> (this);
		stateMachine.init(new Roaming ());


    }
		
	void Update () {
		
	}
		
	void FixedUpdate() {


        if (pauseManager && pauseManager.GetComponent<PauseHandler>().isPaused)
			return;
		if (health <= 0) 
        {
			transform.Rotate (new Vector3 (random.Next (360), random.Next (360), random.Next (360)) * Time.deltaTime);
            if (!hasTriggeredDrop)
            {
                hasTriggeredDrop = true;
                BoosterDrop(gene);
            }
			return;
		}
		stateMachine.update ();
		if (timerStarted && health > 0)
			timeAliveTimer += Time.fixedDeltaTime;
	}

    public void pickRandomRoamingTarget()
    {
		target = new Vector3((int)player.transform.position.x + (random.Next(2000)) - 1000, (int)player.transform.position.y + (random.Next(2000)) - 1000, (int)player.transform.position.z + (random.Next(2000)) - 1000);
    }

    public void roamToTarget()
    {

        Vector3 desiredVelocity = target - transform.position;

        float arriveDistance = Vector3.Distance(target, transform.position);

        if (arriveDistance < arriveDampingDistance)
        {
            float mappedSpeed = map(arriveDistance + arriveDampingOffset, 0, arriveDampingDistance, 0, maxSpeed);
            desiredVelocity *= mappedSpeed;
        }
        else desiredVelocity *= maxSpeed;

        Vector3 steering = desiredVelocity - currentVelocity;
        
		steering += avoidOthers();
		steering += avoidPlayer();
		steering += avoidMeteors ();
		steering = Vector3.ClampMagnitude(steering, maxTurn);


        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        //currentVelocity.y = 0f;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        //transform.position.y = 0f;
        if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    public void seekTarget()
    {

		Vector3 targetPrediction = target.normalized * player.GetComponent<Rigidbody>().velocity.magnitude * playerPathPredictionAmount * Time.fixedDeltaTime;
        Vector3 desiredVelocity = (target + targetPrediction) - transform.position;

        float arriveDistance = Vector3.Distance(target, transform.position);

        if (arriveDistance < arriveDampingDistance)
        {
            float mappedSpeed = map(arriveDistance + arriveDampingOffset, 0, arriveDampingDistance, 0, maxSpeed);
            desiredVelocity *= mappedSpeed;
        }
        else desiredVelocity *= maxSpeed;

        Vector3 steering = desiredVelocity - currentVelocity;
        steering += avoidOthers();
		steering += avoidPlayer ();
		steering += avoidMeteors ();
		steering = Vector3.ClampMagnitude(steering, maxTurn);

        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        //currentVelocity.y = 0f;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public void fleeTarget()
    {
        Vector3 desiredVelocity = transform.position - target;

        desiredVelocity.Normalize();
        desiredVelocity *= maxSpeed;

        Vector3 steering = desiredVelocity - currentVelocity;

        
        steering += avoidOthers();
		steering += avoidPlayer();
		steering += avoidMeteors();
		steering = Vector3.ClampMagnitude(steering, maxTurn);

        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        //currentVelocity.y = 0f;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    public Vector3 avoidOthers()
    {
        Vector3 ahead = transform.position + currentVelocity.normalized * enemiesAvoidDistance;
        Vector3 ahead2 = (transform.position + currentVelocity.normalized * enemiesAvoidDistance) * 0.5f;
        Vector3 totAvoidForce = new Vector3();

        for (int i = 0; i < otherEnemies.Count; i++)
        {
            Vector3 bounds = otherEnemies[i].transform.GetChild(0).gameObject.GetComponent<Renderer>().bounds.size;
            float rad = bounds.x > bounds.z ? bounds.x * 2 : bounds.z * 2;
            Vector3 otherEnemyLocation = otherEnemies[i].transform.position;

            bool willCollide = Vector3.Distance(otherEnemyLocation, ahead) <= rad ? true : Vector3.Distance(otherEnemyLocation, ahead2) <= rad;
            if (Vector3.Distance(otherEnemyLocation, transform.position) <= rad) willCollide = true;

            if (willCollide)
            {
                Vector3 avoidForce = ahead - otherEnemyLocation;
                //avoidForce.y = 0.0f;
                totAvoidForce += avoidForce;
            }
        }
        totAvoidForce = Vector3.ClampMagnitude(totAvoidForce, maxTurn);
        return totAvoidForce;
    }

	public Vector3 avoidMeteors()
	{
		Vector3 ahead = transform.position + currentVelocity.normalized * enemiesAvoidDistance;
		Vector3 ahead2 = (transform.position + currentVelocity.normalized * enemiesAvoidDistance) * 0.5f;
		Vector3 totAvoidForce = new Vector3();

		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Meteor"))
		{
			Vector3 bounds = go.gameObject.transform.localScale;
			float rad = bounds.x;
			Vector3 meteorLoc = go.transform.position;

			bool willCollide = Vector3.Distance(meteorLoc, ahead) <= rad ? true : Vector3.Distance(meteorLoc, ahead2) <= rad;
			if (Vector3.Distance(meteorLoc, transform.position) <= rad) willCollide = true;

			if (willCollide)
			{
				Vector3 avoidForce = ahead - meteorLoc;
				//avoidForce.y = 0.0f;
				totAvoidForce += avoidForce;
			}
		}
		totAvoidForce = Vector3.ClampMagnitude(totAvoidForce, maxTurn);
		return totAvoidForce;
	}

	public Vector3 avoidPlayer()
	{
		Vector3 ahead = transform.position + currentVelocity.normalized * enemiesAvoidDistance;
		Vector3 ahead2 = (transform.position + currentVelocity.normalized * enemiesAvoidDistance) * 0.5f;
		Vector3 totAvoidForce = new Vector3();

		Vector3 bounds = player.transform.localScale;
		float rad = bounds.x * playerFleeDistance/2 ;
		Vector3 playerLoc = player.transform.position;

		bool willCollide = Vector3.Distance(playerLoc, ahead) <= rad ? true : Vector3.Distance(playerLoc, ahead2) <= rad;
		if (Vector3.Distance(playerLoc, transform.position) <= rad) willCollide = true;

		if (willCollide)
		{
			Vector3 avoidForce = ahead - playerLoc;
			//avoidForce.y = 0.0f;
			totAvoidForce += avoidForce;
		}
		totAvoidForce = Vector3.ClampMagnitude(totAvoidForce, maxTurn);
		return totAvoidForce;
	}

    public void fire()
    {
        if (Time.time > timeLastFired + fireSpeed)
        {
			//Vector3 dirFromAtoB = (transform.position - (target + (target.normalized * player.GetComponent<Rigidbody>().velocity.magnitude * playerPathPredictionAmount * Time.fixedDeltaTime))).normalized;
			Vector3 dirFromAtoB = (transform.position - target);
			float dotProd = Vector3.Dot(dirFromAtoB.normalized, transform.forward.normalized);

            if (dotProd > 0.95)
            {
                timeLastFired = Time.time;
				Quaternion noise = Quaternion.Euler(Random.Range(-2f, 2f), Random.Range(-2f, 2f), Random.Range(-2f, 2f));
				Quaternion noiseyrotation = transform.rotation * noise;
				GameObject bullet = Instantiate (bulletPreFab, transform.position, noiseyrotation);
				bullet.transform.localScale = new Vector3(ValueRemapping(bulletDamage, 10, 0.4f), ValueRemapping(bulletDamage, 10, 0.4f), ValueRemapping(bulletDamage, 10, 0.4f));

				bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * -bulletSpeed;
                bullet.GetComponent<BulletData>().damage = bulletDamage;
                bullet.GetComponent<BulletData>().parentShip = "Enemy";
                bullet.GetComponent<BulletData>().parent = this.gameObject;
				Destroy(bullet, 2000f/bulletSpeed);
            }
        }
    }

    public void checkPlayerSeekProximity()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, target) < playerSeekDistance)
        {
            stateMachine.changeState(new Seeking());
        }
    }
		

    public void checkRoamingLocationProximity()
    {
        if (Vector3.Distance(transform.position, target) < NEARBY)
        {
            pickRandomRoamingTarget();
        }
    }

    public void checkSeekOrRoamProximity()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, target) > playerSeekDistance)
        {
            stateMachine.changeState(new Roaming());
        }

    }

    public bool checkPlayerAvoidProximity()
    {
        if (player == null) return false;
        //if (Vector3.Distance(transform.position, target) < playerFleeDistance)
		if (Vector3.Distance(transform.position, target) < 200)
        {
            stateMachine.changeState(new FleeingPlayer());
			return true;
        }
		return false;
    }

    public void checkPlayerFleeBuffer()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, target) > playerFleeBuffer+10) stateMachine.changeState(new Roaming());
    }

    public void setPlayerAsTarget()
    {
        if (player == null) return;
        target = player.transform.position;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.Contains("shot_prefab") && collision.gameObject.GetComponent<BulletData>().parentShip != "Enemy")
        {
            float damage = collision.gameObject.GetComponent<BulletData>().damage;
			collision.gameObject.GetComponent<BulletData> ().explode ();
            health -= damage;
        }

		if (collision.gameObject.name.Contains("Rocket("))
		{
			collision.gameObject.GetComponent<RocketScript> ().explode ();
		}

		if(collision.gameObject.name.Contains("Meteor"))
		{
			health = 0;
		}
    }

    public void setGenoPheno(float[] _genes)
    {
        gene = _genes;

        health = (gene[0] * 20) + 40 ;
        maxSpeed = gene[1] * 50 + 20;
        bulletSpeed = gene[2] * 600 + 240;
        bulletDamage = 10 - gene[2];

		if (bulletDamage < 0)
			bulletDamage = 0;
		bulletDamage+=2;

        playerSeekDistance = gene[3] * 160 + 80;
        playerFleeDistance = gene[4] * 10 + 10;
        playerFleeBuffer = playerFleeDistance + 400;
        enemiesAvoidDistance = gene[5] * 16;


		if (gene [0] > gene [1] && gene [0] > gene [2])
			activeModel = 0;
		else if (gene [1] > gene [2])
			activeModel = 1;
		else
			activeModel = 2;
		transform.GetChild (activeModel).gameObject.SetActive (true); 
		transform.GetChild (activeModel).GetChild (0).GetComponent<MiniMapShipController> ().target = player;

		float scalingValueIncrement = ValueRemapping(gene[0], 9, 2); // the 0-9 value will be remapped to 0-1 value. this will be used to update the scale values.
		//scale the Thrusters
		for (int childIndex=1; childIndex < 3; childIndex++)
		{
			transform.GetChild (activeModel).GetChild(childIndex).transform.localScale =
				new Vector3(0.3f + scalingValueIncrement*0.5f, 0.3f + scalingValueIncrement*0.5f, 0.3f + scalingValueIncrement*0.5f);
		}
		//change the ship
		//todo: This is broken becuase of sub models (I think)
		transform.localScale = new Vector3(1f + scalingValueIncrement, 1f + scalingValueIncrement, 1f + scalingValueIncrement);
    }

    public void BoosterDrop(float[] _genes)
    {
        // get the genes
        gene = _genes;
        // p for geneBooster
        var pBooster = random.NextDouble();
        // p for shieldBooster
        var pShield = random.NextDouble();

        List<float> booster = new List< float > ();
        int BoosterType;
        float BoosterAmmount;

        // consider only the first 3 genes 
        for (int i = 0; i < 3; i++)
        {
            booster.Add(gene[i]);
        }
        // select the gene with the highest value
        // [0] health - [1] speed - [2] damage
       
        BoosterType = booster.IndexOf(booster.Max());
        BoosterAmmount = booster.Max();

        // chek if booosters are droped
        if (pBooster <= 0.1)
        {
            if (BoosterType == 0) {
				GameObject boosterDrop = Instantiate (ShieldPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				//boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			}
			else if (BoosterType == 1) {
				GameObject boosterDrop = Instantiate (SpeedPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				//boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			} else {
				GameObject boosterDrop = Instantiate (WeaponPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				//boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			}


        }

        else if (pBooster >= 0.9)
        {
            BoosterType = Random.Range(0, 3);
            BoosterAmmount = booster[BoosterType];

            if (BoosterType == 0)
            {
                GameObject boosterDrop = Instantiate(ShieldPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
                boosterDrop.GetComponent<Booster>().boostAmount = BoosterAmmount;
                //boosterDrop.GetComponent<Rigidbody>().velocity = Vector3.zero;
                boosterDrop.transform.parent = null;
            }
            else if (BoosterType == 1)
            {
                GameObject boosterDrop = Instantiate(SpeedPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
                boosterDrop.GetComponent<Booster>().boostAmount = BoosterAmmount;
                //boosterDrop.GetComponent<Rigidbody>().velocity = Vector3.zero;
                boosterDrop.transform.parent = null;
            }
            else
            {
                GameObject boosterDrop = Instantiate(WeaponPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
                boosterDrop.GetComponent<Booster>().boostAmount = BoosterAmmount;
                //boosterDrop.GetComponent<Rigidbody>().velocity = Vector3.zero;
                boosterDrop.transform.parent = null;
            }

        }

        else
        {
            if (pShield <= 0.1)
            {
				GameObject boosterDrop = Instantiate (ShieldPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = gene[0];
				//boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
            }  

        }
        
        }


    public float[] GetGene()
    {
		return gene;
	}

    public void updateDamageDealt(float _damage)
 	{
	damageDealt += _damage;
    }


    private static float ValueRemapping(float initialVal, float initialHigh,  float targetHigh)
    {
		if(initialVal > initialHigh) return targetHigh;
        return ((initialVal*targetHigh)/initialHigh);
    }

	public void startSeekTimer ()
	{
		timerStarted = true;
	}

}
