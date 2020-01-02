using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour {

	public List<SpriteRenderer> homeHorizontalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> homeUpperDiagonalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> homeLowerDiagonalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> homeUpperVerticalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> homeLowerVerticalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> awayHorizontalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> awayUpperDiagonalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> awayLowerDiagonalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> awayUpperVerticalStepFills = new List<SpriteRenderer>();
	public List<SpriteRenderer> awayLowerVerticalStepFills = new List<SpriteRenderer>();

	private Color stepColor;

    public GameObject crowdPrefab;

	private MatchController matchController;
	private AudioManager audioManager;

	private List<Crowd> crowdList = new List<Crowd>();
	private bool crowdPresent = false;


	void Start() {
		matchController = FindObjectOfType<MatchController>();
		audioManager = FindObjectOfType<AudioManager>();

		if(homeHorizontalStepFills.Count > 0) {
			stepColor = homeHorizontalStepFills[0].color;
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

				float randomForEmpty = Random.value;
				if(randomForEmpty > 0.7) {
					//Leave the seat empty
				} else {
					GameObject crowdAthlete = Instantiate(crowdPrefab, seatObj.transform.position, seatObj.transform.rotation, seatObj.transform);

					Team sectionTeam;
					Team nonSectionTeam;
					if(transform.GetChild(i).GetComponent<SeatingSection>().homeSection) {
						sectionTeam = matchController.GetTeam(true);
						nonSectionTeam = matchController.GetTeam(false);
					} else {
						sectionTeam = matchController.GetTeam(false);
						nonSectionTeam = matchController.GetTeam(true);
					}

					float randomForWrong = Random.value;
					Team fav;
					if(randomForWrong > 0.9) {
						fav = nonSectionTeam;
					} else {
						fav = sectionTeam;
					}

					crowdAthlete.GetComponent<Crowd>().SetCrowdMember(fav);

					crowdList.Add(crowdAthlete.GetComponent<Crowd>());
				}
			}
		}

		ExpressEmotion("watching");

		StartCoroutine(CrowdMovements());
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
			crowdList[i].face.ChangeExpression(emotion, 2f);
		}
	}

	public void ExpressEmotion(string emotion, bool home) {
		Team fromTeam = matchController.GetTeam(home);

		for(int i = 0; i < crowdList.Count; i++) {
			if(crowdList[i].GetAthlete().GetTeam() == fromTeam) {
				crowdList[i].face.ChangeExpression(emotion, 2f);
			}
		}
	}

	public void GoalReaction(Team team) {
		for(int i = 0; i < crowdList.Count; i++) {
			Crowd c = crowdList[i];
			if(c.GetAthlete().GetTeam() == team) {
				c.face.ChangeExpression("happy", Random.Range(1f, 5f) + Random.value);
				//c.IncreaseJumpTimer(Random.Range(1f, 4f));
			} else {
				c.face.ChangeExpression("sad", Random.Range(1f, 6f) + Random.value);
			}
		}
	}

	public void SetPostMatchFaces() {
		for(int i = 0; i < crowdList.Count; i++) {
			Crowd c = crowdList[i];
			if(c.GetAthlete().GetTeam() == matchController.GetMatchData().GetWinner()) {
				c.face.SetFaceSprite("victory");
				//ac.GetComponent<Crowd>().IncreaseJumpTimer(Random.Range(10f, 30f));
			} else {
				c.face.SetFaceSprite("defeat");
			}
		}
	}

	public IEnumerator FlashSteps(Color flashColor, string soundID, int repeats, float delayBetween) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		for(int i = 0; i < homeHorizontalStepFills.Count; i++) {
			homeHorizontalStepFills[i].color = flashColor;
			homeUpperDiagonalStepFills[i].color = flashColor;
			homeLowerDiagonalStepFills[i].color = flashColor;
			homeUpperVerticalStepFills[i].color = flashColor;
			homeLowerVerticalStepFills[i].color = flashColor;
			awayHorizontalStepFills[i].color = flashColor;
			awayUpperDiagonalStepFills[i].color = flashColor;
			awayLowerDiagonalStepFills[i].color = flashColor;
			awayUpperVerticalStepFills[i].color = flashColor;
			awayLowerVerticalStepFills[i].color = flashColor;
		}

		for(int r = 0; r < repeats; r++) {
			if(audioManager != null) {
				audioManager.Play(soundID);
			}

			for(int i = 0; i < homeHorizontalStepFills.Count; i++) {
				homeHorizontalStepFills[i].color = flashColor;
				homeUpperDiagonalStepFills[i].color = flashColor;
				homeLowerDiagonalStepFills[i].color = flashColor;
				homeUpperVerticalStepFills[i].color = flashColor;
				homeLowerVerticalStepFills[i].color = flashColor;
				awayHorizontalStepFills[i].color = flashColor;
				awayUpperDiagonalStepFills[i].color = flashColor;
				awayLowerDiagonalStepFills[i].color = flashColor;
				awayUpperVerticalStepFills[i].color = flashColor;
				awayLowerVerticalStepFills[i].color = flashColor;

				float timer = 0f;
				while(timer < delayBetween) {
					timer += Time.deltaTime;

					yield return waiter;
				}

				if(i > 0) {
					StartCoroutine(LightCooldown(homeHorizontalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeUpperDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeLowerDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeUpperVerticalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeLowerVerticalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayHorizontalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayUpperDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayLowerDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayUpperVerticalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayLowerVerticalStepFills[i - 1]));
				}
			}

			float pauseBetweenRepitions = 0.2f;
			yield return new WaitForSeconds(pauseBetweenRepitions);

			StartCoroutine(LightCooldown(homeHorizontalStepFills[homeHorizontalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeUpperDiagonalStepFills[homeUpperDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeLowerDiagonalStepFills[homeLowerDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeUpperVerticalStepFills[homeUpperVerticalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeLowerVerticalStepFills[homeLowerVerticalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayHorizontalStepFills[awayHorizontalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayUpperDiagonalStepFills[awayUpperDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayLowerDiagonalStepFills[awayLowerDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayUpperVerticalStepFills[awayUpperVerticalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayLowerVerticalStepFills[awayLowerVerticalStepFills.Count - 1]));
		}
	}

	public IEnumerator FlashHomeSteps(Color flashColor, string soundID, int repeats, float delayBetween) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		for(int i = 0; i < homeHorizontalStepFills.Count; i++) {
			homeHorizontalStepFills[i].color = flashColor;
			homeUpperDiagonalStepFills[i].color = flashColor;
			homeLowerDiagonalStepFills[i].color = flashColor;
			homeUpperVerticalStepFills[i].color = flashColor;
			homeLowerVerticalStepFills[i].color = flashColor;
		}

		for(int r = 0; r < repeats; r++) {
			if(audioManager != null) {
				audioManager.Play(soundID);
			}

			for(int i = 0; i < homeHorizontalStepFills.Count; i++) {
				homeHorizontalStepFills[i].color = flashColor;
				homeUpperDiagonalStepFills[i].color = flashColor;
				homeLowerDiagonalStepFills[i].color = flashColor;
				homeUpperVerticalStepFills[i].color = flashColor;
				homeLowerVerticalStepFills[i].color = flashColor;

				float timer = 0f;
				while(timer < delayBetween) {
					timer += Time.deltaTime;

					yield return waiter;
				}

				if(i > 0) {
					StartCoroutine(LightCooldown(homeHorizontalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeUpperDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeLowerDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeUpperVerticalStepFills[i - 1]));
					StartCoroutine(LightCooldown(homeLowerVerticalStepFills[i - 1]));
				}
			}

			float pauseBetweenRepitions = 0.2f;
			yield return new WaitForSeconds(pauseBetweenRepitions);

			StartCoroutine(LightCooldown(homeHorizontalStepFills[homeHorizontalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeUpperDiagonalStepFills[homeUpperDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeLowerDiagonalStepFills[homeLowerDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeUpperVerticalStepFills[homeUpperVerticalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(homeLowerVerticalStepFills[homeLowerVerticalStepFills.Count - 1]));
		}
	}

	public IEnumerator FlashAwaySteps(Color flashColor, string soundID, int repeats, float delayBetween) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		for(int i = 0; i < awayHorizontalStepFills.Count; i++) {
			awayHorizontalStepFills[i].color = flashColor;
			awayUpperDiagonalStepFills[i].color = flashColor;
			awayLowerDiagonalStepFills[i].color = flashColor;
			awayUpperVerticalStepFills[i].color = flashColor;
			awayLowerVerticalStepFills[i].color = flashColor;
		}

		for(int r = 0; r < repeats; r++) {
			if(audioManager != null) {
				audioManager.Play(soundID);
			}

			for(int i = 0; i < awayHorizontalStepFills.Count; i++) {
				awayHorizontalStepFills[i].color = flashColor;
				awayUpperDiagonalStepFills[i].color = flashColor;
				awayLowerDiagonalStepFills[i].color = flashColor;
				awayUpperVerticalStepFills[i].color = flashColor;
				awayLowerVerticalStepFills[i].color = flashColor;

				float timer = 0f;
				while(timer < delayBetween) {
					timer += Time.deltaTime;

					yield return waiter;
				}

				if(i > 0) {
					StartCoroutine(LightCooldown(awayHorizontalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayUpperDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayLowerDiagonalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayUpperVerticalStepFills[i - 1]));
					StartCoroutine(LightCooldown(awayLowerVerticalStepFills[i - 1]));
				}
			}

			float pauseBetweenRepitions = 0.2f;
			yield return new WaitForSeconds(pauseBetweenRepitions);

			StartCoroutine(LightCooldown(awayHorizontalStepFills[awayHorizontalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayUpperDiagonalStepFills[awayUpperDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayLowerDiagonalStepFills[awayLowerDiagonalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayUpperVerticalStepFills[awayUpperVerticalStepFills.Count - 1]));
			StartCoroutine(LightCooldown(awayLowerVerticalStepFills[awayLowerVerticalStepFills.Count - 1]));
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

	public IEnumerator CrowdMovements() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		float timer = 0f;

		while(true) {
			timer += Time.deltaTime;

			for(int i = 0; i < crowdList.Count; i++) {
				Crowd c = crowdList[i];

				//Jumps
				if(c.GetJumpTimer() > 0) {
					c.IncreaseJumpElapsed(Time.deltaTime);
					float step = c.GetJumpElapsed() % c.GetJumpDuration();
					step = 1f - Mathf.Abs((step - 0.5f) / 0.5f);
					c.SetJumpStep(step);

					c.ReduceJumpTimer();
					if(c.GetJumpTimer() <= 0) {
						c.EndJumpState();
					}
				} else {
					if(c.GetFocusChanging()) {
						c.ChangeTimeSinceSwitchedFocus(Time.deltaTime);
					}

					//Rotations
					if(c.GetWatching() || c.GetFocusChanging()) {
						c.transform.rotation = c.GetFocalQuaternion();
					}

					//Subtle Shifts
					c.IncreaseShiftElapsed(Time.deltaTime);
					if(c.GetShiftWaitTime() <= 0) {
						float step = c.GetShiftElapsed() / c.GetShiftDuration();
						if(step > 1f || c.GetShiftDuration() <= 0f) { //If the shift is completed (or not initially set)
							c.SetNewShift();
						}
						step = 1f - Mathf.Abs((step - 0.5f) / 0.5f); //Reaches destination at step = 0.5f, then returns to start
						c.transform.localPosition = Vector3.Lerp(c.GetStartLocalPosition(), c.GetShiftPosition(), step);
					}
				}
			}

			yield return waiter;
		}
	}
}
