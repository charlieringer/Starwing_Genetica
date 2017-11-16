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
	protected static float NEARBY = 5f;
	protected static System.Random random = new System.Random();

	private Vector3 target;
	private Vector3 currentVelocity = new Vector3(0,0,0);
	private float timeLastFired = 0;

	public List<GameObject> otherEnemies;

	List<GameObject> bullets = new List<GameObject>();

	public float timeAliveTimer;
	private bool timerStarted;

    private bool hasTriggeredDrop = false;


    void Awake () {
		stateMachine = new StateMachine<EnemyBrain> (this);
		stateMachine.init(new Roaming ());
	}
		
	void Update () {
		
	}
		
	void FixedUpdate() {
		//changed colour based on gene (speed and bullet speed) information
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(ValueRemapping(gene[1], 9, 225)/225, 20/225, 20/225, 225/225);
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().materials[3].color = new Color(225/225, ValueRemapping(gene[2], 9, 225)/225, 0, 225/225);

        //change size based on the health value from the gene
        float scalingValueIncrement = ValueRemapping(gene[0], 9, 3); // the 0-9 value will be remapped to 0-1 value. this will be used to update the scale values.
        transform.localScale = new Vector3(0.5f+scalingValueIncrement, 0.5f+scalingValueIncrement, 0.5f+scalingValueIncrement);


        //transform.GetChild(0).gameObject.GetComponent<Transform>().ro
        //    .localScale = new Vector3(1+scalingValueIncrement, 1+scalingValueIncrement, 1+scalingValueIncrement);

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
        target = new Vector3((int)player.transform.position.x + (random.Next(80)) - 40, 0, (int)player.transform.position.z + (random.Next(80)) - 40);
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
        steering = Vector3.ClampMagnitude(steering, maxTurn);
        steering += avoidOthers();

        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        currentVelocity.y = 0f;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        //transform.position.y = 0f;
        if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    public void seekTarget()
    {

        Vector3 targetPrediction = target.normalized * player.GetComponent<PlayerControls>().currentSpeed * playerPathPredictionAmount * Time.fixedDeltaTime;
        Vector3 desiredVelocity = (target + targetPrediction) - transform.position;

        float arriveDistance = Vector3.Distance(target, transform.position);

        if (arriveDistance < arriveDampingDistance)
        {
            float mappedSpeed = map(arriveDistance + arriveDampingOffset, 0, arriveDampingDistance, 0, maxSpeed);
            desiredVelocity *= mappedSpeed;
        }
        else desiredVelocity *= maxSpeed;

        Vector3 steering = desiredVelocity - currentVelocity;
        steering = Vector3.ClampMagnitude(steering, maxTurn);
        steering += avoidOthers();

        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        currentVelocity.y = 0f;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        //transform.position.y = 0f;
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

        steering = Vector3.ClampMagnitude(steering, maxTurn);
        steering += avoidOthers();

        currentVelocity += steering;
        currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
        currentVelocity *= 1 - decel;
        currentVelocity.y = 0f;
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
                avoidForce.y = 0.0f;
                totAvoidForce += avoidForce;
            }
        }
        totAvoidForce = Vector3.ClampMagnitude(totAvoidForce, maxTurn);
        return totAvoidForce;
    }

    public void fire()
    {
        if (Time.time > timeLastFired + fireSpeed)
        {
            Vector3 dirFromAtoB = (transform.position - (target + target * player.GetComponent<PlayerControls>().currentSpeed * playerPathPredictionAmount * Time.fixedDeltaTime)).normalized;
            float dotProd = Vector3.Dot(dirFromAtoB, transform.forward);

            if (dotProd > 0.97)
            {
                timeLastFired = Time.time;
                GameObject bullet = Instantiate(bulletPreFab, transform.position, transform.rotation);
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * -bulletSpeed - GetComponent<Rigidbody>().velocity;
                bullet.GetComponent<BulletData>().damage = bulletDamage;
                bullet.GetComponent<BulletData>().parentShip = "Enemy";
                bullet.GetComponent<BulletData>().parent = this.gameObject;
                bullets.Add(bullet);
                Destroy(bullet, 2.0f);
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

    public void checkPlayerAvoidProximity()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, target) < playerFleeDistance)
        {
            stateMachine.changeState(new FleeingPlayer());
        }
    }

    public void checkPlayerFleeBuffer()
    {
        if (player == null) return;
        if (Vector3.Distance(transform.position, target) > playerFleeBuffer) stateMachine.changeState(new Roaming());
    }

    public void setPlayerAsTarget()
    {
        if (player == null) return;
        target = player.transform.position;
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.Contains("Bullet") && collision.gameObject.GetComponent<BulletData>().parentShip != "Enemy")
        {
            float damage = collision.gameObject.GetComponent<BulletData>().damage;
			collision.gameObject.GetComponent<BulletData> ().explode ();
            health -= damage;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.name == "Enemy(Clone)")
        {
            //health = 0;
            //collision.gameObject.GetComponent<EnemyBrain>().health = 0;
        }
    }

    public void setGenoPheno(float[] _genes)
    {
        gene = _genes;

        health = (gene[0] * 30) + 40 ;
        maxSpeed = gene[1] * 30;
        bulletSpeed = gene[2] * 75;
        bulletDamage = 10 - gene[2];

		if (bulletDamage < 0)
			bulletDamage = 0;
		bulletDamage+=2;

        playerSeekDistance = gene[3] * 40;
        playerFleeDistance = gene[4] * 40;
        playerFleeBuffer = playerFleeDistance + 60;
        enemiesAvoidDistance = gene[5] * 16;
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
        if (pBooster <= 0.2)
        {
			if (BoosterType == 0) {
				GameObject boosterDrop = Instantiate (ShieldPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			}
			else if (BoosterType == 1) {
				GameObject boosterDrop = Instantiate (SpeedPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			} else {
				GameObject boosterDrop = Instantiate (WeaponPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = BoosterAmmount;
				boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				boosterDrop.transform.parent = null;
			}

        }
        else
        {
            if (pShield <= 0.2)
            {
				GameObject boosterDrop = Instantiate (ShieldPowerup, transform.position + new Vector3(0, 5, 0), transform.rotation);
				boosterDrop.GetComponent<Booster> ().boostAmount = gene[0];
				boosterDrop.GetComponent<Rigidbody> ().velocity = Vector3.zero;
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
        return ((initialVal*targetHigh)/initialHigh);
    }

	public void startSeekTimer ()
	{
		timerStarted = true;
	}

}
