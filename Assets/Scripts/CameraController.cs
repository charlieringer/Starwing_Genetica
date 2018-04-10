using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;      

	Vector3 offset = new Vector3(0, 75,  130);

	void Start () 
	{
		//float desiredY = target.transform.eulerAngles.y;
		//float desiredZ = target.transform.eulerAngles.z;
		//float desiredX = target.transform.eulerAngles.x;
		//Quaternion rotation = Quaternion.Euler(desiredX, desiredY, desiredZ);

		//Vector3 velocity = Vector3.zero;
		//Vector3 targetVec = target.transform.position;
		//targetVec += offset;
		//transform.position = Vector3.SmoothDamp (transform.position, target.transform.position - (rotation * - offset), ref velocity, 0.02f);
		//transform.position = target.transform.position - (rotation * - offset);
		transform.LookAt (target.transform);
		//transform.rotation *= Quaternion.Euler (-25, 0, 0);

	}


	void FixedUpdate () 
	{
		float desiredY = target.transform.eulerAngles.y;
		float desiredZ = target.transform.eulerAngles.z;
		float desiredX = target.transform.eulerAngles.x;
		Quaternion rotation = Quaternion.Euler(desiredX, desiredY, desiredZ);

		Vector3 velocity = Vector3.zero;
		//Vector3 targetVec = target.transform.position;
		//targetVec += offset;
		transform.position = Vector3.SmoothDamp (transform.position, target.transform.position - (rotation * - offset), ref velocity, 0.02f);
		//transform.position = target.transform.position - (rotation * - offset);
		transform.LookAt (target.transform);
		transform.rotation *= Quaternion.Euler (-25, 0, 0);
	
	}

}
	
	
