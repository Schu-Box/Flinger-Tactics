using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.EventSystems;

public class AthleteController : MonoBehaviour{

	private Athlete athlete;

	//Variables
	private float tongueForce = 600f;
	
	private float tailStretch = 15;

	//Body Parts
	public List<SpriteRenderer> legList;

	private Body body;
	private TailBody tailBody;
	private TailTip tailTip;
	private Face face;
	private Tongue tongue;
	private TextMeshPro jerseyText;

	private MatchController matchController;
	private CameraController cameraController;
	private AudioManager audioManager;
	private AudioSource audioSource;

	private Vector3 directionDragged = Vector3.zero;

	private Coroutine squishCoroutine;
	private Coroutine runningCoroutine;
	private Coroutine tongueCoroutine;

	private bool instantInteraction = false;
	private bool disabledInteraction = false;
	private bool moving = false;
	private bool ready = false;
	private bool dizzy = false;
	private float expressionTimer = 0f;

	private Vector2 lastVelocity;

	void Awake() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
		audioSource = GetComponent<AudioSource>();

		body = GetComponent<Body>();
		tailBody = GetComponentInChildren<TailBody>();
		tailTip = GetComponentInChildren<TailTip>();
		face = GetComponentInChildren<Face>();
		tongue = GetComponentInChildren<Tongue>();

		body.SetBody();
		tailBody.SetTailBody();
		tailTip.SetTailTip();
		face.SetFace();
		tongue.SetTongue();

		jerseyText = GetComponentInChildren<TextMeshPro>();

		SetInstantInteraction(true);
	}

	public void SetAthlete(Athlete a) {
		athlete = a;

		face.SetFaceBase(athlete.athleteType.faceBaseSprite);

		Color teamColor = athlete.GetTeam().primaryColor;
		body.SetColor(teamColor);
		tailTip.SetColor(teamColor);

		Color skinColor = athlete.skinColor;
		face.SetColor(skinColor);
		tailBody.SetColor(skinColor);

        for(int l = 0; l < legList.Count; l++) {
            legList[l].GetComponent<SpriteRenderer>().color = athlete.skinColor;

			if(l < legList.Count / 2) { //If it's the first half of legs
				legList[l].GetComponent<SpriteRenderer>().sprite = athlete.athleteType.frontLegSprite;
			} else {
				legList[l].GetComponent<SpriteRenderer>().sprite = athlete.athleteType.backLegSprite;
			}
        }

		RetractLegs();

		body.SetSprite(athlete.athleteType.bodySprite);

		jerseyText.text = a.jerseyNumber.ToString();
	}

	public Athlete GetAthlete() {
		return athlete;
	}

	public void SetInstantInteraction(bool isTrue) {
		instantInteraction = isTrue;
	}

	void FixedUpdate() {

		Vector2 newVelocity = body.GetVelocity();
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
					if(!dizzy) {
						StoppedMoving();
					}
				}
			}
		}
		lastVelocity = newVelocity;

		if(moving) {
			float step = newVelocity.magnitude / 2;
			float newlegLength;

			for(int i = 0; i < legList.Count; i++) {
				if(i < legList.Count / 2) {
					newlegLength = Mathf.Lerp(athlete.athleteType.frontLegMin, athlete.athleteType.frontLegMax, step);
				} else {
					newlegLength = Mathf.Lerp(athlete.athleteType.backLegMin, athlete.athleteType.backLegMax, step);
				}

				legList[i].size = new Vector2(legList[i].size.x, newlegLength);
			}
		}

		if(expressionTimer > 0f) {
			//Do nothing while the coroutine goes
		} else {
			if(Mathf.Abs(body.GetAngularVelocity()) > 60f) {
				if(!dizzy) {
					StartedDizziness();
				}
			} else {
				if(dizzy) {
					StoppedDizziness();
				}
			}
		}
	}

	public void Collided(Collision2D collision) {
		audioManager.PlaySound("athleteBump");

		IncreaseStat(StatType.Bumps);
		
		if(collision.gameObject.CompareTag("Athlete")){
			//cameraController.AddTrauma(0.18f);

			if(collision.gameObject.GetComponent<AthleteController>().GetAthlete().GetTeam() != GetAthlete().GetTeam()) {
				IncreaseStat(StatType.Tackles);
			}

			if(!dizzy) {
				if(collision.gameObject.GetComponent<AthleteController>().athlete.GetTeam() == athlete.GetTeam()) { //Teammate
					face.ChangeExpression("bumpedteam", 2f);
				} else { //Opponent
					face.ChangeExpression("bumpedenemy", 1.5f);
				}
			}
		} else {
			if(collision.gameObject.CompareTag("Ball")) {
				IncreaseStat(StatType.Touches);
			} /* else if(collision.gameObject.CompareTag("Bumper")) {
				IncreaseStat("bounces");
			}
			*/

			if(!dizzy) {
				face.ChangeExpression("bumped", 1f);
			}
		}

		//StartSquish();
	}

	public void DisableBody() {
		body.DisableBody();
	}

	public void EnableBody() {
		body.EnableBody();
	}

	public void DisableInteraction() {
		disabledInteraction = true;
	}

	public void IgnoreRaycasts(bool ignoring) {
		if(ignoring) {
			tailTip.gameObject.layer = 2;
		} else {
			tailTip.gameObject.layer = 0;
		}
	}

	public void DimAthleteColor() {
		Color teamColor = athlete.GetTeam().primaryColor;
		Color skinColor = athlete.skinColor;
		Color targetColor = Color.black;
		float lerpValue = 0.5f;

		body.SetColor(Color.Lerp(teamColor, targetColor, lerpValue));
		tailTip.SetColor(Color.Lerp(teamColor, targetColor, lerpValue));

		face.SetColor(Color.Lerp(skinColor, targetColor, lerpValue));
		tailBody.SetColor(Color.Lerp(skinColor, targetColor, lerpValue));

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = Color.Lerp(skinColor, targetColor, lerpValue);
		}

		tongue.SetTongueColor(Color.Lerp(athlete.tongueColor, targetColor, lerpValue));

		jerseyText.color = Color.Lerp(Color.white, targetColor, lerpValue);
	}

	public void EnableInteraction() { //And Restore Color
		disabledInteraction = false;
	}

	public void RestoreAthleteColor() {
		Color teamColor = athlete.GetTeam().primaryColor;
		Color skinColor = athlete.skinColor;

		body.SetColor(teamColor);
		tailTip.SetColor(teamColor);

		face.SetColor(skinColor);
		tailBody.SetColor(skinColor);

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = skinColor;
		}

		tongue.SetTongueColor(athlete.tongueColor);

		jerseyText.color = Color.white;
	}

	public bool IsDisabled() {
		return disabledInteraction;
	}

	public void MouseEnterBody() {
		matchController.AthleteHovered(athlete);
	}

	public void MouseExitBody() {
		matchController.AthleteUnhovered(athlete);
	}

	public void MouseEnterTail() {
		matchController.AthleteHovered(athlete);

		if(!disabledInteraction) {
			if(!matchController.GetAthleteBeingDragged()) {
				face.SetFaceSprite("hovered");
			}
		}
	}

	public void MouseExitTail() {
		matchController.AthleteUnhovered(athlete);

		if(!disabledInteraction) {
			if(!matchController.GetAthleteBeingDragged() && !moving) {
				face.SetFaceSprite("neutral");
			}
		}
	}

	public void Clicked() {
		//face.SetFaceSprite("dragging");
		if(!disabledInteraction) {
			matchController.SetAthleteBeingDragged(true);

			ExtendLegs();

			//Unready();
		}
	}

	public void TailAdjusted() {
		if(!disabledInteraction) {
			Vector3 flatDirection = transform.position;
			flatDirection.z = 0f;
			Vector3 direction = (flatDirection - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)));
		
			Vector3 normalizedDirection = direction.normalized;
			float zRotate = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, zRotate + 90);

			if(direction.magnitude > athlete.minPull) {
				if(direction.magnitude >= athlete.maxPull) {
					direction = Vector3.ClampMagnitude(direction, athlete.maxPull);
				}

				

				tailTip.AdjustTailPosition(direction.magnitude);

				if(tongue.GetTongueOut()) {
					tongue.HideTongue();
				}

				face.SetFaceSprite("dragging");
			} else {
				direction = Vector3.zero;

				ResetTail();

				if(!tongue.GetTongueOut()) {
					tongue.RevealTongue();
				}

				face.SetFaceSprite("hovered");
			}

			directionDragged = direction;
		}
	}

	public void ResetTail() {
		tailTip.AdjustTailPosition(0f);
	}

	public void Unclicked() {
		matchController.SetAthleteBeingDragged(false);

		if(directionDragged != Vector3.zero) { //Athlete will move
			if(instantInteraction) {
				matchController.Fling(this);
				StartAction();
			} else {
				ReadyUp();
			}
		} else {
			face.SetFaceSprite("neutral");
			RetractLegs();
		}
	}

	public void ReadyUp() {
		ready = true;

		face.SetFaceSprite("dragging");

		RetractLegs();

		if(runningCoroutine == null) {
			runningCoroutine = StartCoroutine(RunInPlace());
		}
	}

	/*
	public void CancelReady() {
		RetractLegs();

		if(!dizzy) {
			StartCoroutine(ChangeExpression(1f, "sad"));
		}

		Unready();
	}
	*/

	public void Unready() {
		ready = false;

		if(runningCoroutine != null) {
			StopCoroutine(runningCoroutine);
			runningCoroutine = null;
		}
	}

	public bool GetReady() {
		return ready;
	}

	public void StartAction() {
		Unready();

		if(directionDragged != Vector3.zero) {
			FlingAthlete();
		} else {
			if(tongue.GetTongueOut()) {
				FlickTongue();
			}
		}
	}

	public void FlingAthlete() {
		matchController.SetAthleteInitiater(this);
		IncreaseStat(StatType.Flings);

		Vector2 minDirectionDrag = directionDragged.normalized * athlete.minPull;
		Vector2 adjustedDirection = (Vector2)directionDragged - minDirectionDrag;
		if(adjustedDirection.magnitude < 0) {
			adjustedDirection = Vector2.zero;
		}

		Vector2 force = adjustedDirection * athlete.flingForce;
		body.AddForce(force);

		directionDragged = Vector3.zero;

		ResetTail();
	}

	public void IncreaseStat(StatType type) {
		if(matchController.IsTurnActive()) {
			athlete.IncreaseStat(type);

			if(matchController.GetAthleteInitiater() == this) {
				matchController.UpdateStats(type, athlete);
			}
		}
	}

	public void FlickTongue() {
		tongueCoroutine = StartCoroutine(tongue.ExtendTongue());
	}

	public void TongueGrabbed(GameObject obj) {
		if(tongueCoroutine != null) {
			StopCoroutine(tongueCoroutine);
			tongueCoroutine = null;
		}

		if(obj.GetComponent<Rigidbody2D>() != null) {
			obj.GetComponent<Rigidbody2D>().AddForce(-transform.up * tongueForce / 2);
		}

		StartCoroutine(tongue.RetractTongue());
		body.AddForce(transform.up * tongueForce);

		directionDragged = Vector3.zero;
	}

	public void StartSquish() {
		
		if(squishCoroutine == null) {
			squishCoroutine = StartCoroutine(Squish());
		}
	}

	public IEnumerator Squish() {
		Vector3 startScale = transform.localScale;
		Vector3 endScale = new Vector3(startScale.x * 1.1f, startScale.y * 0.9f, startScale.z);

		float timer = 0;
		float duration = 0.3f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);

			yield return waiter;
		}

		transform.localScale = startScale;
		squishCoroutine = null;
	}

	public void StartedMoving() {
		moving = true;

		if(!face.IsExpressing()) {
			face.SetFaceSprite("going");
		}

		ExtendLegs();
	}
	public void StoppedMoving() {
		moving = false;

		if(!dizzy) {
			face.ChangeExpression("stopped", 1.5f);
		}
		
		Unready();

		RetractLegs();

		body.StopMovement();
	}

	public bool GetMoving() {
		return moving;
	}

	public void StartedDizziness() {
		dizzy = true;

		face.SetFaceSprite("dizzy");
	}

	public void StoppedDizziness() {
		dizzy = false;

		face.DetermineFaceState();
	}

	public bool GetDizzy() {
		return dizzy;
	}

	public void ExtendLegs() {
		for(int i = 0; i < legList.Count; i++) {
			if(i < legList.Count / 2) {
				legList[i].size = new Vector2(legList[i].size.x, athlete.athleteType.frontLegMin);
			} else {
				legList[i].size = new Vector2(legList[i].size.x, athlete.athleteType.backLegMin);
			}
		}
	}

	public void RetractLegs() {
		for(int i = 0; i < legList.Count; i++) {
			if(i < legList.Count / 2) {
				legList[i].size = new Vector2(legList[i].size.x, athlete.athleteType.frontLegRest);
			} else {
				legList[i].size = new Vector2(legList[i].size.x, athlete.athleteType.backLegRest);
			}
		}
	}

	public IEnumerator RunInPlace() {
		
		float rando = 0.05f + (Random.value / 3f);
		yield return new WaitForSeconds(rando);

		bool running = true;

		SpriteRenderer leftLeg = legList[0];
		SpriteRenderer rightLeg = legList[1];

		bool left = (Random.value > 0.5f);
		int countTillFlip = Random.Range(10, 18);
		int count = countTillFlip;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(running) {
			float step = (countTillFlip - count) / (float)countTillFlip;

			/*
			if(step <= 1) {
				if(left) {
					leftLeg.size = new Vector2(leftLeg.size.x, Mathf.Lerp(legRest, legMin, step));
				} else {
					rightLeg.size = new Vector2(rightLeg.size.x, Mathf.Lerp(legRest, legMin, step));
				}
			} else {
				if(left) {
					leftLeg.size = new Vector2(leftLeg.size.x, Mathf.Lerp(legMin, legRest, step - 1));
				} else {
					rightLeg.size = new Vector2(rightLeg.size.x, Mathf.Lerp(legMin, legRest, step - 1));
				}
			}
			*/

			count--;

			if(count == 0) {

			} else if(count <= -countTillFlip) {
				count = countTillFlip;
				left = !left;
			}

			yield return waiter;
		}
	}

	public void FinishMatch() {
		//if(GetTeam().
		//If team won, victory face, else defeat face
		if(athlete.GetTeam().wonTheGame) {
			face.SetFaceSprite("victory");
		} else {
			face.SetFaceSprite("defeat");
		}

		athlete.moodFace = face.GetFaceSprite();
	}
}
