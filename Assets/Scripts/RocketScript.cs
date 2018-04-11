using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {
	float timer;
	public GameObject target;
	public Vector3 targetLoc;
	int maxSpeed = 500;
	Vector3 currentVelocity;
	int maxTurn = 20;

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
		var th = transform.GetChild (0).GetChild (0).GetComponent<ParticleSystem>();
		th.Stop();
	}

	public void roamToTarget()
	{
		if(target != null) targetLoc = target.transform.position;
		Vector3 desiredVelocity = targetLoc - transform.position;

		desiredVelocity *= maxSpeed;

		Vector3 steering = desiredVelocity - currentVelocity;
		steering = Vector3.ClampMagnitude(steering, maxTurn);

		currentVelocity += steering;
		currentVelocity = Vector3.ClampMagnitude(currentVelocity, maxSpeed);
		transform.position += currentVelocity * Time.fixedDeltaTime;
		if (currentVelocity.magnitude != 0) transform.rotation = Quaternion.LookRotation(-currentVelocity);
	}

}
