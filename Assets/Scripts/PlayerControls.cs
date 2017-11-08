using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    
    public float topSpeed;
    public float maxTurn;
	[SerializeField]
    Rigidbody rigidBody;
    
    Vector3 currentVel = new Vector3(0,0,0);
    Vector3 currentAccel = new Vector3(0,0,0);
	
	void Awake()
	{
		rigidBody = GetComponent <Rigidbody>();
	}
    

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");
						 


        Vector3 movement = new Vector3(0,0, -v);
		movement.Normalize();
		currentVel += movement;
 
        rigidBody.MovePosition(currentVel);
		//transform.rotation = Quaternion.LookRotation (currentVel);
	}
}
