using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerController : MonoBehaviour {

	private GameController gameController;
	private SpriteRenderer sr;

	private Vector2 originalSize;

	private float bumpForce = 1.5f;

	private Team teamOwner;

	private Coroutine currentCoroutine;

	private bool immune = false;
	private Vector3 restPosition;
	private Vector3 offFieldPosition;

	private void Start() {
		gameController = FindObjectOfType<GameController>();

		sr = GetComponent<SpriteRenderer>();
		originalSize = transform.localScale;
	}

	public void SetPosition() {
		restPosition = transform.position;
		offFieldPosition = transform.position;

		if(teamOwner == gameController.homeTeam) {
			offFieldPosition.x -= 3f;
		} else if(teamOwner == gameController.awayTeam){
			offFieldPosition.x += 3f;
		} else {
			Debug.Log("Error my dude");
		}
	}

	public void ChildCollision(Collision2D collision, ColliderController collCont) {
		currentCoroutine = StartCoroutine(BumpAnimation());

		if(collision.gameObject.CompareTag("Ball") || collCont.type == colliderType.Destructer) { //What is this tho?
			BreakBlock();
			//Could potentially also increase the break stat of whichever athlete broke it
		} else if(collision.gameObject.CompareTag("Athlete") && collision.gameObject.GetComponent<AthleteController>().GetAthlete().GetTeam() != teamOwner) {
			BreakBlock();
			collision.gameObject.GetComponent<AthleteController>().GetAthlete().IncreaseStat("breaks");
		} else {
			//Do nothing
		}

		Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
	
		otherRB.AddForce(collCont.bumpDirection * bumpForce, ForceMode2D.Impulse);
	}

	public IEnumerator BumpAnimation() {
		gameController.AddTrauma(0.25f);

		Vector2 shrinkSize = transform.localScale;
		shrinkSize.x = transform.localScale.x * 1.3f;
		shrinkSize.y = transform.localScale.y * 0.95f;

		float timer = 0f;
		float duration = 7f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer++;

			transform.localScale = Vector2.Lerp(originalSize, shrinkSize, timer/duration);

			yield return waiter;
		}

		//Debug.Log("Bump complete");
		EndCoroutines();
	}

	public void BreakBlock() {
		if(!immune) {
			StartCoroutine(DestoryAfterAnimation());
		}
	}

	public IEnumerator DestoryAfterAnimation() {
		gameController.AddTrauma(0.3f);

		yield return currentCoroutine;

		Vector2 shrinkSize = new Vector2(originalSize.x * 0.9f, 0);

		float timer = 0f;
		float duration = 7f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer++;

			transform.localScale = Vector2.Lerp(originalSize, shrinkSize, timer/duration);

			yield return waiter;
		}

		EndCoroutines();

		Deactivate();
	}

	public void Deactivate() {
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(false);
	}

	public void EndCoroutines() {
		transform.localScale = originalSize;

		if(currentCoroutine != null) {
			StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		}
	}

	public void RestoreBlock() {
		Reactivate();

		StartCoroutine(RestoreAnimation());
	}

	public void Reactivate() {
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(1).gameObject.SetActive(true);
	}

	public IEnumerator RestoreAnimation() {
		immune = true;

		transform.position = offFieldPosition;
		transform.localScale = originalSize;

		float timer = 0f;
		float duration = 40f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer++;

			transform.position = Vector3.Lerp(offFieldPosition, restPosition, timer/duration);

			yield return waiter;
		}

		immune = false;
	}

	public Team GetTeam() {
		return teamOwner;
	}

	public void SetTeam(Team owner) {
		teamOwner = owner;

		Start();

		SetPosition();
	}
}
