using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;


public class EnemyBrain : MonoBehaviour {


	public StateMachine<EnemyBrain> stateMachine;
	
	public GameObject player = null;
	public GameObject bulletPreFab;

	public float playerSeekDistance;
	public float playerFleeDistance;
	public float bulletFleeDistance;
	public float playerFleeBuffer;
	public float fireSpeed;
	public float bulletSpeed;

	public float speed;
	protected static float NEARBY = 5f;
	protected static System.Random random = new System.Random();

	private Vector3 target;
	private Vector3 currentVelocity = new Vector3(0,0,0);
	private Vector3 currentAccel = new Vector3(0,0,0);
	private float timeLastFired = 0;

	private float maxTurn = 2f;
	private float breakingForce = 0.1f;

	List<GameObject> bullets = new List<GameObject>();

	void Start () {
		stateMachine = new StateMachine<EnemyBrain> (this);
		stateMachine.init(new Roaming ());
	}
		
	void Update () {}
		
	void FixedUpdate() {
		stateMachine.update ();
	}

	public void pickRandomRoamingTarget(){
		target = new Vector3 ((random.Next (80))-40, 0, (random.Next (80))-40);
	}

	public void seekTarget(){
		Vector3 desiredVelocity = target - transform.position;

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

	public void checkDecel()
	{
		Vector3 dirFromAtoB = (transform.position - target).normalized;
		float dotProd = Vector3.Dot(dirFromAtoB, transform.forward);

		if (dotProd > 0.95) {
			currentAccel *= 1-breakingForce;
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
				bullet.GetComponent<Rigidbody> ().velocity = currentVelocity * bulletSpeed;
				bullets.Add (bullet);

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
		

}