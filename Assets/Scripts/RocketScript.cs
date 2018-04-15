using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {
	float timer;
	public GameObject target;
	int maxSpeed = 400;
	public Vector3 currentVelocity;
	int maxTurn =  75;
	int arriveDampingDistance = 200;

	// Use this for initialization
	void Start () {
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		roamToTarget ();
		timer += Time.deltaTime;
		if (timer > 2.5)
			explode();
	}

	public void explode() {

		var exp = transform.GetChild (1).GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);

		Collider[] others = Physics.OverlapSphere (transform.position, 300);

		foreach (Collider obj in  others)
		{
			var brain = obj.GetComponent<EnemyBrain>();
			if (brain != null)
			{
				float damage = 300 - Vector3.Distance (transform.position, obj.transform.position);

				brain.health -= damage;
			}
		}
	}

	public void roamToTarget()
	{
		Vector3 targetLoc; 
		if (target != null)
			targetLoc = target.transform.position;
		else
			targetLoc = transform.position -transform.forward * 1000;

		float arriveDistance = Vector3.Distance(targetLoc, transform.position);

		Vector3 desiredVelocity = targetLoc - transform.position;
		if (arriveDistance < arriveDampingDistance)
		{
			float mappedSpeed = map(arriveDistance, 0, arriveDistance, 0, maxSpeed);
			desiredVelocity *= mappedSpeed;
		} else desiredVelocity *= maxSpeed;

		Vector3 steering = desiredVelocity - currentVelocity;
		steering = Vector3.ClampMagnitude(steering, maxTurn);

		currentVelocity += steering;
		currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
		transform.position += currentVelocity * Time.fixedDeltaTime;
		if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
	}

	float map(float s, float a1, float a2, float b1, float b2)
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}

}
