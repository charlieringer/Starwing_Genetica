using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIfader : MonoBehaviour {

	public Image linkedImage;
	bool active = false;

	float opacity = 0.0f;

	// Use this for initialization
	void Start () {
		linkedImage.color = new Color(1F, 1F, 1F, opacity);
	}
	
	// Update is called once per frame
	void Update () {
		if (active && opacity < 1.0f) 
		{
			opacity += 0.075f;
			linkedImage.color = new Color(1F, 1F, 1F, opacity);
		} else if (!active && opacity > 0.0f) 
		{
			opacity -= 0.075f;
			linkedImage.color = new Color(1F, 1F, 1F, opacity);
		}
		
	}

	public void setActive(bool value){
		active = value;
	}
}
