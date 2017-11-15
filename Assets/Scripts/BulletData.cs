using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData : MonoBehaviour {

	public float damage = 0f;
	public string parentShip = "";
	public GameObject parent;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void updateParentDamageDealt()
	{
		if(parent!= null)
        parent.GetComponent<EnemyBrain>().updateDamageDealt(damage);
	}

	public void explode()
	{
		var exp = GetComponent<ParticleSystem>();
		exp.Play();
	}

}
