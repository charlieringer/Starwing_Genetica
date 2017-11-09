using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;


public class EnemyBrain : MonoBehaviour {

	public float health;

	public StateMachine<EnemyBrain> stateMachine;
	
	public GameObject player = null;
	public GameObject bulletPreFab;

	public float playerSeekDistance;
	public float playerFleeDistance;
	public float bulletFleeDistance;
	public float playerFleeBuffer;
	public float fireSpeed;
	public float bulletSpeed;
	public float bulletDamage;
	public float enemiesAvoidDistance;

	public float speed;
	public float maxTurn;
	protected static float NEARBY = 5f;
	protected static System.Random random = new System.Random();

	private Vector3 target;
	private Vector3 currentVelocity = new Vector3(0,0,0);
	private Vector3 currentAccel = new Vector3(0,0,0);
	private float timeLastFired = 0;

	public List<GameObject> otherEnemies;

	List<GameObject> bullets = new List<GameObject>();

	void Awake () {
		stateMachine = new StateMachine<EnemyBrain> (this);
		stateMachine.init(new Roaming ());
	}
		
	void Update () {}
		
	void FixedUpdate() {
		avoidOthers();
		stateMachine.update ();
	}

	public void pickRandomRoamingTarget(){
		target = new Vector3 ((int)player.transform.position.x+(random.Next (80))-40, 0, (int)player.transform.position.z + (random.Next (80))-40);
	}

	public void seekTarget(){
		Vector3 desiredVelocity = target - transform.position;

		desiredVelocity.Normalize ();
		desiredVelocity *= speed;

		Vector3 steering = desiredVelocity - currentVelocity;
		steering = Vector3.ClampMagnitude (steering, maxTurn);
		steering *= Time.fixedDeltaTime;
		steering.y = 0f;

		//currentAccel += steering;
		currentVelocity += steering;
		currentVelocity = Vector3.ClampMagnitude (currentVelocity, speed);

		transform.position += currentVelocity * Time.fixedDeltaTime;
		transform.rotation = Quaternion.LookRotation (-currentVelocity);
	}

	public void fleeTarget(){
		Vector3 desiredVelocity =  transform.position - target;

		desiredVelocity.Normalize ();
		desiredVelocity *= speed;


		Vector3 steering = desiredVelocity - currentVelocity;
		steering = Vector3.ClampMagnitude (steering, maxTurn);
		steering.y = 0f;

		steering *= Time.fixedDeltaTime;

		currentAccel += steering;
		currentVelocity += currentAccel;
		currentVelocity = Vector3.ClampMagnitude (currentVelocity, speed);

		transform.position += currentVelocity * Time.fixedDeltaTime;

		transform.rotation = Quaternion.LookRotation (-currentVelocity);
	}

	public void avoidOthers()
	{
		Vector3 ahead = transform.position + currentVelocity.normalized * enemiesAvoidDistance;
		Vector3 ahead2 = (transform.position + currentVelocity.normalized * enemiesAvoidDistance) * 0.5f;

		for(int i = 0; i < otherEnemies.Count; i++)
		{
			float rad = 18.0f;
			Vector3 otherEnemyLocation = otherEnemies[i].transform.position;

			bool willCollide = Vector3.Distance(otherEnemyLocation, ahead) <= rad ? true : Vector3.Distance(otherEnemyLocation, ahead2) <= rad;

			if(willCollide)
			{
				Vector3 avoidForce = ahead - otherEnemyLocation;
				avoidForce = avoidForce.normalized * maxTurn*10;
				avoidForce.y = 0.0f;
				currentVelocity += avoidForce * Time.fixedDeltaTime;
			}

		}
	}

	public void fire()
	{
		if (Time.time > timeLastFired + fireSpeed) {
			Vector3 dirFromAtoB = (transform.position - target).normalized;
			float dotProd = Vector3.Dot(dirFromAtoB, transform.forward);

			if(dotProd > 0.95) {
				timeLastFired = Time.time;
				GameObject bullet = Instantiate (bulletPreFab, transform.position, transform.rotation);				
				bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * -bulletSpeed - GetComponent<Rigidbody>().velocity;
				bullet.GetComponent<BulletLogic>().damage = bulletDamage;
				bullet.GetComponent<BulletLogic>().parentShip = "Enemy";
				bullets.Add (bullet);
				Destroy (bullet, 2.0f);
			}
		}
	}
		
	public void checkRoamingLocationProximity()
	{
		if(Vector3.Distance(transform.position, target) < NEARBY)
		{
			pickRandomRoamingTarget ();
		}
	}

	public void checkPlayerSeekProximity()
	{
		if (player == null) return;	
		if (Vector3.Distance (transform.position, target) < playerSeekDistance) {
			stateMachine.changeState (new Seeking ());
			Debug.Log ("Seeking Player");
		}

	}

	public void checkSeekOrRoamProximity()
	{
		if (player == null) return;	
		if (Vector3.Distance (transform.position, target) > playerSeekDistance) {
			stateMachine.changeState (new Roaming ());
			Debug.Log ("Roaming");
		}

	}

	public void checkPlayerAvoidProximity()
	{
		if (player == null) return;	
		if (Vector3.Distance (transform.position, target) < playerFleeDistance) {
			Debug.Log ("Fleeing Player");
			stateMachine.changeState (new FleeingPlayer ());
		}
	}

	public void checkPlayerFleeBuffer()
	{
		if (player == null) return;	
		if(Vector3.Distance(transform.position, target) > playerFleeBuffer) stateMachine.changeState(new Roaming());
	}

	public void setPlayerAsTarget()
	{
		if (player == null) return;	
		target = player.transform.position;
	}

	void OnTriggerEnter(Collider collision)
	{
		if(collision.gameObject.name.Contains("Bullet") && collision.gameObject.GetComponent<BulletLogic>().parentShip != "Enemy")
		{
			Debug.Log(collision.gameObject.GetComponent<BulletLogic>().parentShip);
			float damage = collision.gameObject.GetComponent<BulletLogic>().damage;
			health -= damage;
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.name == "Enemy(Clone)")
		{
			health = 0;
			collision.gameObject.GetComponent<EnemyBrain>().health = 0;
		}
	}
}