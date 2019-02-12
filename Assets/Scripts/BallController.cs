using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	private GameController gameController;
	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioSource;

	private List<SpriteRenderer> scoreSlots = new List<SpriteRenderer>();

	private List<Athlete> athleteTouchOrder = new List<Athlete>();
	private Athlete lastTurnToucher;

	private Team scoredByTeam = null;

	private Vector3 originalScale;

	private bool moving = false;

	private Vector2 lastVelocity = Vector2.zero;

	private void Start() {
		gameController = FindObjectOfType<GameController>();
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();

		for(int i = 0; i < transform.childCount; i++) {
			scoreSlots.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
		}

		originalScale = transform.localScale;
	}

	private void FixedUpdate() {
		Vector2 newVelocity = rb.velocity;
		if(lastVelocity.magnitude == 0) {
			if(newVelocity.magnitude > 0) {
				if(!moving) {
					StartedMoving();
				}
			}
		} else {
			Vector2 acceleration = (newVelocity - lastVelocity) / Time.deltaTime;

			if(acceleration.magnitude < 0.1f) {
				if(moving) {
					StoppedMoving();
				}
			}
		}
		lastVelocity = newVelocity;
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		if(collision.gameObject.CompareTag("Athlete")) {
			TouchedByAthlete(collision.gameObject.GetComponent<AthleteController>().GetAthlete());
		}
	}

	private void TouchedByAthlete(Athlete athlete) {
		for(int i = 0; i < scoreSlots.Count; i++) {
			scoreSlots[i].color = athlete.GetTeam().color;
		}

		athleteTouchOrder.Add(athlete);
	}

	public void ResetTouchOrder() {
		if(athleteTouchOrder.Count > 0) {
			lastTurnToucher = athleteTouchOrder[athleteTouchOrder.Count - 1];
		} else {
			lastTurnToucher = null;
		}

		athleteTouchOrder = new List<Athlete>();
	}

	public void ScoreBall(Team scoringTeam) {
		scoredByTeam = scoringTeam;

		//This will lead to errors if no athlete has touched the ball this turn. Currently, this should not be possible
		Athlete scorer = athleteTouchOrder[athleteTouchOrder.Count - 1];
		if(scorer.GetTeam() == scoringTeam) {
			scorer.IncreaseStat("goals");

			Athlete assister;
			if(athleteTouchOrder.Count > 1) {
				assister = athleteTouchOrder[athleteTouchOrder.Count - 2];
			} else {
				assister = lastTurnToucher;
			}

			
			if(assister != null && assister.GetTeam() == scoringTeam && assister != scorer) {
				assister.IncreaseStat("assists");
			}
		} //else it's an own goal

		athleteTouchOrder = new List<Athlete>();
		lastTurnToucher = null;

		//Can probably get rid of these
		/*
		scoreSlots[teamScores.Count].color = scoringTeam.color;
		*/

		StartCoroutine(ShrinkBall());
	}

	public IEnumerator ShrinkBall() {
		WaitForEndOfFrame waiter = new WaitForEndOfFrame();

		float timer = 0f;
		float duration = 0.2f;
		while(timer < duration) {
			timer += Time.deltaTime;
			transform.localScale = Vector3.Lerp(originalScale, (originalScale * 1.4f), timer/duration);

			yield return waiter;
		}
		
		timer = 0f;
		duration = 0.2f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, timer/duration);

			yield return waiter;
		}

		DisableBall();
	}

	public void DisableBall() {
		//transform.localScale = originalScale;
		moving = false;
		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0f;
		gameObject.SetActive(false);
	}

	public void RespawnBall(Vector3 spawnPosition) {
		Debug.Log("Respawning ball");
		//spriteRenderer.color = Color.white;

		transform.position = spawnPosition;

		StartCoroutine(GrowBall(originalScale));
	}

	public IEnumerator GrowBall(Vector3 newScale) {
		WaitForEndOfFrame waiter = new WaitForEndOfFrame();

		float timer = 0f;
		float duration = 0.4f;
		while(timer < duration) {
			timer += Time.deltaTime;
			transform.localScale = Vector3.Lerp(Vector3.zero, newScale, timer/duration);

			yield return waiter;
		}

		transform.localScale = newScale;
	}

	private void StartedMoving() {
		moving = true;
	}

	private void StoppedMoving() {
		moving = false;

		rb.velocity = Vector2.zero;

		//spriteRenderer.color = Color.white;
		scoreSlots[0].color = Color.white;
	}

	public void SetScoredByTeam(Team team) {
		scoredByTeam = team;
	}

	public Team GetScoredByTeam() {
		return scoredByTeam;
	}

	public bool GetMoving() {
		return moving;
	}
}
