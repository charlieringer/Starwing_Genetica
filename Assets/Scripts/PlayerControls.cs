using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour {
    
    public float topSpeed;
	public float decel;
	public float accel;
	public float turnSpeed;
    public float maxTurn;
	public float health;
	public float maxHealth;
	private float shields = 0f;

	public float bulletDamage;
	public float bulletSpeed;
	public GameObject bulletPreFab;

	public GameObject pauseManager;

    public GameObject LThruster;
    public GameObject RThruster;
    public GameObject leftBarrel;
	public GameObject rightBarrel;


	public float currentSpeed = 0;
	private float currentTurn = 0;

	public Image playerHealthBar;
	public Image playerShieldBar;
	
	void Awake(){}
    

	// Use this for initialization
	void Start () {
		maxHealth = health;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;

		if(health <= 0) SceneManager.LoadScene ("GameOver");
		
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		currentSpeed += v * Time.fixedDeltaTime * accel;
		currentSpeed -= decel * Time.fixedDeltaTime;

		if (currentSpeed > topSpeed)currentSpeed = topSpeed;
		if(currentSpeed < 0) currentSpeed = 0;

		currentTurn += h * Time.fixedDeltaTime * turnSpeed;
		if (currentTurn > 0) currentTurn -= decel*10 * Time.fixedDeltaTime;
		else if (currentTurn < 0) currentTurn += decel*10 * Time.fixedDeltaTime;

		if (currentTurn > maxTurn ) currentTurn = maxTurn;
		if (currentTurn < -maxTurn) currentTurn = -maxTurn;

		thrust(currentSpeed);
		turn(currentTurn);

		if (Input.GetKeyDown (KeyCode.Space)) {
			fire ();
		}

        handleThrusterEffect();
		updatePlayerHeathText();

    }


	private void thrust(float amount)
	{
		transform.position += transform.forward * Time.fixedDeltaTime * -amount;
	}

	private void turn(float amount)
	{
		float yaw = currentTurn * Time.fixedDeltaTime;
		transform.Rotate (0, yaw, 0);
	}

	public void fire()
	{
		Vector3 leftGun = leftBarrel.transform.position;
		Vector3 rightGun = rightBarrel.transform.position;

		GameObject bulletL = Instantiate (bulletPreFab, leftGun, transform.rotation);
		bulletL.GetComponent<Rigidbody> ().velocity = (bulletL.transform.forward * -bulletSpeed )+ GetComponent<Rigidbody>().velocity; 
		bulletL.GetComponent<BulletData>().damage = bulletDamage;
		bulletL.GetComponent<BulletData>().parentShip = "Player";
		Destroy (bulletL, 2.0f);

		GameObject bulletR = Instantiate (bulletPreFab, rightGun, transform.rotation);
		bulletR.GetComponent<Rigidbody> ().velocity = (bulletR.transform.forward * -bulletSpeed )+ GetComponent<Rigidbody>().velocity; 
		bulletR.GetComponent<BulletData>().damage = bulletDamage;
		bulletR.GetComponent<BulletData>().parentShip = "Player";
        Destroy (bulletR, 2.0f);




	}

    public void handleThrusterEffect()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        ParticleSystem LeftParticle = LThruster.GetComponent<ParticleSystem>();
        ParticleSystem RightParticle = RThruster.GetComponent<ParticleSystem>();
        var LeftEmission = LeftParticle.emission;
        var RightEmission = RightParticle.emission;

        if (v == 0 && h == 0)
        {
            LeftEmission.rateOverTime = 100.0f;
            RightEmission.rateOverTime = 100.0f;
        }
        else if (v > 0 && h == 0)
        {
            LeftEmission.rateOverTime = 400.0f;
            RightEmission.rateOverTime = 400.0f;
        }
        else if (v > 0 && h < 0)
        {
            LeftEmission.rateOverTime = 100.0f;
            RightEmission.rateOverTime = 400.0f;
        }
        else if (v > 0 && h > 0)
        {
            LeftEmission.rateOverTime = 400.0f;
            RightEmission.rateOverTime = 100.0f;
        }
        else if (v == 0 && h > 0)
        {
            LeftEmission.rateOverTime = 400.0f;
            RightEmission.rateOverTime = 100.0f;
        }
        else if (v == 0 && h < 0)
        {
            LeftEmission.rateOverTime = 100.0f;
            RightEmission.rateOverTime = 400.0f;
        }
    }


	void OnTriggerEnter(Collider collision)
	{
		if(collision.gameObject.name.Contains("Bullet") && collision.gameObject.GetComponent<BulletData>().parentShip != "Player")
		{
			float damage = collision.gameObject.GetComponent<BulletData>().damage;
			collision.gameObject.GetComponent<BulletData>().updateParentDamageDealt();
			takeDamage (damage);
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("Enemy(Clone)"))
		{
			float damage = collision.gameObject.GetComponent<EnemyBrain>().health;
			takeDamage (damage);
			currentSpeed *= 0.8f;
			currentTurn *= 0.8f;
			
			collision.gameObject.GetComponent<EnemyBrain>().health = 0;
		}

		if(collision.gameObject.name.Contains("ShieldPowerup"))
		{
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("SpeedPowerup"))
		{
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("WeaponPowerup"))
		{
			Destroy(collision.gameObject);
		}
	}

	 void updatePlayerHeathText()
	 {
		playerHealthBar.rectTransform.sizeDelta = new Vector2((health/maxHealth) * 110f , 15);
		playerShieldBar.rectTransform.sizeDelta = new Vector2((100 - (100-shields))*1.1f , 15);
	 }

	void takeDamage(float damage)
	{
		if (shields >= damage) {
			shields -= damage;
		} else if (shields > 0) {
			health -= (damage - shields);
			shields = 0;
		} else {
			health -= damage;
		}
	}

}
