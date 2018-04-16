using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;


public class EnemyBrain : MonoBehaviour {

	private float[] genes = new float[5];

	public float health;

	public StateMachine<EnemyBrain> stateMachine;
	
	public GameObject player = null;
	public GameObject bulletPreFab;
	public GameObject pauseManager = null;

    public float playerSeekDistance;
	public float playerFleeDistance;
	public float fireSpeed;
	public float bulletSpeed;
	public float bulletDamage;
	public float avoidDistance;
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
	public Vector3 currentVelocity = new Vector3(0,0,0);
	private float timeLastFired = 0;

	public List<GameObject> otherEnemies;

	public float timeAliveTimer;
	private bool timeAliveTimerStarted;

	private int activeModel;

    void Awake () {
		stateMachine = new StateMachine<EnemyBrain> (this);
		stateMachine.init(new Roaming ());
	}
		
	void FixedUpdate() {
		stateMachine.update ();
		if (timeAliveTimerStarted && health > 0) timeAliveTimer += Time.deltaTime;
	}

    public void pickRandomRoamingTarget()
    {
		Vector3 playerLoc = player.transform.position;
		target = new Vector3(playerLoc.x + (random.Next(2000)) - 1000, playerLoc.y + (random.Next(2000)) - 1000, playerLoc.z + (random.Next(2000)) - 1000);
    }

    public void roamToTarget()
    {
        Vector3 desiredVelocity = target - transform.position;
        float arriveDistance = Vector3.Distance(target, transform.position);

        if (arriveDistance < arriveDampingDistance)
        {
			float mappedSpeed = map(arriveDistance, 0, arriveDistance, 0, maxSpeed);
            desiredVelocity *= mappedSpeed;
        } else desiredVelocity *= maxSpeed;

        Vector3 steering = desiredVelocity - currentVelocity;
        
		steering += avoidOthers();
		steering += avoidPlayer();
		steering += avoidMeteors ();
		steering = Vector3.ClampMagnitude(steering, maxTurn);

        currentVelocity += steering;
        currentVelocity *= 1 - decel;
        transform.position += currentVelocity * Time.fixedDeltaTime;
        if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    public void seekTarget()
    {
		Vector3 targetPrediction = player.transform.forward.normalized * player.GetComponent<Rigidbody>().velocity.magnitude * playerPathPredictionAmount * Time.fixedDeltaTime;
		Vector3 desiredVelocity = (target + targetPrediction) - transform.position;
		float arriveDistance = Vector3.Distance(target, transform.position);

		if (arriveDistance < arriveDampingDistance)
		{
			float mappedSpeed = map(arriveDistance, 0, arriveDistance, 0, maxSpeed);
			desiredVelocity *= mappedSpeed;
		} else desiredVelocity *= maxSpeed;

		Vector3 steering = desiredVelocity - currentVelocity;

		steering += avoidOthers();
		steering += avoidPlayer();
		steering += avoidMeteors ();
		steering = Vector3.ClampMagnitude(steering, maxTurn);

		currentVelocity += steering;
		currentVelocity *= 1 - decel;
		transform.position += currentVelocity * Time.fixedDeltaTime;
		if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public void fleeTarget()
    {
		Vector3 desiredVelocity = target - transform.position;
		float arriveDistance = Vector3.Distance(target, transform.position);

		if (arriveDistance < arriveDampingDistance)
		{
			float mappedSpeed = map(arriveDistance, 0, arriveDistance, 0, maxSpeed);
			desiredVelocity *= mappedSpeed;
		} else desiredVelocity *= maxSpeed;

		Vector3 steering = desiredVelocity - currentVelocity;

		steering += avoidOthers();
		steering += avoidPlayer();
		steering += avoidMeteors ();
		steering = Vector3.ClampMagnitude(steering, maxTurn);

		currentVelocity += steering;
		currentVelocity *= 1 - decel;
		transform.position += currentVelocity * Time.fixedDeltaTime;
		if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
	}

    public Vector3 avoidOthers()
    {
        Vector3 ahead = transform.position + currentVelocity.normalized * avoidDistance;
		Vector3 ahead2 = ahead * 0.5f;
        Vector3 totAvoidForce = new Vector3();

        for (int i = 0; i < otherEnemies.Count; i++)
        {
			float rad = otherEnemies[i].gameObject.transform.localScale.x;
            Vector3 otherEnemyLocation = otherEnemies[i].transform.position;

            bool willCollide = Vector3.Distance(otherEnemyLocation, ahead) <= rad ? true : Vector3.Distance(otherEnemyLocation, ahead2) <= rad;
            if (Vector3.Distance(otherEnemyLocation, transform.position) <= rad) willCollide = true;

            if (willCollide)
            {
                Vector3 avoidForce = ahead - otherEnemyLocation;
                totAvoidForce += avoidForce;
            }
        }
        return totAvoidForce;
    }

	public Vector3 avoidMeteors()
	{
		Vector3 ahead = transform.position + currentVelocity.normalized * avoidDistance;
		Vector3 ahead2 = ahead * 0.5f;
		Vector3 totAvoidForce = new Vector3();

		foreach(GameObject go in GameObject.FindGameObjectsWithTag("Meteor"))
		{
			float rad = go.gameObject.transform.localScale.x;
			Vector3 meteorLoc = go.transform.position;

			bool willCollide = Vector3.Distance(meteorLoc, ahead) <= rad ? true : Vector3.Distance(meteorLoc, ahead2) <= rad;
			if (Vector3.Distance(meteorLoc, transform.position) <= rad) willCollide = true;

			if (willCollide)
			{
				Vector3 avoidForce = ahead - meteorLoc;
				totAvoidForce += avoidForce;
			}
		}
		return totAvoidForce;
	}

	public Vector3 avoidPlayer()
	{
		Vector3 ahead = transform.position + currentVelocity.normalized * avoidDistance;
		Vector3 ahead2 = ahead * 0.5f;
		Vector3 totAvoidForce = new Vector3();

		float rad = player.transform.localScale.x * playerFleeDistance/2;
		Vector3 playerLoc = player.transform.position;

		bool willCollide = Vector3.Distance(playerLoc, ahead) <= rad ? true : Vector3.Distance(playerLoc, ahead2) <= rad;
		if (Vector3.Distance(playerLoc, transform.position) <= rad) willCollide = true;

		if (willCollide)
		{
			Vector3 avoidForce = ahead - playerLoc;
			totAvoidForce += avoidForce;
		}
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
				bullet.transform.localScale = new Vector3(map(bulletDamage, 10, 0.5f), map(bulletDamage, 10, 0.5f), map(bulletDamage, 10, 0.3f));

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
        if (Vector3.Distance(transform.position, player.transform.position) < playerSeekDistance)
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

	public void checkFleeLocationProximity()
	{
		if (Vector3.Distance(transform.position, target) < NEARBY)
		{
			stateMachine.changeState(new Roaming());
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
        if (Vector3.Distance(transform.position, target) < playerFleeDistance)
        {
            stateMachine.changeState(new FleeingPlayer());
			return true;
        }
		return false;
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
		genes = _genes;

        health = (genes[0] * 10) + 70 ;//70-170
		maxSpeed = (10-genes[0]) * 25 + 100;//100-350
        bulletSpeed = genes[1] * 50 + 500;//500-1000
		bulletDamage = (10 - genes[1]) + 5;//5-15

//        playerSeekDistance = genes[2] * 160 + 80;
//		playerFleeDistance = genes[3] * 10 + 100;
//        avoidDistance = genes[4] * 16;

		playerSeekDistance = 5 * 160 + 80;
		playerFleeDistance = 5 * 10 + 100;
		avoidDistance = 5 * 16;


		//if (genes [0] > genes [1] && genes [0] > genes [2])
		if (genes [0] > genes [1])
			activeModel = 0;
		//else if (genes [1] > genes [2])
		//	activeModel = 1;
		else
			activeModel = 2;
		transform.GetChild (activeModel).gameObject.SetActive (true); 
		transform.GetChild (activeModel).GetChild (0).GetComponent<MiniMapShipController> ().target = player;

		float scalingValueIncrement = map(genes[0], 9, 2); // the 0-9 value will be remapped to 0-1 value. this will be used to update the scale values.
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

    

    public float[] getGenes()
    {
		return genes;
	}

    public void updateDamageDealt(float _damage)
 	{
	damageDealt += _damage;
    }


    private static float map(float initialVal, float initialHigh,  float targetHigh)
    {
		if(initialVal > initialHigh) return targetHigh;
        return ((initialVal*targetHigh)/initialHigh);
    }

	public void startSeekTimer ()
	{
		timeAliveTimerStarted = true;
	}

}
