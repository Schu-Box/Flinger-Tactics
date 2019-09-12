using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour {

	private ParticleManager particleManager;

	private SpriteRenderer sr;

	float forceMultiplier = 300;

	List<Collider2D> collidersHit = new List<Collider2D>();

	public IEnumerator AnimateShockwave(Vector3 end, Color color) {
		particleManager = FindObjectOfType<ParticleManager>(); //Could probably handle this a bit better (maybe? I guess this is okay.)

		sr = GetComponent<SpriteRenderer>();
		sr.color = color;

		Vector3 start = Vector3.zero;
		
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.5f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localPosition = Vector3.Lerp(start, end, timer/duration);

			yield return waiter;
		}

		transform.localPosition = end;
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		
		bool alreadyHit = false;
		for(int i = 0; i < collidersHit.Count; i++) {
			if(collidersHit[i] == collider) {
				alreadyHit = true;
				break;
			}
		}
		
		if(!alreadyHit) {

			//Debug.Log("Shockwave shocking a fool (or ball)");

			if(collider.gameObject.tag == "Athlete") {
				AthleteController ac = collider.gameObject.GetComponent<AthleteController>();

				if(!ac.crowdAthlete) {
					ShockwaveAthlete(ac);

					collidersHit.Add(collider);
				}
			} else if(collider.gameObject.tag == "Ball") {
				ShockwaveBall(collider.gameObject.GetComponent<BallController>());

				collidersHit.Add(collider);
			}
		} //else it's been hit so ignore it
	}

	public void ShockwaveAthlete(AthleteController ac) {

		Vector3 forceAdded = ac.transform.position - transform.position;
		forceAdded = forceAdded.normalized;
		forceAdded *= forceMultiplier;

		//Debug.Log(forceAdded);

		ac.AddForce(forceAdded);

		particleManager.PlayShockwaveParticle(this, ac.transform.position, sr.color);
	}

	public void ShockwaveBall(BallController b) {
		Vector3 forceAdded = b.transform.position - transform.position;
		forceAdded = forceAdded.normalized;
		forceAdded *= forceMultiplier;

		b.AddForce(forceAdded);

		particleManager.PlayShockwaveParticle(this, b.transform.position, sr.color);
	}
}
