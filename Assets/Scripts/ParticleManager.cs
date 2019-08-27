using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

	public GameObject defaultBumpParticlePrefab;
	public GameObject breakParticlePrefab;
	public GameObject goalParticlePrefab;
	public GameObject bumperRestorationParticlePrefab;
	public GameObject chargedParticlePrefab;
	public GameObject dischargeParticlePrefab;
	public GameObject victoryParticlePrefab;
	public GameObject shockwavePaticlePrefab;

	public Transform court; //necessary to adjust scale

	public void PlayBump(Vector3 position, Color color) {
		ParticleSystem.MainModule ps = Instantiate(defaultBumpParticlePrefab, position, Quaternion.identity, court).GetComponent<ParticleSystem>().main;
		
		//Color adjustedColor = color * Color.Lerp(color, Color.black, 0.1f);

		ps.startColor = color;
	}

	public void PlayBreak(Bumper bumper) {
		Debug.Log("Play break");

		ParticleSystem.MainModule ps = Instantiate(breakParticlePrefab, bumper.transform.position, bumper.transform.rotation, court).GetComponent<ParticleSystem>().main;

		ps.startColor = bumper.GetTeam().GetDarkTint();
	}

	public void PlayGoal(Transform spawnSpot, Transform goalTrans, Team team) {
		Debug.Log("Play goal");

		ParticleSystem ps = Instantiate(goalParticlePrefab, spawnSpot.position, goalTrans.rotation, court).GetComponent<ParticleSystem>();

		ParticleSystem.MinMaxGradient test = new ParticleSystem.MinMaxGradient(team.primaryColor, team.secondaryColor);
		
		ParticleSystem.MainModule psm = ps.main;
		psm.startColor = test;
	}

	public void PlayBumperRestoration(Bumper bumper) {
		ParticleSystem.MainModule ps = Instantiate(bumperRestorationParticlePrefab, bumper.transform.position, bumper.transform.rotation, court).GetComponent<ParticleSystem>().main;

		ps.startColor = bumper.GetTeam().GetDarkTint();
	}

	public void PlayCharged(AthleteController ac) {
		Debug.Log("Started charge particles");

		ParticleSystem ps = Instantiate(chargedParticlePrefab, ac.transform.position, Quaternion.identity, ac.transform).GetComponent<ParticleSystem>();
		ps.transform.eulerAngles = ac.transform.eulerAngles;

		ac.SetActiveParticles(ps);
		ps.GetComponent<ParticleScript>().SetAthleteController(ac);
		
		ParticleSystem.MainModule psm = ps.main;

		psm.startColor = ac.GetAthlete().GetTeam().GetDarkTint();
	}

	public void StopCharged(AthleteController ac) {
		ParticleSystem aps = ac.GetActiveParticles();

		if(aps != null) {
			aps.Stop();

			ac.SetActiveParticles(null);
		} else {
			Debug.Log("ps don't exist thus it cannot be stopped.");
		}
	}

	public void PlayDischarge(AthleteController ac) {
		ParticleSystem ps = Instantiate(dischargeParticlePrefab, ac.transform.position, Quaternion.identity, court).GetComponent<ParticleSystem>();

		ParticleSystem.MainModule psm = ps.main;

		psm.startColor = ac.GetAthlete().GetTeam().GetDarkTint();
	}

	public void PlayVictoryConfetti(Vector3 startPos, Team winner) {
		ParticleSystem ps = Instantiate(victoryParticlePrefab, startPos, Quaternion.identity, court).GetComponent<ParticleSystem>();

		ParticleSystem.MinMaxGradient test = new ParticleSystem.MinMaxGradient(winner.primaryColor, winner.secondaryColor);

		ParticleSystem.MainModule psm = ps.main;

		psm.startColor = test;
	}

	public void PlayShockwaveParticle(Shockwave shockwave, Vector3 shockPosition, Color color) {
		Debug.Log("Shockwave particle is PLAYED");

		ParticleSystem ps = Instantiate(shockwavePaticlePrefab, shockPosition, shockwave.transform.rotation, court).GetComponent<ParticleSystem>();
		ParticleSystem.MainModule psm = ps.main;
		psm.startColor = color;
	}
}
