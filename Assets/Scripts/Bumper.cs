using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

	public Vector2 bumpDirection = new Vector2(1, 0);

	private MatchController matchController;
	private CameraController cameraController;
	private AudioManager audioManager;
	private AudioSource audioSource;
	private SpriteRenderer spriteRenderer;
	private Collider2D collie;

	private GoalController parentGoal;

	private Coroutine currentCoroutine;

	private Vector2 originalSize;

	private float bumpForce = 1.5f;

	private Team teamOwner;

	private Vector3 restPosition;
	private Vector3 offFieldPosition;

	private bool indestructible = false;
	private bool immune = false;
	private bool goal = true;

	void Awake() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
		audioSource = GetComponent<AudioSource>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		collie = GetComponent<Collider2D>();

		originalSize = transform.localScale;
	}

	public void SetAsGoal(bool isGoal) {
		goal = isGoal;
		indestructible = isGoal;
	}

	public Team GetTeam() {
		return teamOwner;
	}

	public void SetTeam(Team owner) {
		teamOwner = owner;

		if(matchController.GetTeam(true) == owner) {
			parentGoal = matchController.homeGoal;
		} else {
			parentGoal = matchController.awayGoal;
		}

		SetPosition();
	}

	public void SetPosition() {
		restPosition = transform.position;
		offFieldPosition = transform.position;
		offFieldPosition.x = parentGoal.transform.position.x;
	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		currentCoroutine = StartCoroutine(BumpAnimation());

		audioManager.PlaySound("bumperBump");

		if(!indestructible && !immune) {
			if(collision.gameObject.CompareTag("Ball")) {
				BreakBlock();
				AthleteController lastToucher = collision.gameObject.GetComponent<BallController>().GetLastToucher();
				if(lastToucher != null) {
					if(lastToucher.GetAthlete().GetTeam() != teamOwner) {
						lastToucher.IncreaseStat(StatType.Breaks);
					} else { //If the last toucher isn't on the scoring team, then award the break to the initiater (if they're on the scoring team)
						if(matchController.GetAthleteInitiater().GetAthlete().GetTeam() != teamOwner) {
							matchController.GetAthleteInitiater().IncreaseStat(StatType.Breaks);
						}
					}
				}
			} else if(collision.gameObject.CompareTag("Athlete") && collision.gameObject.GetComponent<AthleteController>().GetAthlete().GetTeam() != teamOwner) {
				BreakBlock();
				collision.gameObject.GetComponent<AthleteController>().IncreaseStat(StatType.Breaks);
			} else {
				//Do nothing
			}
		}

		Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();

		float modifier = 1f;
		if(otherRB.gameObject.GetComponent<AthleteController>() != null) {
			modifier = otherRB.gameObject.GetComponent<AthleteController>().GetAthlete().athleteType.bumperModifier;
		}
	
		otherRB.AddForce(bumpDirection * bumpForce * modifier, ForceMode2D.Impulse);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.CompareTag("Ball")) { //Ball entered bumper zone
			other.gameObject.GetComponent<BallController>().SetLastBumper(this);
		}
	}

	public IEnumerator BumpAnimation() {
		if(goal) {
			cameraController.AddTrauma(0.6f);
		} else {
			cameraController.AddTrauma(0.4f);
		}

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
		StartCoroutine(DestoryAfterAnimation());
	}

	public IEnumerator DestoryAfterAnimation() {
		cameraController.AddTrauma(0.3f); //In addition to the Bump Animation Trauma

		yield return currentCoroutine;

		audioManager.PlaySound("bumperBreak");

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
		spriteRenderer.enabled = false;
		collie.isTrigger = true;
	}

	public void EndCoroutines() {
		transform.localScale = originalSize;

		if(currentCoroutine != null) {
			StopCoroutine(currentCoroutine);
			currentCoroutine = null;
		}
	}

	public void RestoreBumper() {
		Reactivate();

		StartCoroutine(RestoreAnimation());
	}

	public void Reactivate() {
		spriteRenderer.enabled = true;
		collie.isTrigger = false;
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
}
