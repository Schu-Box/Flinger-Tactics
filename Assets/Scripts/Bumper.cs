using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

	public Vector2 bumpDirection = new Vector2(1, 0);

	private MatchController matchController;
	private CameraController cameraController;
	private AudioManager audioManager;
	private ParticleManager particleManager;


	private SpriteRenderer spriteRenderer;
	private Collider2D collie;


	private BumperTrigger bumperTrigger;

	private GoalController parentGoal;

	private Coroutine currentCoroutine;

	private Vector2 originalSize;

	private float bumpForce = 1f;

	private Team teamOwner;

	private Vector3 restPosition;
	private Vector3 offFieldPosition;

	private bool indestructible = false;
	private bool immune = false;
	private bool goal = true;

	private bool disabled = false;

	void Awake() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
		particleManager = FindObjectOfType<ParticleManager>();

		spriteRenderer = GetComponent<SpriteRenderer>();
		collie = GetComponent<Collider2D>();

		bumperTrigger = GetComponentInChildren<BumperTrigger>();

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

		audioManager.Play("bumperBump");

		if(!indestructible && !immune) {
			if(collision.gameObject.CompareTag("Ball")) {
				BreakBlock();
				
				AthleteController lastToucher = collision.gameObject.GetComponent<BallController>().GetLastToucher();
				if(lastToucher != null && lastToucher.GetAthlete().GetTeam() != teamOwner) {
					lastToucher.IncreaseStat(StatType.Breaks);
				} else {
					if(matchController.GetAthleteInitiater().GetAthlete().GetTeam() != teamOwner) {
						matchController.GetAthleteInitiater().IncreaseStat(StatType.Breaks);
					}
				}
			} else if(collision.gameObject.CompareTag("Athlete")) {
				AthleteController ac = collision.gameObject.GetComponent<AthleteController>();
				if(ac.GetAthlete().GetTeam() != teamOwner) {
					BreakBlock();
					ac.IncreaseStat(StatType.Breaks);
				} else {
					//if athlete sendsShockwaves
					if(ac.GetSendsShockwaves()) {
						cameraController.AddTrauma(0.4f);
						SendShockwave();
					}
				}
			} else {
				//Do nothing
			}
		}

		Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();

		float modifier = 1f;
		if(otherRB.gameObject.GetComponent<AthleteController>() != null) {
			modifier = otherRB.gameObject.GetComponent<AthleteController>().GetAthlete().athleteData.bumperModifier;
		}
	
		otherRB.AddForce(bumpDirection * bumpForce * modifier, ForceMode2D.Impulse);
	}


	public IEnumerator BumpAnimation() {
		if(goal) {
			cameraController.AddTrauma(0.6f);
		} else {
			cameraController.AddTrauma(0.4f);
		}

		Vector2 shrinkSize = transform.localScale;
		if(!goal) {
			shrinkSize.x = transform.localScale.x * 1.3f;
			shrinkSize.y = transform.localScale.y * 0.95f;
		} else {
			shrinkSize.x = transform.localScale.x * 1.15f;
		}
		

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
		particleManager.PlayBreak(this);

		StartCoroutine(DestoryAfterAnimation());
	}

	public IEnumerator DestoryAfterAnimation() {
		cameraController.AddTrauma(0.3f); //In addition to the Bump Animation Trauma

		yield return currentCoroutine;

		audioManager.Play("bumperBreak");

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
		collie.enabled = false;

		disabled = true;
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

		particleManager.PlayBumperRestoration(this);

		StartCoroutine(RestoreAnimation());
	}

	public void Reactivate() {
		spriteRenderer.enabled = true;
		collie.enabled = true;

		disabled = false;
	}

	public IEnumerator RestoreAnimation() {
		immune = true;

		transform.position = offFieldPosition;
		transform.localScale = originalSize;

		yield return new WaitForSeconds(0.6f);

		float timer = 0f;
		float duration = 0.4f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.position = Vector3.Lerp(offFieldPosition, restPosition, timer/duration);

			yield return waiter;
		}

		immune = false;
	}

	public bool GetDisabled() {
		return disabled;
	}

	public bool GetImmunue() {
		return immune;
	}

	public void SendShockwave() {
		Debug.Log("Shockwave");

		Shockwave shocker = Instantiate(matchController.shockwavePrefab, Vector3.zero, Quaternion.identity, this.transform).GetComponent<Shockwave>();

		float xDistance = 5f;
		Vector3 endPosition = Vector3.zero;
		if(teamOwner == matchController.GetTeam(true)) { //if it's the home team
			endPosition.x += xDistance;
		} else {
			shocker.transform.localEulerAngles = new Vector3(180, 0, 0);

			endPosition.x += xDistance;
		}

		StartCoroutine(shocker.AnimateShockwave(endPosition, teamOwner.secondaryColor));
	}
}
