using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

	private ParticleSystem ps;
	private ParticleSystem.EmissionModule pse;
	private ParticleSystem.ShapeModule pss;
	private AthleteController ac = null;

	float startRate;
	float minimumRate = 20f;

	float minStretch = 1.5f;
	float maxStretch;

	private void Start() {
		ps = gameObject.GetComponent<ParticleSystem>();
		pse = ps.emission;
		pss = ps.shape;

		startRate = pse.rateOverTime.constant;

		maxStretch = ps.shape.radius;

		AdjustTailStretch(0);
	}

	public void SetAthleteController(AthleteController a) {
		ac = a;
	}

	public void AdjustTailStretch(float percentStretch) {
		pss.radius = Mathf.Lerp(minStretch, maxStretch, percentStretch);
	}

	public void AdjustedVelocity(Vector2 newVelocity) {
		float baseVelocity = newVelocity.magnitude;
		float topSpeed = 10f;

		if(newVelocity.magnitude > topSpeed) {
			baseVelocity = topSpeed;
		}

		float percent = 1 - baseVelocity / topSpeed;

		float newRate = startRate * percent;
		if(newRate < minimumRate) {
			newRate = minimumRate;
		}

		//Debug.Log("V: " + baseVelocity + " at " + percent + " % and rate of " + newRate);
		
		pse.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);
	}
}
