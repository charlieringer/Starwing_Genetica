using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 targetLoc = target.transform.position;
		this.transform.position = new Vector3 (targetLoc.x, 800 , targetLoc.z);
	}
}
