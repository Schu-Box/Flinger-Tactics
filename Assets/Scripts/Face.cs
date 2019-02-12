using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour {

	public bool faceChanges = true;

	public Sprite face_Asleep;
	public Sprite face_Clicked;
	public Sprite face_Ready;
	public Sprite face_Going;
	public Sprite face_Dizzy;
	public Sprite face_Bumped;
	public Sprite face_BumpedEnemy;
	public Sprite face_BumpedTeammate;
	public Sprite face_Stopped;
	public Sprite face_Victory;
	public Sprite face_Defeat;

	private GameController gameController;
	private AthleteController athleteController;

	private SpriteRenderer spriteRenderer;

	void Start() {
		gameController = FindObjectOfType<GameController>();
		athleteController = GetComponentInParent<AthleteController>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		/*
		Vector3 randomFaceFlip = gameObject.transform.localEulerAngles;
		randomFaceFlip.y = (Random.Range(0, 2) * 180f);
		gameObject.transform.localEulerAngles = randomFaceFlip;
		*/

		SetFaceSprite("sleep");
	}

	public void SetFaceSprite(string faceSprite) {
		if(faceChanges) {

			switch(faceSprite.ToLower()) {
				case "sleep":
					spriteRenderer.sprite = face_Asleep;
					break;
				case "clicked":
					spriteRenderer.sprite = face_Clicked;
					break;
				case "ready":
					spriteRenderer.sprite = face_Ready;
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
					Debug.Log("Expression does not exist");
					spriteRenderer.sprite = face_Asleep;
					break;
			}

		}
	}
	
	public void SetFaceColor(Color color) {
		spriteRenderer.color = color;
	}

	public IEnumerator ChangeExpression(string expression, float duration) {
		
		SetFaceSprite(expression);

		float timer = 0f;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer += Time.deltaTime;
			yield return waiter;
		}

		DetermineFaceState();
	}	

	public void DetermineFaceState() {
		if(!gameController.matchOver) {
			if(athleteController.GetDizzy()) {
				SetFaceSprite("dizzy");
			} else {
				if(athleteController.GetMoving()) {
					SetFaceSprite("going");
				} else {
					if(athleteController.GetReady()) { 
						SetFaceSprite("ready");
					} else {
						SetFaceSprite("sleep");
					}
				}
			}
		} else {

			if(athleteController.GetAthlete().GetTeam().wonTheGame) {
				SetFaceSprite("victory");
			} else {
				SetFaceSprite("defeat");
			}
		}
	}
}
