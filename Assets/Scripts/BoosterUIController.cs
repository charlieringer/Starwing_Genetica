using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterUIController : MonoBehaviour {

	public List<int> queueOfMessages;	
	public Text messageText;
	float displayTime = 1.5f;
	float currentDisplayedtime = 0;
	bool started = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (queueOfMessages.Count == 0)
			return;
		if (!started) {
			started = true;
			if (queueOfMessages [0] == 0)
				messageText.text = "SHIELDS CHARGED";
			else if (queueOfMessages [0] == 1)
				messageText.text = "WEAPONS IMPROVED";
			else if (queueOfMessages [0] == 2)
				messageText.text = "THRUSTERS UPGRADED";
		}
		if (currentDisplayedtime + Time.deltaTime > displayTime) {
			currentDisplayedtime = 0;
			queueOfMessages.RemoveAt (0);
			started = false;
			messageText.text = "";
		} else {
			currentDisplayedtime += Time.deltaTime;
		}
	}
}
