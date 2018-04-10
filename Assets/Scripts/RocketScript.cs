using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour {
	float timer;

	// Use this for initialization
	void Start () {
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		timer += Time.deltaTime;
		if (timer > 1.5)
			explode();
	}

	public void explode() {

		var exp = transform.GetChild (1).GetComponent<ParticleSystem>();
		exp.Play();
		Destroy(gameObject, exp.duration);

		Collider[] others = Physics.OverlapSphere (transform.position, 150);

		foreach (Collider obj in  others)
		{
			var brain = obj.GetComponent<EnemyBrain>();
			if (brain != null)
			{
				float damage = 150 - Vector3.Distance (transform.position, obj.transform.position);

				brain.health -= damage*2;
			}
		}
		var th = transform.GetChild (0).GetChild (0).GetComponent<ParticleSystem>();
		th.Stop();
	}

}
