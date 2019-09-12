using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour {

	public bool faceChanges = true;

	public Sprite face_Neutral;
	public Sprite face_Hovered;
	public Sprite face_Dragging;
	public Sprite face_Going;
	public Sprite face_Dizzy;
	public Sprite face_Bumped;
	public Sprite face_BumpedEnemy;
	public Sprite face_BumpedTeammate;
	public Sprite face_Stopped;
	public Sprite face_Victory;
	public Sprite face_Defeat;
	public Sprite face_Paralyzed;
	public Sprite face_Woken;
	public Sprite face_Speaking;

	public List<Sprite> speakingSprites;

	private MatchController matchController;
	private AthleteController athleteController;

	private SpriteRenderer spriteRenderer;
	private SpriteRenderer faceBaseSpriteRenderer;

	private Coroutine expressionCoroutine;

	public void SetFace() {
		matchController = FindObjectOfType<MatchController>();
		athleteController = GetComponentInParent<AthleteController>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		faceBaseSpriteRenderer = transform.parent.GetChild(0).GetComponent<SpriteRenderer>();

		Vector3 randomFaceFlip = gameObject.transform.localEulerAngles;
		randomFaceFlip.y = (Random.Range(0, 2) * 180f);
		gameObject.transform.localEulerAngles = randomFaceFlip;

		SetFaceSprite("neutral");
	}


	public void SetFaceBase(Sprite sprite) {
		faceBaseSpriteRenderer.sprite = sprite;
	}

	public void SetColor(Color color) {
		faceBaseSpriteRenderer.color = color;
	}

	public void SetMaterial(Shader material) {
		faceBaseSpriteRenderer.material.shader = material;
		//spriteRenderer
	}

	public void SetFaceSprite(Sprite faceSprite) {
		spriteRenderer.sprite = faceSprite;
	}

	public void SetFaceSprite(string faceSprite) {
		if(faceChanges) {
			Sprite newFace;
			switch(faceSprite.ToLower()) {
				case "neutral":
					newFace = face_Neutral;
					break;
				case "hovered":
				case "watching":
					newFace = face_Hovered;
					break;
				case "dragging":
					newFace = face_Dragging;
					break;
				case "going":
				case "happy":
					newFace = face_Going;
					break;
				case "dizzy":
					newFace = face_Dizzy;
					break;
				case "bumped":
					newFace = face_Bumped;
					break;
				case "bumpedenemy":
					newFace = face_BumpedEnemy;
					break;
				case "bumpedteam":
					newFace = face_BumpedTeammate;
					break;
				case "stopped":
				case "sad":
					newFace = face_Stopped;
					break;
				case "victory":
					newFace = face_Victory;
					break;
				case "defeat":
					newFace = face_Defeat;
					break;
				case "paralyzed":
					newFace = face_Paralyzed;
					break;
				case "woken":
					newFace = face_Woken;
					break;
				case "speaking":
					newFace = face_Speaking;
					break;
				default:
					Debug.Log(faceSprite + " expression does not exist");
					newFace = face_Neutral;
					break;
			}

			if(athleteController.GetParalyzed()) { //Overrides any other face if the athlete is paralyzed.
				newFace = face_Paralyzed;
			}

			spriteRenderer.sprite = newFace;

			if(athleteController.GetAthlete() != null) {
				athleteController.GetAthlete().moodFace = newFace;
			}
		}
	}

	public Sprite GetFaceSprite() {
		return spriteRenderer.sprite;
	}

	public void ChangeExpression(string expression, float duration) {

		if(!athleteController.GetParalyzed()) {
			if(expressionCoroutine != null) { //Athlete is currently already expressing something
				//expressionCoroutine = StartCoroutine(Express(expression, duration));
			} else {
				if(expression != "speaking") {
					expressionCoroutine = StartCoroutine(Express(expression, duration));
				} else {
					expressionCoroutine = StartCoroutine(Speak(duration));
				}
			}
		}
	}

	private IEnumerator Express(string expression, float duration) {
		SetFaceSprite(expression);

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;
			yield return waiter;
		}

		DetermineFaceState();

		expressionCoroutine = null;
	}	

	private IEnumerator Speak(float duration) {
		SetFaceSprite(speakingSprites[0]);

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			float cycleTime = duration / 2f;
			float spriteIntervals = cycleTime / speakingSprites.Count;

			float adjustedTime = timer % cycleTime;

			int step = 0;
			float timeRemaining = adjustedTime;
			for(int i = 0; i < speakingSprites.Count; i++) {
				if(timeRemaining > spriteIntervals) {
					step++;

					timeRemaining -= spriteIntervals;
				} else { //else it's not time to move to the next sprite yet
					break;
				}
			}

			SetFaceSprite(speakingSprites[step]);

			yield return waiter;
		}

		DetermineFaceState();

		expressionCoroutine = null;
	} 

	public bool IsExpressing() {
		if(expressionCoroutine != null) {
			return true;
		} else {
			return false;
		}
	}

	public void DetermineFaceState() {
		if(athleteController.crowdAthlete) {
			SetFaceSprite("watching");
		} else {
			if(athleteController.GetParalyzed()) {
				SetFaceSprite("paralyzed");
			} else {
				if(athleteController.GetDizzy()) {
						SetFaceSprite("dizzy");
					} else {
						if(athleteController.GetMoving()) {
							SetFaceSprite("going");
						} else {
							/*
							if(athleteController.GetReady()) { 
								SetFaceSprite("ready");
							} else {
								SetFaceSprite("neutral");
							}
							*/
							if(matchController.GetAthleteBeingHovered() == athleteController) {
								SetFaceSprite("hovered");
							} else {
								SetFaceSprite("neutral");
							}
						}
					}


				if(matchController != null && matchController.GetMatchEnded()) {
					if(athleteController.GetAthlete().GetTeam().GetCurrentMatchData().GetTeamMatchData(athleteController.GetAthlete().GetTeam()).DidTeamWin()) {
						SetFaceSprite("victory");
					} else {
						SetFaceSprite("defeat");
					}
				}
			}
		}
	}
}
