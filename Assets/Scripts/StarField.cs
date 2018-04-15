using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarField : MonoBehaviour {

	private Transform thisTrans;
	private ParticleSystem.Particle[] points;
	private float starDistanceSqr;
	private float starClipDistanceSqr;

	public Color starColour;
	public int maxStars = 100;
	public float starSize = 1f;
	public float starDistance = 60f;
	public float starClipDistance = 15f;

	// Use this for initialization
	void Start () {
		thisTrans = transform;
		starDistanceSqr = starDistance * starDistance;
		starClipDistanceSqr = starClipDistance * starClipDistance;
	}

	private void initStarField()
	{
		points = new ParticleSystem.Particle[maxStars];

		for (int i = 0; i < maxStars; i++) {
			points[i].position = Random.insideUnitSphere * starDistance + thisTrans.position;
			points[i].startColor = new Color (starColour.r, starColour.g, starColour.b, starColour.a);
			points [i].startSize = starSize;
		}
	}

	// Update is called once per frame
	void Update () {
		if (points == null)
			initStarField ();
		for (int i = 0; i < maxStars; i++) {
			Vector3 postionVel = points [i].position - thisTrans.position;
			if (postionVel.sqrMagnitude > starDistanceSqr) {
				points [i].position = Random.insideUnitSphere.normalized * starDistance + thisTrans.position;

			}

			if (postionVel.sqrMagnitude <= starClipDistanceSqr) {
				float percentage = postionVel.sqrMagnitude / starClipDistanceSqr;
				points [i].startColor = new Color (1, 1, 1, percentage);
				points [i].startSize = starSize * percentage;
			}
		}
		GetComponent<ParticleSystem> ().SetParticles (points, points.Length);
	}
}