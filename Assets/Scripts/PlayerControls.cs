using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
    
    public float topSpeed;
	public float decel;
	public float accel;
	public float turnSpeed;
    public float maxTurn;

	public float bulletDamage;
	public float bulletSpeed;
	public GameObject bulletPreFab;

	private float currentSpeed = 0;
	private float currentTurn = 0;

	List<GameObject> bullets = new List<GameObject>();
	
	void Awake(){}
    

	// Use this for initialization
	void Start () {}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		currentSpeed += v * Time.fixedDeltaTime;
		currentSpeed -= decel * Time.fixedDeltaTime;

		currentTurn += h * Time.fixedDeltaTime * turnSpeed;
		currentTurn -= decel * Time.fixedDeltaTime;


		if (currentTurn > maxTurn) currentTurn = maxTurn;

		if (currentSpeed > topSpeed)currentSpeed = topSpeed;
		if(currentSpeed < 0) currentSpeed = 0;

		thrust(currentSpeed);
		turn(currentTurn);

		if (Input.GetKeyDown (KeyCode.Space)) {
			fire ();
		}
	}


	private void thrust(float amount)
	{
		transform.position += transform.forward * topSpeed * Time.fixedDeltaTime * -amount * accel;

	}

	private void turn(float amount)
	{
		float yaw = currentTurn * Time.fixedDeltaTime;
		transform.Rotate (0, yaw, 0);
	}

	public void fire()
	{
		GameObject bullet = Instantiate (bulletPreFab, transform.position, transform.rotation);
		bullet.GetComponent<Rigidbody> ().velocity = bullet.transform.forward * -bulletSpeed - GetComponent<Rigidbody>().velocity; 
		bullet.GetComponent<BulletLogic>().damage = bulletDamage;
		Destroy (bullet, 2.0f);
	}


	void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.name == "Bullet")
		{
			Destroy(collision.gameObject);
		}
	}

}
