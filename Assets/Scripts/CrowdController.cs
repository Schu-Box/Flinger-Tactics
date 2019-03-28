using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour {

	public GameObject stepFillHolder;
	private List<SpriteRenderer> stepFills = new List<SpriteRenderer>();
	private Color stepColor;

    public GameObject crowdPrefab;

	private MatchController matchController;
	private AudioManager audioManager;

	private List<AthleteController> crowdList = new List<AthleteController>();


	void Start() {
		matchController = FindObjectOfType<MatchController>();
		audioManager = FindObjectOfType<AudioManager>();

		for(int i = 0; i < stepFillHolder.transform.childCount; i++) {
			stepFills.Add(stepFillHolder.transform.GetChild(i).GetComponent<SpriteRenderer>());
		}

		if(stepFills.Count > 0) {
			stepColor = stepFills[0].color;
		}
	}

	public void SetStadiumSeats(Team team) {
		for(int i = 0; i < transform.childCount; i++) {
			for(int j = 0; j < transform.GetChild(i).childCount; j++) {
				GameObject seatObj = transform.GetChild(i).GetChild(j).gameObject;
				seatObj.GetComponent<SpriteRenderer>().color = team.primaryColor;
			}
		}
	}

	public void SetCrowd() {
		for(int i = 0; i < transform.childCount; i++) {
			for(int j = 0; j < transform.GetChild(i).childCount; j++) {
				GameObject seatObj = transform.GetChild(i).GetChild(j).gameObject;

				float rando = Random.value;
				if(rando > 0.7) {
					//Leave the seat empty
				} else {
					GameObject crowdAthlete = Instantiate(crowdPrefab, seatObj.transform.position, seatObj.transform.rotation, seatObj.transform);
					crowdAthlete.GetComponent<AthleteController>().SetAthlete(null);

					crowdList.Add(crowdAthlete.GetComponent<AthleteController>());
				}
			}
		}

		ExpressEmotion("watching");
	}

	public void SetFocus() {
		List<GameObject> focalObjects = new List<GameObject>();

		List<BallController> balls = matchController.GetBallsOnField();
		for(int i = 0; i < balls.Count; i++) {
			focalObjects.Add(balls[i].gameObject);
		}
		List<AthleteController> athletes = matchController.GetAllAthletesOnField();
		for(int i = 0; i < athletes.Count; i++) {
			focalObjects.Add(athletes[i].gameObject);
		}

		for(int i = 0; i < crowdList.Count; i++) {
			float rando = Random.value;
			GameObject chosenObj;
			if(rando > 0.2) { //80% chance they focus on a ball
				chosenObj = focalObjects[Random.Range(0, balls.Count)];
			} else {
				chosenObj = focalObjects[Random.Range(balls.Count, focalObjects.Count)];
			}

			crowdList[i].SetFocus(chosenObj);
		}
	}

	public void StartWatching() {
		for(int i = 0; i < crowdList.Count; i++) {
			crowdList[i].StartWatching();
		}
	}

	public void ExpressEmotion(string emotion) {
		for(int i = 0; i < crowdList.Count; i++) {
			crowdList[i].GetFace().ChangeExpression(emotion, 2f);
		}
	}

	public void ExpressEmotion(string emotion, bool home) {
		Team fromTeam = matchController.GetTeam(home);

		for(int i = 0; i < crowdList.Count; i++) {
			if(crowdList[i].GetAthlete().GetTeam() == fromTeam) {
				crowdList[i].GetFace().ChangeExpression(emotion, 2f);
			}
		}
	}

	public IEnumerator FlashSteps(Color flashColor) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		for(int i = 0; i < stepFills.Count; i++) {
			stepFills[i].color = flashColor;
		}

		for(int r = 0; r < 3; r++) {
			if(audioManager != null) {
				audioManager.PlaySound("goalLight");
			}

			float duration = 0.11f;
			//float startDuration = 0.25f;
			//float interval = startDuration / stepFills.Count;

			for(int i = 0; i < stepFills.Count; i++) {
				stepFills[i].color = flashColor;

				//float duration = baseDuration + ((stepFills.Count - 1 - i) * interval);

				float timer = 0f;
				while(timer < duration) {
					timer += Time.deltaTime;

					yield return waiter;
				}

				if(i > 0) {
					StartCoroutine(LightCooldown(stepFills[i - 1]));
				}
			}

			float pauseBetweenRepitions = 0.2f;
			yield return new WaitForSeconds(pauseBetweenRepitions);

			StartCoroutine(LightCooldown(stepFills[stepFills.Count - 1]));
		}
	}

	public IEnumerator LightCooldown(SpriteRenderer sr) {	
		Color startColor = sr.color;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.3f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			sr.color = Color.Lerp(startColor, stepColor, timer / duration);

			yield return waiter;
		}

		sr.color = stepColor;
	}
}
