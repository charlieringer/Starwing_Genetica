using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class DummyAI : MonoBehaviour  {

	public float arriveDampingOffset;
	public float arriveDampingDistance;
	public float decel;

	public float maxSpeed;
	public float maxTurn;
	protected static float NEARBY = 5f;
	//protected static System.Random random = new System.Random();

	private Vector3 target;
	private Vector3 currentVelocity = new Vector3(0,0,0);


	void Awake () {
		pickRandomRoamingTarget ();
	}

	void FixedUpdate() {
		roamToTarget ();
		checkRoamingLocationProximity ();
	}

	public void pickRandomRoamingTarget()
	{
		target = new Vector3(Random.Range(-1000,2000), Random.Range(-300,300), Random.Range(0,1000));
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

		currentVelocity += steering;
		currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
		transform.position += currentVelocity * Time.deltaTime;
		if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}
		
	public void checkRoamingLocationProximity()
	{
		if (Vector3.Distance(transform.position, target) < NEARBY)
		{
			pickRandomRoamingTarget();
		}
	}
}
