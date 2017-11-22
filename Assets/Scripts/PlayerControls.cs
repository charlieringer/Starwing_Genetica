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

	public GameObject hitAlert;

	Rigidbody rb;

	public float currentSpeed = 0;
	private float currentTurn = 0;

	public Image playerHealthBar;
	public Image playerShieldBar;

	public Text bulletText;
	public Text reloadingText;

    public AudioClip ShootingSound;
    public AudioClip LaserBoosterSound;
    public AudioClip ShieldBoosterSound;
    public AudioClip ThrustersBoosterSound;
    public AudioClip Hit;
    public AudioClip Death;
    public AudioClip ReloadSound;
    public AudioSource source;
 
	int bullets = 200;

	bool reloading = false;
	float reloadProgress;
	float reloadTime = 1f;
	float fireRateTimer = 0f;
	float fireRate = 0.15f;

	int barrelRollTotalTurn = 0;
	int barrelRollForce = 0;
	int barrelRollRotation = 0;
	bool barrelRolling = false;
	
	void Awake()
	{
		maxHealth = StaticData.startingShipHealth;
		bulletDamage = StaticData.startingShipDamage;
		topSpeed = StaticData.startingShipSpeed;
	}

	// Use this for initialization
	void Start () {
		maxHealth = health;
        source = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody> ();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		if (hitAlert.activeSelf) hitAlert.SetActive (false);

        //changed colour based on gene (speed and bullet speed) information
		transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().materials[1].color = new Color(ValueRemapping( bulletDamage, 100, 225)/225, 20/225, 20/225, 0);
		transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().materials[3].color = new Color(1, ValueRemapping(topSpeed, 500, 225)/225, 0, 0);
		transform.GetChild(3).gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(0, 0, ValueRemapping(health, maxHealth, 225)/225, 0);
		updatePlayerHeathText();

		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;
	
		if (!barrelRolling && (Input.GetKeyDown (KeyCode.Z) || Input.GetKeyDown (KeyCode.X))) {
				doABarrelRoll ();
		}

		if (barrelRolling)
			continueToBarrelRoll ();

		if (Input.GetKeyDown (KeyCode.R) && !reloading) {
			reloading = true;
			reloadingText.text = "RELOADING";
		}

		if (health <= 0) {
            source.PlayOneShot(Death, .1f);
            transform.Rotate (new Vector3 (Random.Range (0, 360), Random.Range  (0, 360), Random.Range  (0, 360)) * Time.deltaTime);
			GetComponent<Rigidbody>().useGravity = true;
			ParticleSystem LeftParticle = LThruster.GetComponent<ParticleSystem>();
			ParticleSystem RightParticle = RThruster.GetComponent<ParticleSystem>();
			LeftParticle.Stop();
			RightParticle.Stop();
			if (transform.position.y < -400) {
				SceneManager.LoadScene ("GameOver");
			}
			return;
		}

		if (reloading) {
            source.PlayOneShot(ReloadSound, 0.1f);
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

		rb.AddTorque (transform.up * h * turnSpeed);
		rb.AddForce (transform.forward * v * accel);


//		if (v < 0.001 && v > -0.001) {
//			currentSpeed = currentSpeed / decel;
//		} else {
//			currentSpeed += v * Time.fixedDeltaTime * accel;
//		}
//
//		if (h < 0.001 && h > -0.001) {
//			currentTurn = currentTurn / decel;
//		} else {
//			currentTurn += h * Time.fixedDeltaTime * turnSpeed;
//		}
//
//		if (currentSpeed > topSpeed)currentSpeed = topSpeed;
//		if(currentSpeed < 0) currentSpeed = 0;
//
//
//		if (currentTurn > 0) currentTurn -= decel*10 * Time.fixedDeltaTime;
//		else if (currentTurn < 0) currentTurn += decel*10 * Time.fixedDeltaTime;
//
//		if (currentTurn > maxTurn ) currentTurn = maxTurn;
//		if (currentTurn < -maxTurn) currentTurn = -maxTurn;
//
//		thrust(currentSpeed);
//		turn(currentTurn);

		if (Input.GetKey(KeyCode.Space) && !reloading) 
        {
        fire ();
        source.PlayOneShot(ShootingSound, .02f);
        }
	
        handleThrusterEffect();

		
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
			reloading = true;
			reloadingText.text = "RELOADING";
			return;
		}

		if (fireRateTimer < fireRate) {
			fireRateTimer += Time.fixedDeltaTime;
			return;
		}
		fireRateTimer = 0;

		bullets -= 2;
		Vector3 leftGun = leftBarrel.transform.position;
		Vector3 rightGun = rightBarrel.transform.position;

		GameObject bulletL = Instantiate (bulletPreFab, leftGun, transform.rotation);
		bulletL.GetComponent<Rigidbody> ().velocity = (bulletL.transform.forward * -bulletSpeed )+ GetComponent<Rigidbody>().velocity; 
		bulletL.GetComponent<BulletData>().damage = bulletDamage;
		bulletL.GetComponent<BulletData>().parentShip = "Player";
		Destroy (bulletL, 0.5f);

		GameObject bulletR = Instantiate (bulletPreFab, rightGun, transform.rotation);
		bulletR.GetComponent<Rigidbody> ().velocity = (bulletR.transform.forward * -bulletSpeed )+ GetComponent<Rigidbody>().velocity; 
		bulletR.GetComponent<BulletData>().damage = bulletDamage;
		bulletR.GetComponent<BulletData>().parentShip = "Player";
        Destroy (bulletR, 0.5f);

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
		if(collision.gameObject.name.Contains("shot_prefab") && collision.gameObject.GetComponent<BulletData>().parentShip != "Player")
		{
			float damage = collision.gameObject.GetComponent<BulletData>().damage;
			collision.gameObject.GetComponent<BulletData>().updateParentDamageDealt();
			takeDamage (damage);
			Destroy(collision.gameObject);
		}

		if(collision.gameObject.name =="Enemy(Clone)")
		{
			float damage = collision.gameObject.GetComponent<EnemyBrain>().health;
			takeDamage (damage);
			//currentSpeed *= 0.8f;
			//currentTurn *= 0.8f;
			rb.velocity = rb.velocity*0.6f; //Vector3.zero;
			rb.angularVelocity = rb.angularVelocity*0.6f;//Vector3.zero;
			
			collision.gameObject.GetComponent<EnemyBrain>().health = 0;
		}

		if(collision.gameObject.name.Contains("ShieldPowerup"))
		{
			shields += (collision.gameObject.GetComponent<Booster> ().boostAmount)*2;
			if (shields >= 100) shields = 100;
			GetComponent<BoosterUIController>().queueOfMessages.Add(0);
            source.PlayOneShot(ShieldBoosterSound, 1.0f);
            Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("SpeedPowerup"))
		{
			topSpeed += collision.gameObject.GetComponent<Booster> ().boostAmount;
			GetComponent<BoosterUIController>().queueOfMessages.Add(2);
            source.PlayOneShot(ThrustersBoosterSound, 1.0f);
            Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("WeaponPowerup"))
		{
			bulletDamage += (collision.gameObject.GetComponent<Booster> ().boostAmount)*0.5f;
			GetComponent<BoosterUIController>().queueOfMessages.Add(1);
            source.PlayOneShot(LaserBoosterSound, 1.0f);
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
		hitAlert.SetActive (true);
		if (damage <= 0)
			return;
		if (shields >= damage) {
            source.PlayOneShot(Hit, .2f);
			shields -= damage;

		} else if (shields > 0) {
            source.PlayOneShot(Hit, .2f);
			health -= (damage - shields);
			shields = 0f;
        }
        else {
            source.PlayOneShot(Hit, .2f);
            health -= damage;
		}
		if (shields >= 100)
			shields = 100;
	}

	private static float ValueRemapping(float initialVal, float initialHigh,  float targetHigh)
	{
		if (initialVal >= initialHigh)
			return targetHigh;
		return ((initialVal*targetHigh)/initialHigh);
	}



	private void doABarrelRoll()
	{
		barrelRolling = true;
		barrelRollTotalTurn = 0;
		if(Input.GetKeyDown(KeyCode.Z))
		{
			barrelRollRotation = -10;
			barrelRollForce = 700;
		} else {		
			barrelRollRotation = 10;
			barrelRollForce = -700;
		}

		continueToBarrelRoll ();


	}

	private void continueToBarrelRoll()
	{
		rb.AddForce (transform.right * barrelRollForce);
		transform.GetChild(3).gameObject.transform.Rotate(0,0,barrelRollRotation);
		barrelRollTotalTurn += barrelRollRotation;
		if (barrelRollTotalTurn >= 360 || barrelRollTotalTurn <= -360) {
			barrelRolling = false;
		}
	}
}
