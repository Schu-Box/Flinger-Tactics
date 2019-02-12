using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.EventSystems;

public class AthleteController : MonoBehaviour{

	public static bool muteAthletes = true;

	private Athlete athlete;

	//Variables
	private float tongueForce = 600f;
	
	private float tailStretch = 15;

	//Body Parts
	public GameObject tailBody;
	private SpriteRenderer tailBodyRenderer;
	private float tailBodyRestLength;
	public GameObject tailTip;
	private SpriteRenderer tailTipRenderer;
	private float tailTipRestLength;
	private Rigidbody2D rb;
	private SpriteRenderer spriteRenderer;
	public List<SpriteRenderer> legList;

	private Face face;
	private Tongue tongue;

	private GameController gameController;
	private AudioSource audioSource;

	private Vector3 directionDragged = Vector3.zero;

	private Coroutine squishCoroutine;
	private Coroutine runningCoroutine;
	private Coroutine tongueCoroutine;

	private bool disabledInteraction = false;
	private bool beingDragged = false;
	private bool moving = false;
	private bool ready = false;
	private bool dizzy = false;
	private float expressionTimer = 0f;

	private float legRest = 10f;
	private float legMin = 10.5f;
	private float legMax = 12f;

	private Vector2 lastVelocity;

	public void SetAthlete(Athlete a) {
		athlete = a;
		
		gameController = FindObjectOfType<GameController>();
		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		audioSource = GetComponent<AudioSource>();

		face = GetComponentInChildren<Face>();
		tongue = GetComponentInChildren<Tongue>();

		tailBodyRenderer = tailBody.GetComponent<SpriteRenderer>();
		tailBodyRestLength = tailBodyRenderer.size.y;

		tailTipRenderer = tailTip.GetComponent<SpriteRenderer>();
		tailTipRestLength = tailTipRenderer.size.y;

        face.GetComponent<SpriteRenderer>().color = athlete.skinColor;
        tailBody.GetComponent<SpriteRenderer>().color = athlete.skinColor;
        for(int l = 0; l < legList.Count; l++) {
            legList[l].GetComponent<SpriteRenderer>().color = athlete.skinColor;
        }

		spriteRenderer.color = athlete.GetTeam().color;
		tailTipRenderer.color = athlete.GetTeam().color;


		a.SetBodyPartSprite("body", spriteRenderer.sprite);
		a.SetBodyPartSprite("face", face.GetComponent<SpriteRenderer>().sprite);
		a.SetBodyPartSprite("tailBody", tailBodyRenderer.sprite);
		a.SetBodyPartSprite("tailTip", tailTipRenderer.sprite);
		
		List<Sprite> legSpriteList = new List<Sprite>();
		for(int i = 0; i < legList.Count; i++) {
			legSpriteList.Add(legList[i].GetComponent<SpriteRenderer>().sprite);
		}
		a.SetMultipleBodyPartSprites("legs", legSpriteList);
	}

	public Athlete GetAthlete() {
		return athlete;
	}

	void FixedUpdate() {

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
					if(!dizzy) {
						StoppedMoving();
					}
				}
			}
		}
		lastVelocity = newVelocity;

		if(moving) {
			float step = newVelocity.magnitude / 2;
			float newLegLength = Mathf.Lerp(legMin, legMax, step);

			for(int i = 0; i < legList.Count; i++) {
				legList[i].size = new Vector2(legList[i].size.x, newLegLength);
			}
		}

		if(expressionTimer > 0f) {
			//Do nothing while the coroutine goes
		} else {
			if(Mathf.Abs(rb.angularVelocity) > 60f) {
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

	private void OnCollisionEnter2D(Collision2D collision) {
		
		PlayNoise();
		
		if(collision.gameObject.CompareTag("Athlete")){
			athlete.IncreaseStat("bumps");

			if(!dizzy) {
				if(collision.gameObject.GetComponent<AthleteController>().athlete.GetTeam() == athlete.GetTeam()) { //Teammate
					StartCoroutine(face.ChangeExpression("bumpedteam", 1.5f));
				} else { //Opponent
					StartCoroutine(face.ChangeExpression("bumpedenemy", 1.5f));
				}
			}
		} else {
			if(collision.gameObject.CompareTag("Ball")) {
				athlete.IncreaseStat("touches");
			} else if(collision.gameObject.CompareTag("Bumper")) {
				athlete.IncreaseStat("bounces");
			}

			if(!dizzy) {
				StartCoroutine(face.ChangeExpression("bumped", 1f));
			}
		}

		//StartSquish();
	}

	private void PlayNoise() {
		//audioSource.clip = gameController.sounds_Bwomp[Random.Range(0, gameController.sounds_Bwomp.Count)];
		if(audioSource.clip != null && !muteAthletes) {
			audioSource.Play();
		}
	}

	public void GreyOutAthlete() {
		disabledInteraction = true;

		Color teamColor = athlete.GetTeam().color;
		Color skinColor = athlete.skinColor;
		Color targetColor = Color.black;
		float lerpValue = 0.4f;

		spriteRenderer.color = Color.Lerp(teamColor, targetColor, lerpValue);
		tailTipRenderer.color = Color.Lerp(teamColor, targetColor, lerpValue);

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = Color.Lerp(skinColor, targetColor, lerpValue);
		}
		face.SetFaceColor(Color.Lerp(skinColor, targetColor, lerpValue));
		tailBodyRenderer.color = Color.Lerp(skinColor, targetColor, lerpValue);

		tongue.SetTongueColor(Color.Lerp(athlete.tongueColor, targetColor, lerpValue));
	}

	public void RestoreAthleteColor() {
		disabledInteraction = false;

		Color teamColor = athlete.GetTeam().color;
		Color skinColor = athlete.skinColor;

		spriteRenderer.color = teamColor;
		tailTipRenderer.color = teamColor;

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = skinColor;
		}
		face.SetFaceColor(skinColor);
		tailBodyRenderer.color = skinColor;

		tongue.SetTongueColor(athlete.tongueColor);
	}

	/*
	public void OnMouseEnter() {
		if(!gameController.turnActive && !gameController.matchOver) {
			if(!ready && !beingDragged) {
				faceRenderer.sprite = gameController.face_Peeking;
			}
		}
	}

	public void OnMouseExit() {
		if(!gameController.turnActive && !gameController.matchOver) {
			if(!ready && !beingDragged && expressionTimer <= 0f) {
				faceRenderer.sprite = gameController.face_Neutral;
			}
		}
	}
	*/

	public void OnMouseDown() { //Clicked athlete
		if(!disabledInteraction && gameController.matchStarted && !gameController.turnActive && !gameController.matchOver) {
			face.SetFaceSprite("clicked");

			beingDragged = true;

			ExtendLegs();

			Unready();
		}
	}

	public void OnMouseDrag() {
		if(!disabledInteraction && gameController.matchStarted && !gameController.turnActive && !gameController.matchOver) {
			Vector3 flatDirection = transform.position;
			flatDirection.z = 0f;
			Vector3 direction = (flatDirection - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)));

			Vector3 normalizedDirection = direction.normalized;
			float zRotato = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler(0, 0, zRotato - 90);

			if(direction.magnitude > athlete.minPull) {
				if(direction.magnitude >= athlete.maxPull) {
					direction = Vector3.ClampMagnitude(direction, athlete.maxPull);
				}

				if(athlete.HasPoleTail()) {
					float addedLength = (((Vector2)direction).magnitude - athlete.minPull) * tailStretch;
					tailTipRenderer.size = new Vector2(tailTipRenderer.size.x, tailTipRestLength + addedLength);
					tailBodyRenderer.size = new Vector2(tailBodyRenderer.size.x, tailBodyRestLength + (addedLength / 2));
				} else {
					float addedLength = (((Vector2)direction).magnitude - athlete.minPull) * tailStretch;
					tailTipRenderer.size = new Vector2(tailTipRenderer.size.x, tailTipRestLength + (addedLength / 2));
				}

				if(tongue.GetTongueOut()) {
					tongue.HideTongue();
				}
			} else {
				direction = Vector3.zero;

				ResetTail();

				if(!tongue.GetTongueOut()) {
					tongue.RevealTongue();
				}
			}

			directionDragged = direction;
		}
	} 

	public void OnMouseUp() {
		if(gameController.matchStarted && !gameController.turnActive && !gameController.matchOver) {
			beingDragged = false;

			if(directionDragged != Vector3.zero) { //Athlete will move
				ReadyUp();
			} 
			
			/*
			else {
				CancelReady();
			}
			*/
		}
	}

	public void ReadyUp() {
		ready = true;

		face.SetFaceSprite("ready");

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
			FlickAthlete();
		} else {
			if(tongue.GetTongueOut()) {
				FlickTongue();
			}
		}
	}

	public void FlickAthlete() {
		Vector2 minDirectionDrag = directionDragged.normalized * athlete.minPull;
		Vector2 adjustedDirection = (Vector2)directionDragged - minDirectionDrag;
		if(adjustedDirection.magnitude < 0) {
			adjustedDirection = Vector2.zero;
		}

		Vector2 force = adjustedDirection * athlete.flingForce;
		rb.AddForce(force);

		directionDragged = Vector3.zero;

		ResetTail();
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
		rb.AddForce(transform.up * tongueForce);

		directionDragged = Vector3.zero;
	}

	private void ResetTail() {
		tailTipRenderer.size = new Vector2(tailTipRenderer.size.x, tailTipRestLength);
		tailBodyRenderer.size = new Vector2(tailBodyRenderer.size.x, tailBodyRestLength);
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

		face.SetFaceSprite("going");

		ExtendLegs();
	}
	public void StoppedMoving() {
		moving = false;

		if(!dizzy) {
			StartCoroutine(face.ChangeExpression("stopped", 1.5f));
		}
		
		Unready();

		RetractLegs();

		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0f;
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
			legList[i].size = new Vector2(legList[i].size.x, legMin);
		}
	}

	public void RetractLegs() {
		for(int i = 0; i < legList.Count; i++) {
			legList[i].size = new Vector2(legList[i].size.x, legRest);
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
	}
}
