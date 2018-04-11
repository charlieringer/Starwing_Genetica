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
	private int damageLevel = 0;
	public float bulletDamage;
	public float bulletSpeed;
	public GameObject bulletPreFab;
	public GameObject rocketPreFab;

	public GameObject pauseManager;

    public GameObject LThruster;
    public GameObject RThruster;

	public GameObject smoke;

	public GameObject LBarrel;
	public GameObject RBarrel;

	public GameObject hitAlert;

	public GameObject shield;

	private Vector3 target;
	private GameObject targetObj;
	public List<GameObject> enemies;

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

	float rocketCoolDown = 0.0f;

	float barrelRollCharge = 200;
	public float barrelRollCost = 20;

	int barrelRollTotalTurn = 0;
	float barrelRollForce = 0;
	float barrelRollRotation = 0;
	bool barrelRolling = false;
	bool uturning = false;

	int specialCharges = 100;
	int specialManov = 2;

	bool hasPlow = false;
	float plowTimer = 0.0f;

	private float turnSpeed;
	
	void Awake()
	{
		maxHealth = StaticData.startingShipHealth;
		bulletDamage = StaticData.startingShipDamage;
		accel = StaticData.startingShipSpeed;
		specialManov = StaticData.startShipSpecial;
		turnSpeed = accel / 60;

		if(specialManov == 0)transform.GetChild (0).GetChild (6).gameObject.SetActive (true);
		if(specialManov == 1)transform.GetChild (0).GetChild (7).gameObject.SetActive (true);
		if(specialManov == 2)transform.GetChild (0).GetChild (8).gameObject.SetActive (true);
	}

	// Use this for initialization
	void Start () {
		maxHealth = health;
        source = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody> ();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
		target = transform.position - transform.forward*1000;
		targetObj = null;
		transform.GetChild (1).transform.position = target;
		foreach(GameObject enemy in enemies)
		{
			float max = 0.0f;
			if (enemy.gameObject.activeSelf && Vector3.Distance(transform.position, enemy.transform.position) < 1000 && Vector3.Distance(transform.position, enemy.transform.position) > 20)
			{
				//Vector3 dirFromAtoB = (transform.position - (target + (target.normalized * player.GetComponent<Rigidbody>().velocity.magnitude * playerPathPredictionAmount * Time.fixedDeltaTime))).normalized;
				Vector3 dirFromAtoB = (transform.position - enemy.transform.position);

				float dotProd = Vector3.Dot(dirFromAtoB.normalized, transform.forward.normalized);
				if (dotProd > 0.9 && dotProd > max) {
					max = dotProd;
					target = enemy.transform.position + (enemy.GetComponent<EnemyBrain>().currentVelocity * Vector3.Distance(transform.position, enemy.transform.position)/bulletSpeed);
					targetObj = enemy;
					transform.GetChild (1).transform.position = enemy.transform.position;
				}
			}
		}

		//transform.GetChild (3).transform.rotation = Quaternion.LookRotation(transform.position - target);

		//Debug.DrawLine (LBarrel.transform.position, target, Color.green);
		//pwDebug.DrawLine (RBarrel.transform.position, target, Color.green);
		if (hitAlert.activeSelf)
			hitAlert.SetActive (false);
    
		updatePlayerHeathText();

		if (pauseManager.GetComponent<PauseHandler>().isPaused)
			return;
	
		if (!uturning && !barrelRolling && (barrelRollCharge >= 20) && (Input.GetKeyDown (KeyCode.Z) || Input.GetKeyDown (KeyCode.X) || Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.D))) {
				doABarrelRoll ();
		}

		if (specialManov == 0 && specialCharges > 0 && !uturning && !barrelRolling && (Input.GetKeyDown (KeyCode.C) || Input.GetKeyDown (KeyCode.F))) {
			doAUTurn ();
		}

		barrelRollCharge += Time.fixedDeltaTime * 4;
		if (barrelRollCharge > 200)
			barrelRollCharge = 200;

		if (barrelRolling) continueToBarrelRoll ();
		if (uturning) continueToUTurn ();

		rocketCoolDown -= Time.deltaTime;
		if (rocketCoolDown < 0)
			rocketCoolDown = 0f;
		if (Input.GetKeyDown (KeyCode.R) && !reloading) {
			reloading = true;
			reloadingText.text = "RELOADING";
		}

		if (health <= 0) {
			shield.GetComponent<MeshRenderer> ().material.color = new Color (0f,0f,0f,0f);
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
		rb.AddTorque (transform.right * v * turnSpeed);
		if(Input.GetKey(KeyCode.W)) rb.AddForce (transform.forward  * -accel);



		if ((Input.GetKey(KeyCode.Space)||Input.GetMouseButton(0)) && !reloading) 
        {
        	fire();
        	source.PlayOneShot(ShootingSound, .02f);
        }
		if ((Input.GetKey(KeyCode.C)||Input.GetKey(KeyCode.F)) && !reloading && rocketCoolDown == 0 && specialManov == 1 && specialCharges > 0) 
		{
			fireRocket ();
			rocketCoolDown = 1.5f;
			specialCharges -= 1;
			source.PlayOneShot(ShootingSound, .02f);
		}

		if ((Input.GetKey(KeyCode.C)||Input.GetKey(KeyCode.F)) && specialManov == 2 && hasPlow == false && specialCharges > 0) 
		{
			hasPlow = true;
			specialCharges -= 1;
			plowTimer = 5;
			transform.GetChild (0).GetChild (8).GetChild (0).gameObject.SetActive (true);
			transform.GetChild (0).GetChild (8).GetChild (1).gameObject.SetActive (true);
		}

		if (hasPlow) {
			plowTimer -= Time.deltaTime;
			if (plowTimer <= 0) {
				hasPlow = false;
				transform.GetChild (0).GetChild (8).GetChild (0).gameObject.SetActive (false);
				transform.GetChild (0).GetChild (8).GetChild (1).gameObject.SetActive (false);
			}
		}
	
        handleThrusterEffect();

		shield.GetComponent<MeshRenderer> ().material.color = new Color (0.1f, 0.78f, 0.85f, (float)((shields / 100) * 0.33));


		if ((health / maxHealth) < 0.75 && damageLevel == 0) {
			damageLevel += 1;
			transform.GetChild (5).GetChild (0).GetComponent<ParticleSystem> ().Play ();
			transform.GetChild (0).GetChild (0).gameObject.SetActive (false);
		}if ((health / maxHealth) < 0.5  && damageLevel == 1) {
			damageLevel += 1;
			transform.GetChild (5).GetChild (1).GetComponent<ParticleSystem> ().Play ();
			transform.GetChild (0).GetChild (1).gameObject.SetActive (false);
		}if ((health / maxHealth) < 0.25 && damageLevel == 2)
		{
			damageLevel += 1;
			transform.GetChild (5).GetChild (3).GetComponent<ParticleSystem>().Play();
			transform.GetChild (0).GetChild (3).gameObject.SetActive (false);
		}if ((health / maxHealth) < 0.1 && damageLevel == 3) {
			damageLevel += 1;
			transform.GetChild (5).GetChild (2).GetComponent<ParticleSystem> ().Play ();
			transform.GetChild (0).GetChild (2).gameObject.SetActive (false);

		}

		ParticleSystem smokeParticle = smoke.GetComponent<ParticleSystem>();
		var particles = smokeParticle.emission;
		particles.rateOverTime = (1-(health/maxHealth))*15.0f;
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

		Quaternion rotation;

		if (target != null)
		{
			//rotation = Quaternion.LookRotation( target.transform.position - transform.position);
			rotation = Quaternion.LookRotation(transform.position - target);
		}
		else rotation = transform.rotation;

		Quaternion noise = Quaternion.Euler(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		Quaternion leftRotation = Quaternion.LookRotation(leftGun - target) * noise;
		GameObject bulletL = Instantiate (bulletPreFab, leftGun, leftRotation);
		bulletL.GetComponent<Rigidbody> ().velocity = (bulletL.transform.forward * -bulletSpeed) + GetComponent<Rigidbody>().velocity;
		bulletL.GetComponent<BulletData>().damage = bulletDamage;
		bulletL.GetComponent<BulletData>().parentShip = "Player";
		Destroy (bulletL, 0.5f);

		noise = Quaternion.Euler(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		Quaternion rightRotation = Quaternion.LookRotation(rightGun - target) * noise;
		GameObject bulletR = Instantiate (bulletPreFab, rightGun, rightRotation);
		bulletR.GetComponent<Rigidbody> ().velocity =  (bulletR.transform.forward * -bulletSpeed) + GetComponent<Rigidbody>().velocity; 
		bulletR.transform.rotation *= noise;
	
		bulletR.GetComponent<BulletData>().damage = bulletDamage;
		bulletR.GetComponent<BulletData>().parentShip = "Player";
        Destroy (bulletR, 0.5f);
	}

	public void fireRocket()
	{
		specialCharges -= 1;
		Vector3 positon = transform.GetChild (0).GetChild (11).position;

		GameObject rocket = Instantiate (rocketPreFab, positon, transform.rotation);
		rocket.GetComponent<RocketScript> ().target = targetObj;
		rocket.GetComponent<RocketScript> ().currentVelocity = rb.velocity;
	}

    public void handleThrusterEffect()
    {
        float h = Input.GetAxis("Horizontal");
		float v = Input.GetKey(KeyCode.W) ? 1 : 0;

        ParticleSystem LeftParticle = LThruster.GetComponent<ParticleSystem>();
        ParticleSystem RightParticle = RThruster.GetComponent<ParticleSystem>();
        var LeftEmission = LeftParticle.emission;
        var RightEmission = RightParticle.emission;

        if (v == 0 && h == 0)
        {
            LeftEmission.rateOverTime = 10.0f;
            RightEmission.rateOverTime = 10.0f;
        }
        else if (v > 0 && h == 0)
        {
            LeftEmission.rateOverTime = 1000.0f;
            RightEmission.rateOverTime = 1000.0f;
        }
        else if (v > 0 && h < 0)
        {
            LeftEmission.rateOverTime = 600.0f;
            RightEmission.rateOverTime = 2000.0f;
        }
        else if (v > 0 && h > 0)
        {
            LeftEmission.rateOverTime = 2000.0f;
            RightEmission.rateOverTime = 600.0f;
        }
        else if (v == 0 && h > 0)
        {
            LeftEmission.rateOverTime = 400.0f;
            RightEmission.rateOverTime = 10.0f;
        }
        else if (v == 0 && h < 0)
        {
            LeftEmission.rateOverTime = 10.0f;
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
			if(!hasPlow)takeDamage (damage);
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
		if (damage <= 0)
			return;
		source.PlayOneShot(Hit, .2f);


		if(shields > 0)shield.GetComponent<MeshRenderer> ().material.color = new Color (0.1f, 0.78f, 0.85f, 0.75f);
		else hitAlert.SetActive (true);
		if (shields >= damage) {
			shields -= damage;

		} else if (shields > 0) {
			health -= (damage - shields);
			shields = 0f;
        }
        else {
            health -= damage;
		}
		if (shields >= 100)
			shields = 100;
		transform.GetChild (6).GetComponent<ParticleSystem> ().Play ();
		
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
		if(Input.GetKeyDown(KeyCode.Z)  || Input.GetKeyDown(KeyCode.A))
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
		transform.GetChild(0).gameObject.transform.Rotate(0,0,barrelRollRotation);
		barrelRollTotalTurn += (int)barrelRollRotation;
		if (barrelRollTotalTurn >= 360 || barrelRollTotalTurn <= -360) {
			transform.GetChild(0).gameObject.transform.rotation = new Quaternion(0,0,0,0) ;
			barrelRolling = false;
		}
	}

	private void doAUTurn()
	{
		uturning = true;
		barrelRollTotalTurn = 0;

		barrelRollRotation = -accel/50;
		barrelRollForce = accel;

		specialCharges -= 1;
		continueToUTurn ();
	}

	private void continueToUTurn()
	{
		rb.AddForce (transform.right * barrelRollForce);
		rb.AddForce (transform.forward * barrelRollForce);
		transform.GetChild(0).gameObject.transform.Rotate(0,0,barrelRollRotation);
		transform.GetChild(0).gameObject.transform.Rotate(barrelRollRotation,0,0);
		barrelRollTotalTurn += (int)barrelRollRotation;
		if (barrelRollTotalTurn >= 180 || barrelRollTotalTurn <= -180) {
			transform.RotateAround (transform.position, transform.up, 180f);;
			transform.GetChild(0).gameObject.transform.rotation = new Quaternion(0,0,0,0);

			uturning = false;
		}
	}
}
