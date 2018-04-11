using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapShipController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 oldRotation = this.transform.rotation.eulerAngles;
		oldRotation.y = 0.0f;
		this.transform.rotation = Quaternion.Euler(oldRotation);
		
	}
}
