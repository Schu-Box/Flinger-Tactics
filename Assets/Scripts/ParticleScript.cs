using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

	private ParticleSystem ps;
	private ParticleSystem.EmissionModule pse;
	private AthleteController ac = null;

	float startRate;
	float minimumRate = 20f;

	private void Start() {
		ps = gameObject.GetComponent<ParticleSystem>();
		pse = ps.emission;

		startRate = pse.rateOverTime.constant;
	}

	public void SetAthleteController(AthleteController a) {
		ac = a;
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

		Debug.Log("V: " + baseVelocity + " at " + percent + " % and rate of " + newRate);
		
		pse.rateOverTime = new ParticleSystem.MinMaxCurve(newRate);


	}
}
