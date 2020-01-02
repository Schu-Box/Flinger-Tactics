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
	public GameObject victoryParticleFullScreenPrefab;
	public GameObject shockwaveParticlePrefab;
	public GameObject ballSpawnExplosionParticlePrefab;


	public Transform court; //necessary to adjust scale

	private ParticleSystem activeVictoryParticles;
	private ParticleSystem activeVictoryParticles2; //wow bad code nice bruh

	public void PlayBump(Vector3 position, Color color) {
		ParticleSystem.MainModule ps = Instantiate(defaultBumpParticlePrefab, position, Quaternion.identity, court).GetComponent<ParticleSystem>().main;
		
		//Color adjustedColor = color * Color.Lerp(color, Color.black, 0.1f);

		ps.startColor = color;
	}

	public void PlayBreak(Bumper bumper) {
		ParticleSystem.MainModule ps = Instantiate(breakParticlePrefab, bumper.transform.position, bumper.transform.rotation, court).GetComponent<ParticleSystem>().main;

		ps.startColor = bumper.GetTeam().GetDarkTint();
	}

	public void PlayGoal(Transform spawnSpot, Transform goalTrans, Team team) {
		ParticleSystem ps = Instantiate(goalParticlePrefab, spawnSpot.position, goalTrans.rotation, court).GetComponent<ParticleSystem>();

		ParticleSystem.MinMaxGradient test = new ParticleSystem.MinMaxGradient(team.primaryColor, team.secondaryColor);
		
		ParticleSystem.MainModule psm = ps.main;
		psm.startColor = test;
	}

	public void PlayBumperRestoration(Bumper bumper) {
		ParticleSystem ps = Instantiate(bumperRestorationParticlePrefab, bumper.transform.position, bumper.transform.rotation, court).GetComponent<ParticleSystem>();
		ParticleSystem.MainModule psm = ps.main;
		ParticleSystem.ShapeModule pss = ps.shape;

		psm.startColor = bumper.GetTeam().GetDarkTint();

		Vector3 newScale = pss.scale;
		newScale.y = bumper.transform.localScale.y * pss.scale.y;
		pss.scale = newScale;
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

		activeVictoryParticles2 = ps;

		ParticleSystem.MinMaxGradient test = new ParticleSystem.MinMaxGradient(winner.primaryColor, winner.secondaryColor);

		ParticleSystem.MainModule psm = ps.main;

		psm.startColor = test;
	}

	public void PlayFullScreenVictoryConfetti(Vector3 startPos, Team winner) {
		ParticleSystem ps = Instantiate(victoryParticleFullScreenPrefab, startPos, Quaternion.identity, court).GetComponent<ParticleSystem>();

		activeVictoryParticles = ps;

		ParticleSystem.MinMaxGradient test = new ParticleSystem.MinMaxGradient(winner.primaryColor, winner.secondaryColor);

		ParticleSystem.MainModule psm = ps.main;

		psm.startColor = test;
	}

	public void StopVictoryParticles() {
		if(activeVictoryParticles != null) {
			activeVictoryParticles.Stop();
			Destroy(activeVictoryParticles.gameObject);
			activeVictoryParticles = null;
		}

		if(activeVictoryParticles2 != null) {
			activeVictoryParticles2.Stop();
			Destroy(activeVictoryParticles2.gameObject);
			activeVictoryParticles2 = null;
		}
	}

	public void PlayShockwaveParticle(Shockwave shockwave, Vector3 shockPosition, Color color) {
		ParticleSystem ps = Instantiate(shockwaveParticlePrefab, shockPosition, shockwave.transform.rotation, court).GetComponent<ParticleSystem>();
		ParticleSystem.MainModule psm = ps.main;
		psm.startColor = color;
	}

	public void PlayBallSpawnExplosionParticlePrefab(Vector3 position) {
		ParticleSystem ps = Instantiate(ballSpawnExplosionParticlePrefab, position, Quaternion.identity, court).GetComponent<ParticleSystem>();
	}
}
