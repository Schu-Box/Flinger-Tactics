using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
//using UnityEngine.EventSystems;

public class AthleteController : MonoBehaviour{

	private Athlete athlete;

	public bool crowdAthlete = false;

	//Variables
	private float tongueForce = 600f;
	
	private float tailStretch = 15;

	//Body Parts
	public List<SpriteRenderer> legList;
	public SpriteRenderer jersey;
	public SpriteRenderer jerseyStripes;

	private Body body;
	private TailBody tailBody;
	private TailTip tailTip;
	private Face face;
	private Tongue tongue;
	private TextMeshPro jerseyText;

	//private GravityField gravityField;
	private ParticleManager particleManager;

	private MatchController matchController;
	private CameraController cameraController;
	private AudioManager audioManager;
	private AudioSource audioSource;
	private FocalObject focalObject;

	private Vector3 directionDragged = Vector3.zero;

	private Coroutine squishCoroutine;
	private Coroutine runningCoroutine;
	private Coroutine tongueCoroutine;

	private ParticleSystem activeParticles;

	private bool instantInteraction = false;
	private bool disabledInteraction = false;
	private bool moving = false;
	private bool ready = false;
	private bool dizzy = false;
	private float expressionTimer = 0f;
	private bool substitute = false;

	private bool spokeThisTurn = false;

	private bool speaking = false;
	private bool paralyzed = false;
	private int turnPhasesTilUnparalyzed = 0;
	private bool paralyzeChargeActive = false;
	

	//Abilities?
	private bool restoresBumperOnHit = false;
	private bool sendsShockwaveOnBumperHit = false;
	private bool paralyzesOnTackle = false;

	private Vector2 lastVelocity;
	private Vector3 originalScale;

	private Bumper lastBumperEntered;
	private QuoteBox currentQuoteBox;

	void Awake() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
		audioSource = GetComponent<AudioSource>();
		focalObject = GetComponent<FocalObject>();

		body = GetComponent<Body>();
		tailBody = GetComponentInChildren<TailBody>();
		tailTip = GetComponentInChildren<TailTip>();
		face = GetComponentInChildren<Face>();
		tongue = GetComponentInChildren<Tongue>();

		jerseyText = GetComponentInChildren<TextMeshPro>();

		SetInstantInteraction(true);

		originalScale = transform.localScale;

		body.SetBody();
		tailBody.SetTailBody();
		SetTailBodyActive(false);
		tailTip.SetTailTip();
		face.SetFace();
		tongue.SetTongue();

		//gravityField = GetComponentInChildren<GravityField>();
		particleManager = FindObjectOfType<ParticleManager>();
	}

	public void SetAthlete(Athlete a) {
		
		if(a == null) { //Set the athlete as a spectator
			athlete = new Athlete();
			float rando = Random.value;
			if(rando > 0.8) {
				athlete.SetTeam(matchController.GetTeam(false));
			} else {
				athlete.SetTeam(matchController.GetTeam(true));
			}

			jerseyText.text = "";
		} else {
			athlete = a;

			jerseyText.text = a.jerseyNumber.ToString();
		}

		body.SetSprite(athlete.athleteData.bodySprite);
		face.SetFaceBase(athlete.athleteData.faceBaseSprite);
		face.SetFaceSprite("neutral");
		
		if(jersey != null) {
			jersey.sprite = athlete.athleteData.athleteJersey;

			if(jerseyStripes != null) {
				jerseyStripes.sprite = athlete.athleteData.athleteJerseyStripes;
				//jerseyStripes.color = a.GetTeam().secondaryColor;
			}
		}

        for(int l = 0; l < legList.Count; l++) {
			/* 
			if(l < legList.Count / 2) { //If it's the first half of legs
				legList[l].GetComponent<SpriteRenderer>().sprite = athlete.athleteData.frontLegSprite;
			} else {
				legList[l].GetComponent<SpriteRenderer>().sprite = athlete.athleteData.backLegSprite;
			*/
			legList[l].GetComponent<SpriteRenderer>().sprite = athlete.athleteData.legSprite;
        }

		RetractLegs();

		RestoreAthleteColor();

		string c = athlete.athleteData.classString;

		switch(c) {
			case "Box":
				restoresBumperOnHit = true;
				break;

			case "Circle":
				sendsShockwaveOnBumperHit = true;
				break;

			case "Triangle":
				paralyzesOnTackle = true;
				break;

			default:
				Debug.Log("Class doesn't exist");
				break;
		};
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
			float legWidth = legList[0].size.x;
			float newlegLength;

			for(int i = 0; i < legList.Count; i++) {
				if(i < legList.Count / 2) {
					newlegLength = Mathf.Lerp(athlete.athleteData.frontLegMin * legWidth, athlete.athleteData.frontLegMax * legWidth, step);
				} else {
					newlegLength = Mathf.Lerp(athlete.athleteData.backLegMin * legWidth, athlete.athleteData.backLegMax * legWidth, step);
				}

				legList[i].size = new Vector2(legList[i].size.x, newlegLength);
			}

			if(paralyzeChargeActive) {
				activeParticles.gameObject.GetComponent<ParticleScript>().AdjustedVelocity(body.GetVelocity());
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
		audioManager.Play("athleteBump");
		audioManager.Play("athleteSqueak");

		IncreaseStat(StatType.Bumps);
		
		if(collision.gameObject.CompareTag("Athlete")){
			//cameraController.AddTrauma(0.18f);
			AthleteController ac = collision.gameObject.GetComponent<AthleteController>();

			if(ac.GetAthlete().GetTeam() != GetAthlete().GetTeam()) {
				if(moving) {
					IncreaseStat(StatType.Tackles);
				}
			}

			if(paralyzesOnTackle && paralyzeChargeActive) {
				ParalyzeAthlete(ac);
			}

			if(collision.gameObject.GetComponent<AthleteController>().athlete.GetTeam() == athlete.GetTeam()) { //Teammate
				face.ChangeExpression("bumpedteam", 2f);
			} else { //Opponent
				face.ChangeExpression("bumpedenemy", 1.5f);
			}
		} else {
			if(collision.gameObject.CompareTag("Ball")) {
				IncreaseStat(StatType.Touches);

				/*
				if(gravityField.IsGravityFieldEnabled()) {
					if(!moving) {
						gravityField.PossessBall(collision.gameObject.GetComponent<BallController>());
						StoppedMoving();
					} else {
						gravityField.StopGravitation(collision.gameObject);
					}
				}
				*/
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

	int setLayer;

	public void SetBodyLayer(int layer) {
		gameObject.layer = layer;
		setLayer = layer;
	}

	public void ResetBodyLayer() {
		gameObject.layer = setLayer;
	}

	public int GetLayer() {
		return setLayer;
	}

	public void IgnoreRaycasts(bool ignoring) {
		if(ignoring) {
			tailTip.gameObject.layer = 2;

			//gameObject.layer = 2;
		} else {
			tailTip.gameObject.layer = 0;
			//SetBodyLayer(setLayer);
		}
	}

	public void DimAthleteColor() {
		Color teamColor = athlete.GetTeam().primaryColor;
		Color skinColor = athlete.skinColor;
		Color darkerSkinColor = athlete.bodySkinColor;
		Color targetColor = Color.black;
		float lerpValue = 0.5f;

		body.SetColor(Color.Lerp(darkerSkinColor, targetColor, lerpValue));
		tailTip.SetColor(Color.Lerp(darkerSkinColor, targetColor, lerpValue));

		face.SetColor(Color.Lerp(skinColor, targetColor, lerpValue));
		tailBody.SetColor(Color.Lerp(skinColor, targetColor, lerpValue));

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = Color.Lerp(skinColor, targetColor, lerpValue);
		}

		tongue.SetTongueColor(Color.Lerp(athlete.tongueColor, targetColor, lerpValue));

		if(jersey != null) {
			jersey.color = Color.Lerp(teamColor, targetColor, lerpValue);
			jerseyText.color = Color.Lerp(Color.white, targetColor, lerpValue);

			if(jerseyStripes != null) {
				jerseyStripes.color = Color.Lerp(athlete.GetTeam().secondaryColor, targetColor, lerpValue);
			}
		}
	}

	public void EnableInteraction() { //And Restore Color
		disabledInteraction = false;
	}

	public void RestoreAthleteColor() {
		Color teamColor = athlete.GetTeam().primaryColor;
		Color skinColor = athlete.skinColor;
		Color darkerSkinColor = athlete.bodySkinColor;

		body.SetColor(darkerSkinColor);
		tailTip.SetColor(darkerSkinColor);

		face.SetColor(skinColor);
		
		if(tailBody != null) {
			tailBody.SetColor(skinColor);
		}

		for(int i = 0; i < legList.Count; i++) {
			legList[i].GetComponent<SpriteRenderer>().color = skinColor;
		}

		tongue.SetTongueColor(athlete.tongueColor);

		if(jersey != null) {
			jersey.color = teamColor;

			jerseyText.color = Color.white;

			if(jerseyStripes != null) {
				jerseyStripes.color = athlete.GetTeam().secondaryColor;
			}
		}
	}

	public bool IsDisabled() {
		return disabledInteraction;
	}

	public void MouseEnter() {
		if(!crowdAthlete) {
			/*
			if(matchController.GetAthleteBeingHovered() != this) {
				matchController.AthleteHovered(this);
			} else {
				Debug.Log("Athlete already hovered");
			}
			*/
			matchController.AthleteHovered(this);
		}
	}

	public void MouseExit() {
		if(!crowdAthlete) {
			if(matchController.GetAthleteBeingHovered() != null) {
				matchController.AthleteUnhovered(this);
				if(!matchController.GetAthleteBeingDragged() && !moving && !matchController.IsTurnActive() && !paralyzed && !speaking && !matchController.GetMatchEnded()) {
					//I should consider moving this to be consistent with AthleteHovered
					face.SetFaceSprite("neutral");
				}
			}
		}
	}

	public void MouseDrag() {
		if(!disabledInteraction && !athlete.GetTeam().computerControlled) {
			Vector3 flatDirection = transform.position;
			flatDirection.z = 0f;
			Vector3 direction = (flatDirection - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z)));
			TailAdjusted(direction);
		}
	}

	public void MouseClick() {
		//face.SetFaceSprite("dragging");
		if(!crowdAthlete) {
			if(!disabledInteraction) {
				matchController.SetAthleteBeingDragged(this);

				ExtendLegs();

				//matchController.DetermineGravityFields(this);
				if(paralyzesOnTackle) {
					StartParalyzeCharge();
				}
			}
		}
	}

	public void TailAdjusted(Vector3 direction) {
		SetTailBodyActive(true); //This coule be handled a bit better

		if(direction.magnitude > athlete.minPull) {
			//gravityField.DisableGravityField();

			if(direction.magnitude >= athlete.maxPull) {
				direction = Vector3.ClampMagnitude(direction, athlete.maxPull);
			}

			Vector3 normalizedDirection = direction.normalized;	//Rotate the athlete to face opposite of the mouse
			float targetRotation = (Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg + 90);
			//float startRotation = transform.rotation.eulerAngles.z;
			transform.rotation = Quaternion.Euler(0, 0, targetRotation);

			tailTip.AdjustTailPosition(direction.magnitude);

			face.SetFaceSprite("dragging");

			if(activeParticles != null) {
				activeParticles.GetComponent<ParticleScript>().AdjustTailStretch(direction.magnitude - athlete.minPull);
			}
		} else {
			direction = Vector3.zero;

			ResetTail();

			face.SetFaceSprite("hovered");
		}

		directionDragged = direction;
	}

	/*
	public void AdjustTailAndFling(Vector3 target, float percentFlingForce) {
		matchController.SetAthleteBeingDragged(true);

		ExtendLegs();
		Vector2 direction = target - transform.position;

		if(direction.magnitude > athlete.minPull) {
			if(direction.magnitude >= athlete.maxPull) {
				direction = Vector3.ClampMagnitude(direction, athlete.maxPull);
			}

			Vector3 normalizedDirection = direction.normalized;	//Rotate the athlete to face opposite of the mouse
			float targetRotation = (Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg + 90);
			//float startRotation = transform.rotation.eulerAngles.z;
			transform.rotation = Quaternion.Euler(0, 0, targetRotation);

			tailTip.AdjustTailPosition(direction.magnitude);

			if(tongue.GetTongueOut()) {
				tongue.HideTongue();
			}

			face.SetFaceSprite("dragging");
		} else {
			Debug.Log("Why are you even calling this function then with that weak ass tail force?");
		}

		directionDragged = direction;

		StartAction();
	}
	*/

	public void ResetTail() {
		tailTip.AdjustTailPosition(0f);
	}

	public void Unclicked() {
		if(!crowdAthlete) {
			if(!disabledInteraction) {
				if(matchController.GetAthleteBeingDragged() == this) {
					matchController.SetAthleteBeingDragged(null);
				}

				if(directionDragged != Vector3.zero) { //Athlete will move
					StartAction();
					/*
					if(instantInteraction) {
						
					} else {
						ReadyUp();
					}
					*/
				} else {
					face.DetermineFaceState();

					RetractLegs();

					//matchController.DetermineGravityFields(null);

					if(paralyzesOnTackle) {
						StopParalyzeCharge();
					}
				}
			}
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

		matchController.BeginActiveTurnPhase(this);

		if(directionDragged != Vector3.zero) {
			FlingAthlete();
		} else {
			if(tongue.GetTongueOut()) {
				FlickTongue();
			}
		}
	}

	public void FlingAthlete() {
		audioManager.Play("athleteYipee");

		IncreaseStat(StatType.Flings);

		Vector2 minDirectionDrag = directionDragged.normalized * athlete.minPull;
		Vector2 adjustedDirection = (Vector2)directionDragged - minDirectionDrag;
		if(adjustedDirection.magnitude < 0) {
			adjustedDirection = Vector2.zero;
		}

		//Debug.Log(adjustedDirection);

		Vector2 force = adjustedDirection * athlete.flingForce;
		body.AddForce(force);

		directionDragged = Vector3.zero;

		ResetTail();

		if(activeParticles != null) {
			activeParticles.GetComponent<ParticleScript>().AdjustTailStretch(0);
		}
	}

	public void PrepareForNextTurn(bool ourTurn) {
		if(ourTurn && !paralyzed) {
			EnableInteraction();
            IgnoreRaycasts(false);
            RestoreAthleteColor();
            DisableBody();
		} else {
			DisableInteraction();
            IgnoreRaycasts(true);
            DimAthleteColor();
            EnableBody();
		}

		if(paralyzeChargeActive) {
			StopParalyzeCharge();
		}
	}

	public void BeginActiveTurn() {
		if(paralyzed) {
			if(turnPhasesTilUnparalyzed > 0) {
				turnPhasesTilUnparalyzed--;
			} else {
				paralyzed = false;

				face.ChangeExpression("woken", 1f);
			}
		}

		DisableInteraction();
        EnableBody();
        RestoreAthleteColor();

        SetTailBodyActive(false);
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

		focalObject.TriggerWatchers();

		/*
		if(gravityField.IsGravityFieldActive()) {
			gravityField.DisableGravityField();
		}
		*/
	}
	public void StoppedMoving() {
		moving = false;

		if(!dizzy) {
			face.ChangeExpression("stopped", 1.5f);
		}

		if(paralyzeChargeActive) {
			StopParalyzeCharge();
		}
		
		Unready();

		RetractLegs();

		body.StopMovement();

		focalObject.StopWatchers();
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
		float standardWidth = legList[0].size.x;
		for(int i = 0; i < legList.Count; i++) {
			if(i < legList.Count / 2) {
				legList[i].size = new Vector2(standardWidth, athlete.athleteData.frontLegMin * standardWidth);
			} else {
				legList[i].size = new Vector2(standardWidth, athlete.athleteData.backLegMin * standardWidth);
			}
		}
	}

	public void RetractLegs() {
		float standardWidth = legList[0].size.x;
		for(int i = 0; i < legList.Count; i++) {
			if(i < legList.Count / 2) {
				legList[i].size = new Vector2(standardWidth, athlete.athleteData.frontLegRest * standardWidth);
			} else {
				legList[i].size = new Vector2(standardWidth, athlete.athleteData.backLegRest * standardWidth);
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

	public void SetFaceSprite(string faceSpriteID) {
		face.SetFaceSprite(faceSpriteID);
	}

	public Face GetFace() {
		return face;
	}

	public IEnumerator RemoveAthleteFromField(Vector3 goalCenter) {
        //Fade the athlete out first - similar to pokeball effect

		StoppedMoving();
		//body.DisableBody();

		focalObject.RemoveAllWatchers();

		Vector3 originalPosition = transform.position;
		Vector3 absorbPosition = goalCenter;
		absorbPosition.y = originalPosition.y;

		yield return new WaitForSeconds(0.2f);

		//SetAllSpritesWhite();

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 1f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.position = Vector3.Lerp(originalPosition, absorbPosition, timer/duration);

			transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, timer/duration);

			yield return waiter;
		}

        gameObject.SetActive(false);
        gameObject.transform.position = Vector2.zero;
    }

	public void PrepareSubstitute(Vector2 spawnPosition, Vector3 spawnAngle, Transform chair) {
		transform.localPosition = spawnPosition;
		transform.eulerAngles = spawnAngle;
		transform.localScale = originalScale;
		
		SetTailBodyActive(false);

		substitute = true;
		//
		//AYY This never gets set back to false. Probably causing you issues, bro

		gameObject.layer = 8;

		//GetComponent<SortingGroup>().sortingLayerName = "Focal Athlete";

		//IgnoreRaycasts(true);

		//RestoreAthleteColor();

		DisableInteraction();
		AttachToChair(chair);
	}

	public void AttachToChair(Transform chair) {
		transform.SetParent(chair);
		transform.localPosition = Vector2.zero;

		chair.GetComponent<SubstituteChair>().SetCurrentSubstitute(this);
	}

	public void AddForce(Vector2 forceAdded) {
		moving = true;
		body.AddForce(forceAdded);
	}

	public void EnteredBumper(Bumper bumper) {
		lastBumperEntered = bumper;
	}

	public void ExitedBumper(Bumper bumper) {
		//Might not be able to do this so quickly
		//lastBumperEntered = null;
	}

	public Bumper GetLastBumperEntered() {
		return lastBumperEntered;
	}	

	public void SetTailBodyActive(bool setActive) {
		tailBody.gameObject.SetActive(setActive);
	}

	/*
	public void EnableGravityField() {
		gravityField.EnableGravityField();
	}
	
	public void ActivateGravityField() {
		gravityField.ActivateGravityField();
	}
	
	public void DisableGravityField() {
		gravityField.DisableGravityField();
	}

	public bool IsGravityFieldEnabled() {
		return gravityField.IsGravityFieldEnabled();
	}
	*/

	public void SetSpokeThisTurn(bool spoke) {
		spokeThisTurn = spoke;
	}

	public bool GetSpokeThisTurn() {
		return spokeThisTurn;
	}

	public bool GetSendsShockwaves() {
		return sendsShockwaveOnBumperHit;
	}

	public void StartParalyzeCharge() {
		paralyzeChargeActive = true;

		particleManager.PlayCharged(this);
	}

	public void ParalyzeAthlete(AthleteController ac) {
		ac.paralyzed = true;
		ac.turnPhasesTilUnparalyzed = 1;

		IncreaseStat(StatType.Knockouts);

		StopParalyzeCharge();
		particleManager.PlayDischarge(this);

		ac.SetFaceSprite("paralyzed");
	}

	public void StopParalyzeCharge() {
		paralyzeChargeActive = false;

		particleManager.StopCharged(this);
	}

	public bool GetParalyzed() {
		return paralyzed;
	}

	public bool GetRestoresBumper() {
		return restoresBumperOnHit;
	}

	public void SetSpeaking(bool isSpeaking) {
		speaking = isSpeaking;
	}

	public bool GetSpeaking() {
		return speaking;
	}	

	public void SetCurrentQuoteBox(QuoteBox qb) {
		currentQuoteBox = qb;
	}

	public QuoteBox GetCurrentQuoteBox() {
		return currentQuoteBox;
	}

	public void SetActiveParticles(ParticleSystem ps) {
		activeParticles = ps;
	}

	public ParticleSystem GetActiveParticles() {
		return activeParticles;
	}
}
