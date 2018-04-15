using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorController : MonoBehaviour {

	public float randRotX;
	public float randRotY;
	public float randRotZ;

	public GameObject meteorPrefab;

	public float health = 0;
	private Rigidbody rb;
	// Use this for initialization
	void Awake () {
		randRotX = Random.Range (-40, 40);
		randRotY = Random.Range (-40, 40);
		randRotZ = Random.Range (-40, 40);
		rb = GetComponent<Rigidbody> ();
		health = transform.localScale.x * 500;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(randRotX*Time.deltaTime,randRotY*Time.deltaTime, randRotZ*Time.deltaTime);
		rb.velocity = new Vector3 (randRotX, randRotY, randRotZ);

		if (health <= 0) {
			if (transform.localScale.x > 0.25) {
				float newScale = transform.localScale.x/2;
				GameObject childA = Instantiate (meteorPrefab, transform.position, transform.rotation, null);
				GameObject childB = Instantiate (meteorPrefab, transform.position, transform.rotation, null);
				childA.transform.localScale = new Vector3 (newScale, newScale, newScale);
				childB.transform.localScale = new Vector3 (newScale, newScale, newScale);
				childA.GetComponent<MeteorController>().meteorPrefab = meteorPrefab;
				childB.GetComponent<MeteorController>().meteorPrefab = meteorPrefab;
				childA.SetActive (true);
				childB.SetActive (true);
			}
			gameObject.SetActive(false);
		}		
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
			health -= 300;
		}
	}

}
