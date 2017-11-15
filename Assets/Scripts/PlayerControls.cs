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

	public Text bulletText;
	public Text reloadingText;

    public AudioClip ShootingSound;
    public AudioClip ThrustersSound;
    public AudioClip LaserBoosterSound;
    public AudioClip ShieldBoosterSound;
    public AudioClip ThrustersBoosterSound;
    public AudioSource source;
 
	int bullets = 200;

	bool firing = false;
	bool reloading = false;
	float reloadProgress;
	float reloadTime = 1.5f;
	void Awake(){}
    

	// Use this for initialization
	void Start () {
		maxHealth = health;
        source = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {

		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;

		if (health <= 0) {
			transform.Rotate (new Vector3 (Random.Range (0, 360), Random.Range  (0, 360), Random.Range  (0, 360)) * Time.deltaTime);
			GetComponent<Rigidbody>().useGravity = true;
			ParticleSystem LeftParticle = LThruster.GetComponent<ParticleSystem>();
			ParticleSystem RightParticle = RThruster.GetComponent<ParticleSystem>();
			LeftParticle.Stop();
			RightParticle.Stop();
			if(transform.position.y < -400) SceneManager.LoadScene ("GameOver");
			return;
		}

		if (reloading) {
			if (reloadProgress > reloadTime) {
				bullets = 200;
				reloadProgress = 0f;
				reloadingText.text = "";
				reloading = false;
			} else {
				reloadProgress += Time.fixedDeltaTime;
			}
		}
		
		
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");


		if (v < 0.001 && v > -0.001) {
			currentSpeed = currentSpeed / decel;
		} else {
			currentSpeed += v * Time.fixedDeltaTime * accel;
		}

		if (h < 0.001 && h > -0.001) {
			currentTurn = currentTurn / decel;
		} else {
			currentTurn += h * Time.fixedDeltaTime * turnSpeed;
		}

		if (currentSpeed > topSpeed)currentSpeed = topSpeed;
		if(currentSpeed < 0) currentSpeed = 0;


		if (currentTurn > 0) currentTurn -= decel*10 * Time.fixedDeltaTime;
		else if (currentTurn < 0) currentTurn += decel*10 * Time.fixedDeltaTime;

		if (currentTurn > maxTurn ) currentTurn = maxTurn;
		if (currentTurn < -maxTurn) currentTurn = -maxTurn;

		thrust(currentSpeed);
		turn(currentTurn);

		if (Input.GetKeyDown (KeyCode.Space)) {
			firing = true;
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			firing = false;
		}
        if (firing){
            source.PlayOneShot(ShootingSound, .1f);
            fire();
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
		
		if (bullets <= 0) {
			firing = false;
			reloading = true;
			reloadingText.text = "RELOADING";
			return;
		}
		bullets -= 2;
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
            //source.PlayOneShot(ThrustersSound, 0.1f);
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
			shields += (collision.gameObject.GetComponent<Booster> ().boostAmount)*2;
			if (shields > 100)
				shields = 100;
			GetComponent<BoosterUIController>().queueOfMessages.Add(0);
            source.PlayOneShot(ShieldBoosterSound, 0.5f);
            Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("SpeedPowerup"))
		{
			topSpeed += collision.gameObject.GetComponent<Booster> ().boostAmount;
			GetComponent<BoosterUIController>().queueOfMessages.Add(2);
            source.PlayOneShot(ThrustersBoosterSound, 0.5f);
            Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("WeaponPowerup"))
		{
			bulletDamage += (collision.gameObject.GetComponent<Booster> ().boostAmount)*0.5f;
			GetComponent<BoosterUIController>().queueOfMessages.Add(1);
            source.PlayOneShot(LaserBoosterSound, 0.5f);
            Destroy(collision.gameObject);
		}
	}

	 void updatePlayerHeathText()
	 {
		playerHealthBar.rectTransform.sizeDelta = new Vector2((health/maxHealth) * 110f , 15);
		playerShieldBar.rectTransform.sizeDelta = new Vector2((100 - (100-shields))*1.1f , 15);
		bulletText.text = bullets + "/200";
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
