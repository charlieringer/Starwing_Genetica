using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    
    public float topSpeed;
	public float decel;
	public float accel;
    public float maxTurn;

    Rigidbody rigidBody;

	private float currentSpeed = 0;
	Vector3 current = new Vector3(0,0,0);
	
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

		currentSpeed += v * Time.fixedDeltaTime;
		currentSpeed -= decel * Time.fixedDeltaTime;
		if (currentSpeed > topSpeed)currentSpeed = topSpeed;
		if(currentSpeed < 0) currentSpeed = 0;

		if (currentSpeed > 0) thrust (currentSpeed);
		turn(h);
	}


	private void thrust(float amount)
	{
		transform.position += transform.forward * topSpeed * Time.fixedDeltaTime * -amount * accel;

	}

	private void turn(float amount)
	{
		float yaw = amount * maxTurn * Time.fixedDeltaTime;
		float roll = amount * maxTurn * Time.fixedDeltaTime * 1 ;
		transform.Rotate (0, yaw, 0);
		Vector3 fixedY = transform.position;
		fixedY.y = 0.0f;
		transform.position = fixedY;
	}
}
