using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour {
    
    public float accel;

	public float health;
	public float maxHealth;
	private float shields = 0f;

	public float bulletDamage;
	public float bulletSpeed;
	public GameObject bulletPreFab;

	public GameObject pauseManager;

    public GameObject LThruster;
    public GameObject RThruster;

	public GameObject LBarrel;
	public GameObject RBarrel;

	public GameObject hitAlert;

	Rigidbody rb;

	public Image playerHealthBar;
	public Image playerShieldBar;
	public Image playerRollBar;

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

	float barrelRollCharge = 200;
	public float barrelRollCost = 20;

	int barrelRollTotalTurn = 0;
	float barrelRollForce = 0;
	float barrelRollRotation = 0;
	bool barrelRolling = false;

	private float turnSpeed;
	
	void Awake()
	{
		maxHealth = StaticData.startingShipHealth;
		bulletDamage = StaticData.startingShipDamage;
		accel = StaticData.startingShipSpeed;
		turnSpeed = accel / 6f;
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
		transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[1].color = new Color(ValueRemapping( bulletDamage, 100, 1), 0, 0, 0);
		transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[3].color = new Color(1, ValueRemapping(accel, 600, 1), 0, 0);
		transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[0].color = new Color(0, 0,ValueRemapping(shields, 100, 1), 0);
		transform.GetChild(2).gameObject.GetComponent<MeshRenderer>().materials[2].color = new Color(ValueRemapping(health, maxHealth, 1), ValueRemapping(health, maxHealth, 1), ValueRemapping(health, maxHealth, 1), 0);
		updatePlayerHeathText();

		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;
	
		if (!barrelRolling && (barrelRollCharge >= 20) && (Input.GetKeyDown (KeyCode.Z) || Input.GetKeyDown (KeyCode.X) || Input.GetKeyDown (KeyCode.Q) || Input.GetKeyDown (KeyCode.E))) {
				doABarrelRoll ();
		}

		barrelRollCharge += Time.fixedDeltaTime * 4;
		if (barrelRollCharge > 200)
			barrelRollCharge = 200;

		if (barrelRolling) continueToBarrelRoll ();

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
		if(v > -0.001) rb.AddForce (transform.forward * v * -accel);



		if (Input.GetKey(KeyCode.Space) && !reloading) 
        {
        fire ();
        source.PlayOneShot(ShootingSound, .02f);
        }
	
        handleThrusterEffect();

		
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
		Vector3 positon = transform.position;
		Vector3 leftGun = LBarrel.transform.position;
		Vector3 rightGun = RBarrel.transform.position;

		GameObject bulletL = Instantiate (bulletPreFab, leftGun, transform.rotation);
		bulletL.GetComponent<Rigidbody> ().velocity = (bulletL.transform.forward * -bulletSpeed) + GetComponent<Rigidbody>().velocity;
		bulletL.GetComponent<BulletData>().damage = bulletDamage;
		bulletL.GetComponent<BulletData>().parentShip = "Player";
		Destroy (bulletL, 0.5f);

		GameObject bulletR = Instantiate (bulletPreFab, rightGun, transform.rotation);
		bulletR.GetComponent<Rigidbody> ().velocity =  (bulletR.transform.forward * -bulletSpeed) + GetComponent<Rigidbody>().velocity; 
	
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
			accel += collision.gameObject.GetComponent<Booster> ().boostAmount;
			turnSpeed = accel / 6f;
			GetComponent<BoosterUIController>().queueOfMessages.Add(2);
            source.PlayOneShot(ThrustersBoosterSound, 1.0f);
            Destroy(collision.gameObject);
		}

		if(collision.gameObject.name.Contains("WeaponPowerup"))
		{
			bulletDamage += (collision.gameObject.GetComponent<Booster> ().boostAmount)*0.1f;
			GetComponent<BoosterUIController>().queueOfMessages.Add(1);
            source.PlayOneShot(LaserBoosterSound, 1.0f);
            Destroy(collision.gameObject);
		}
	}

	 void updatePlayerHeathText()
	 {
		playerHealthBar.rectTransform.sizeDelta = new Vector2((health/maxHealth) * 110f , 15);
		playerShieldBar.rectTransform.sizeDelta = new Vector2((100 - (100-shields))*1.1f , 15);
		playerRollBar.rectTransform.sizeDelta = new Vector2((barrelRollCharge/200) *110f , 15);
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
		if(Input.GetKeyDown(KeyCode.Z)  || Input.GetKeyDown(KeyCode.Q))
		{
			barrelRollRotation = -accel/50;
			barrelRollForce = accel;
		} else {		
			barrelRollRotation = accel/50;
			barrelRollForce = -accel;
		}
		barrelRollCharge -= barrelRollCost;
		continueToBarrelRoll ();


	}

	private void continueToBarrelRoll()
	{
		rb.AddForce (transform.right * barrelRollForce);
		transform.GetChild(2).gameObject.transform.Rotate(0,0,barrelRollRotation);
		barrelRollTotalTurn += (int)barrelRollRotation;
		if (barrelRollTotalTurn >= 360 || barrelRollTotalTurn <= -360) {
			transform.GetChild(2).gameObject.transform.rotation = new Quaternion(0,0,0,0) ;
			barrelRolling = false;
		}
	}
}
