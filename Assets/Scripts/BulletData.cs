using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletData : MonoBehaviour {

	public float damage = 0f;
	public string parentShip = "";
	public GameObject parent;
	public GameObject sparksPrefab;
	public GameObject laserPiecesPrefab;

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
		GameObject sparks = Instantiate (sparksPrefab, transform.position, transform.rotation);
		var sparksParticles = sparks.GetComponent<ParticleSystem>();
		sparksParticles.Play();
		Destroy (sparks, 0.5f);

		GameObject laser = Instantiate (laserPiecesPrefab, transform.position, transform.rotation);
		var laserParticles = sparks.GetComponent<ParticleSystem>();
		laserParticles.Play();
		Destroy (laser, 0.5f);
	}

}
