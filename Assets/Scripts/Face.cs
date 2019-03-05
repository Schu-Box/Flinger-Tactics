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

	public void SetFaceSprite(string faceSprite) {
		if(faceChanges) {

			switch(faceSprite.ToLower()) {
				case "neutral":
					spriteRenderer.sprite = face_Neutral;
					break;
				case "hovered":
					spriteRenderer.sprite = face_Hovered;
					break;
				case "dragging":
					spriteRenderer.sprite = face_Dragging;
					break;
				case "going":
					spriteRenderer.sprite = face_Going;
					break;
				case "dizzy":
					spriteRenderer.sprite = face_Dizzy;
					break;
				case "bumped":
					spriteRenderer.sprite = face_Bumped;
					break;
				case "bumpedenemy":
					spriteRenderer.sprite = face_BumpedEnemy;
					break;
				case "bumpedteam":
					spriteRenderer.sprite = face_BumpedTeammate;
					break;
				case "stopped":
					spriteRenderer.sprite = face_Stopped;
					break;
				case "victory":
					spriteRenderer.sprite = face_Victory;
					break;
				case "defeat":
					spriteRenderer.sprite = face_Defeat;
					break;
				default:
					Debug.Log(faceSprite + " expression does not exist");
					spriteRenderer.sprite = face_Neutral;
					break;
			}

		}
	}

	public Sprite GetFaceSprite() {
		return spriteRenderer.sprite;
	}

	public void ChangeExpression(string expression, float duration) {

		if(expressionCoroutine != null) { //Athlete is currently already expressing something
			//expressionCoroutine = StartCoroutine(Express(expression, duration));
		} else {
			expressionCoroutine = StartCoroutine(Express(expression, duration));
		}
	}

	public IEnumerator Express(string expression, float duration) {
		
		SetFaceSprite(expression);

		float timer = 0f;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer += Time.deltaTime;
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
					SetFaceSprite("neutral");
				}
			}


		if(matchController != null && matchController.GetMatchEnded()) {
			if(athleteController.GetAthlete().GetTeam().wonTheGame) {
				SetFaceSprite("victory");
			} else {
				SetFaceSprite("defeat");
			}
		}
	}
}
