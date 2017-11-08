using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;      

	//Vector3 offset = new Vector3(0, 20, 10);
	Vector3 offset = new Vector3(0, 60, 0);


	void LateUpdate () 
	{
		Vector3 velocity = Vector3.zero;

		Vector3 targetVec = target.transform.position;
		targetVec += offset;
		transform.position = Vector3.SmoothDamp (transform.position, targetVec, ref velocity, 0);
	}
}
	
	
