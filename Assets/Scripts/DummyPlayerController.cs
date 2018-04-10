using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerController : MonoBehaviour {
	public float health;
	public float speed;
	public float damage;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(0,Time.deltaTime*45, 0);
		transform.Rotate(Time.deltaTime*50,0, 0);
		transform.Rotate(0, 0,Time.deltaTime*55);
	}

	private static float ValueRemapping(float initialVal, float initialHigh,  float targetHigh)
	{
		return ((initialVal*targetHigh)/initialHigh);
	}
}
