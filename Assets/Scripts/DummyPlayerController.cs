using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour {
	public float health;
	public float speed;
	public float damage;

	public float randRotX;
	public float randRotY;
	public float randRotZ;



	// Use this for initialization
	void Start () {
		randRotX = Random.RandomRange (-40, 40);
		randRotY = Random.RandomRange (-40, 40);
		randRotZ = Random.RandomRange (-40, 40);

		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Rotate(randRotX*Time.deltaTime,randRotY*Time.deltaTime, randRotZ*Time.deltaTime);


	}
}
