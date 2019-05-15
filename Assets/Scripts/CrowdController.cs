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

	private List<Crowd> crowdList = new List<Crowd>();
	private bool crowdPresent = false;


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
			Color sectionColor;
			if(i % 2 == 1) {
				sectionColor = team.primaryColor;
			} else {
				sectionColor = team.secondaryColor;
			}
			for(int j = 0; j < transform.GetChild(i).childCount; j++) {
				GameObject seatObj = transform.GetChild(i).GetChild(j).gameObject;

				seatObj.GetComponent<SpriteRenderer>().color = sectionColor;
			}
		}
	}

	public void SetCrowd() {
		if(crowdPresent) {
			ClearCrowd();
		}

		crowdPresent = true;
		for(int i = 0; i < transform.childCount; i++) {
			for(int j = 0; j < transform.GetChild(i).childCount; j++) {
				GameObject seatObj = transform.GetChild(i).GetChild(j).gameObject;

				float rando = Random.value;
				if(rando > 0.7) {
					//Leave the seat empty
				} else {
					GameObject crowdAthlete = Instantiate(crowdPrefab, seatObj.transform.position, seatObj.transform.rotation, seatObj.transform);
					crowdAthlete.GetComponent<Crowd>().SetCrowdMember();

					crowdList.Add(crowdAthlete.GetComponent<Crowd>());
				}
			}
		}

		ExpressEmotion("watching");
	}

	public void ClearCrowd() {
		for(int i = 0; i < transform.childCount; i++) {
			for(int j = 0; j < transform.GetChild(i).childCount; j++) {
				GameObject seatObj = transform.GetChild(i).GetChild(j).gameObject;

				if(seatObj.transform.childCount > 0) {
					crowdList.Remove(seatObj.transform.GetChild(0).GetComponent<Crowd>());
					Destroy(seatObj.transform.GetChild(0).gameObject);
				}
			}
		}
	}

	public void SetFocus() {
		List<FocalObject> focalObjects = new List<FocalObject>();

		List<BallController> balls = matchController.GetBallsOnField();
		for(int i = 0; i < balls.Count; i++) {
			focalObjects.Add(balls[i].GetComponent<FocalObject>());
		}
		List<AthleteController> athletes = matchController.GetAllAthletesOnField();
		for(int i = 0; i < athletes.Count; i++) {
			focalObjects.Add(athletes[i].GetComponent<FocalObject>());
		}

		for(int i = 0; i < crowdList.Count; i++) {
			float rando = Random.value;
			FocalObject chosenObj;
			if(rando > 0.2) { //80% chance they focus on a ball
				chosenObj = focalObjects[Random.Range(0, balls.Count)];
			} else {
				chosenObj = focalObjects[Random.Range(balls.Count, focalObjects.Count)];
			}

			crowdList[i].SetFocus(chosenObj);
		}
	}

	public void ExpressEmotion(string emotion) {
		for(int i = 0; i < crowdList.Count; i++) {
			crowdList[i].GetAthleteController().GetFace().ChangeExpression(emotion, 2f);
		}
	}

	public void ExpressEmotion(string emotion, bool home) {
		Team fromTeam = matchController.GetTeam(home);

		for(int i = 0; i < crowdList.Count; i++) {
			if(crowdList[i].GetAthleteController().GetAthlete().GetTeam() == fromTeam) {
				crowdList[i].GetAthleteController().GetFace().ChangeExpression(emotion, 2f);
			}
		}
	}

	public IEnumerator FlashSteps(Color flashColor, string soundID, int repeats, float delayBetween) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		for(int i = 0; i < stepFills.Count; i++) {
			stepFills[i].color = flashColor;
		}

		for(int r = 0; r < repeats; r++) {
			if(audioManager != null) {
				audioManager.Play(soundID);
			}

			for(int i = 0; i < stepFills.Count; i++) {
				stepFills[i].color = flashColor;

				float timer = 0f;
				while(timer < delayBetween) {
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
