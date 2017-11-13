using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;      

	Vector3 offset = new Vector3(0, 75,  130);

	void Start () 
	{
		float desiredAngle = target.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

		Vector3 velocity = Vector3.zero;
		//Vector3 targetVec = target.transform.position;
		//targetVec += offset;
		transform.position = Vector3.SmoothDamp (transform.position, target.transform.position - (rotation * - offset), ref velocity, 0.1f);
		//transform.position = target.transform.position - (rotation * - offset);
		transform.LookAt (target.transform);
		transform.rotation *= Quaternion.Euler (-25, 0, 0);

	}


	void FixedUpdate () 
	{
		float desiredAngle = target.transform.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);

		Vector3 velocity = Vector3.zero;
		//Vector3 targetVec = target.transform.position;
		//targetVec += offset;
		transform.position = Vector3.SmoothDamp (transform.position, target.transform.position - (rotation * - offset), ref velocity, 0.1f);
		//transform.position = target.transform.position - (rotation * - offset);
		transform.LookAt (target.transform);
		transform.rotation *= Quaternion.Euler (-25, 0, 0);
	
	}

}
	
	
