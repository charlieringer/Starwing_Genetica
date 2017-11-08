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
		
		float h = 0f;
		float v = 0f;
						 
		if(Input.GetKeyDown(KeyCode.W)||Input.GetKeyDown(KeyCode.UpArrow)) h = 10;
		if(Input.GetKeyDown(KeyCode.S)||Input.GetKeyDown(KeyCode.DownArrow)) h = -10;
		if(Input.GetKeyDown(KeyCode.A)||Input.GetKeyDown(KeyCode.LeftArrow)) v = 10;
		if(Input.GetKeyDown(KeyCode.D)||Input.GetKeyDown(KeyCode.RightArrow)) v = -10;

        Vector3 movement = new Vector3(h,0,v);
 
        rigidBody.MovePosition(GetComponent<Transform>().position + movement);
	}
}
