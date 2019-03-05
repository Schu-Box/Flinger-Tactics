using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	private CameraController cameraController;
	private AudioManager audioManager;

	private Rigidbody2D rb;
	private Collider2D collie;
	private SpriteRenderer spriteRenderer;
	private AudioSource audioSource;

	private SpriteRenderer lightSlot;

	private List<AthleteController> athleteTouchOrder = new List<AthleteController>();
	private Bumper lastBumperTriggerEntered;

	private Team scoredByTeam = null;

	private Vector3 originalScale;

	private bool moving = false;
	private bool scoreAnimationInProgress = false;

	private Vector2 lastVelocity = Vector2.zero;

	private void Start() { 
		cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();

		rb = GetComponent<Rigidbody2D>();
		collie = GetComponent<Collider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();

		lightSlot = transform.GetChild(0).GetComponent<SpriteRenderer>();

		originalScale = transform.localScale;

		//audioSource.clip = audioManager.GetBallBumpClip();
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
		audioManager.PlaySound("ballBump");

		if(collision.gameObject.CompareTag("Athlete")) {
			TouchedByAthlete(collision.gameObject.GetComponent<AthleteController>());
		} else if(collision.gameObject.CompareTag("Goal")) {
			Debug.Log("Collided with goal!!");
		}
	}

	private void TouchedByAthlete(AthleteController athlete) {
		lightSlot.color = athlete.GetAthlete().GetTeam().primaryColor;

		athleteTouchOrder.Add(athlete);
	}

	public AthleteController GetLastToucher() {
		if(athleteTouchOrder.Count > 0) {
			return athleteTouchOrder[athleteTouchOrder.Count - 1];
		} else {
			return null;
		}
	}

	public void ResetTouchOrder() {
		athleteTouchOrder = new List<AthleteController>();
	}

	public void SetLastBumper(Bumper bumper) {
		lastBumperTriggerEntered = bumper;
	}

	public Bumper GetLastBumper() {
		return lastBumperTriggerEntered;
	}

	public void ScoreBall(Team scoringTeam, AthleteController initiater, Vector3 goalCenter) {
		scoredByTeam = scoringTeam;

		audioManager.PlaySound("ballGoal");

		AthleteController mostRecentToucher = null;
		if(athleteTouchOrder.Count > 0) {
			mostRecentToucher = athleteTouchOrder[athleteTouchOrder.Count - 1];
		}
		bool ownGoal = false;
		if(initiater.GetAthlete().GetTeam() != scoringTeam) { //If the initater is not on the scoring team
			ownGoal = false;
			for(int i = 0; i < athleteTouchOrder.Count; i++) { //Own goals only occur if no scoring teammate touched the ball during this turn
				if(athleteTouchOrder[i].GetAthlete().GetTeam() == scoringTeam) {
					ownGoal = false;
					break;
				} else {
					ownGoal = true;
				}
			}
		}

		if(mostRecentToucher != null && mostRecentToucher.GetAthlete().GetTeam() == scoringTeam) {
			AwardGoal(mostRecentToucher, initiater);
		} else if(!ownGoal) {
			AwardGoal(initiater, initiater);
		} else {
			//It's an own goal
			Debug.Log("Own goal!");
		}

		ResetTouchOrder();
		lastBumperTriggerEntered = null;

		//StartCoroutine(ShrinkBall());
		StartCoroutine(ScoreAnimation(scoringTeam.primaryColor, goalCenter));
	}

	public void AwardGoal(AthleteController scorer, AthleteController initiater) {
		scorer.IncreaseStat(StatType.Goals);

		List<AthleteController> assisters = new List<AthleteController>();
		for(int i = 0; i < athleteTouchOrder.Count; i++) {
			AthleteController ath = athleteTouchOrder[i];
			if(ath.GetAthlete().GetTeam() == scorer.GetAthlete().GetTeam() && ath != scorer) { //If a teammate also touched the ball then they're an assister
				bool alreadyOnList = false;
				for(int j = 0; j < assisters.Count; j++) {
					if(ath == assisters[j]) {
						alreadyOnList = true;
						break;
					}
				}
				if(!alreadyOnList) {
					assisters.Add(ath);
				}
			}
		}

		for(int i = 0; i < assisters.Count; i++) {
			if(assisters[i] != scorer) { //If this assister is not the scorer
				assisters[i].IncreaseStat(StatType.Assists);
			}
		}

		bool initiaterCredited = false; //Check to see if the initiater has been awarded an assist
		for(int i = 0; i < assisters.Count; i++) {
			if(assisters[i] == initiater) {
				initiaterCredited = true;
				break;
			}
		}
		if(!initiaterCredited && initiater != scorer) { //If the initiater was not awarded an assist or a goal, give them an assist
			initiater.IncreaseStat(StatType.Assists);
		}
	}

	public IEnumerator ScoreAnimation(Color scorerColor, Vector3 goalCenter) {
		scoreAnimationInProgress = true;

		WaitForEndOfFrame waiter = new WaitForEndOfFrame();

		Color originalColor = spriteRenderer.color;
		Color originalLightColor = lightSlot.color;

		StoppedMoving();
		collie.enabled = false;
		rb.simulated = false;
		
		lightSlot.color = originalLightColor;
		spriteRenderer.color = scorerColor;
		float timer = 0f;
		float duration = 0.5f;
		while(timer < duration) {
			timer += Time.deltaTime;

			//transform.localScale = Vector3.Lerp(originalScale, (originalScale / 2), timer/duration);

			yield return waiter;
		}

		Vector3 originalPosition = transform.position;
		Vector3 absorbPosition = goalCenter;
		absorbPosition.y = originalPosition.y;
	
		timer = 0f;
		duration = 0.4f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.position = Vector3.Lerp(originalPosition, absorbPosition, timer/duration);

			float step = Mathf.Sin((timer/duration) * Mathf.PI * 0.5f);
			transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, step);

			yield return waiter;
		}

		spriteRenderer.color = originalColor;

		scoreAnimationInProgress = false;

		DisableBall();
	}

	public bool GetScoreAnimationInProgress() {
		return scoreAnimationInProgress;
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

		collie.enabled = true;
		rb.simulated = true;
		
		lightSlot.color = Color.white;

		transform.localPosition = spawnPosition;

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

		lightSlot.color = Color.white;
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
