using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstituteChair : MonoBehaviour {

	bool dragging = false;

	private TitleMenuController titleMenuController;
	private MatchController matchController;
	private CursorController cursorController;

	private SpriteRenderer sr;
	private Vector3 subChairRest;

	private Color startColor;

	private float launchForce = 500f;

	private float maxXStretch;
	private float maxYStretch;

	private bool interactable = true;

	private bool homeSub;

	private AthleteController currentSubstitute;

	public void SetChair(bool home) {
		interactable = false;

		homeSub = home;

		titleMenuController = FindObjectOfType<TitleMenuController>();
		matchController = FindObjectOfType<MatchController>();
		cursorController = FindObjectOfType<CursorController>();

		sr = GetComponent<SpriteRenderer>();

		GoalController goal;
		
		if(home) {
			goal = matchController.homeGoal;
			
			sr.color = matchController.GetTeam(true).primaryColor;
		} else {
			goal = matchController.awayGoal;

			sr.color = matchController.GetTeam(false).primaryColor;
		}

		SubstitutePlatform subPlatform = goal.GetSubPlatform();

		subChairRest = subPlatform.GetSubChairRest();

		maxXStretch = Mathf.Abs(subPlatform.GetMaxStretch().x - subChairRest.x);
		maxYStretch = subPlatform.GetMaxStretch().y - (transform.localScale.y / 2);

		startColor = sr.color;

		StartCoroutine(GrowChair());

		//SetSpriteMaskInteraction(true);
	}

	public void SetCurrentSubstitute(AthleteController ac) {
		currentSubstitute = ac;
	}

	public AthleteController GetCurrentSubstitute() {
		return currentSubstitute;
	}	

	public void OnMouseEnter() {
		if(interactable) {
			if(currentSubstitute != null) {

				cursorController.SetHover(true);

				if(titleMenuController == null) {
					if(!currentSubstitute.GetSpokeThisTurn()) {
						matchController.DisplayQuote(currentSubstitute, currentSubstitute.GetAthlete().GetQuote("substitute"));
					}
				} else {
					matchController.DisplayQuote(currentSubstitute, titleMenuController.GetTutorialQuote());
				}
			}
		}
	}

	public void OnMouseExit() {
		if(interactable) {
			cursorController.SetHover(false);
		}
	}

	public void OnMouseDrag() {
		if(interactable) {
			Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
			Vector3 newPosition = Camera.main.ScreenToWorldPoint(mousePosition);

			AdjustPosition(newPosition);
		}
	}

	public void OnMouseDown() {
		cursorController.SetDragging(true);

		if(currentSubstitute.GetSpeaking()) { //If the athlete is speaking, close the quote box prematurely
            currentSubstitute.GetCurrentQuoteBox().PrematurelyClose();
        }
	}

	private bool willLaunch = false;

	public void AdjustPosition(Vector3 targetPosition) {
		dragging = true;

		Vector3 newFixedPosition = Vector3.zero;

		SubstitutePlatform subPlat;

		float onBarRoom = 0.3f;

		if(homeSub) {
			if(targetPosition.x > subChairRest.x - onBarRoom) {  //Then keep the chair locked in place
				willLaunch = false;
				newFixedPosition.x = subChairRest.x;
			} else { 
				willLaunch = true;
				newFixedPosition.x = Mathf.Clamp(targetPosition.x, subChairRest.x - maxXStretch, subChairRest.x);
			}

			subPlat = matchController.homeGoal.GetSubPlatform();
		} else {
			if(targetPosition.x < subChairRest.x + onBarRoom) {
				willLaunch = false;
				newFixedPosition.x = subChairRest.x;
			} else {
				willLaunch = true;
				newFixedPosition.x = Mathf.Clamp(targetPosition.x, subChairRest.x, subChairRest.x + maxXStretch);
			}

			subPlat = matchController.awayGoal.GetSubPlatform();
		}

		newFixedPosition.y = Mathf.Clamp(targetPosition.y, subChairRest.y - maxYStretch, subChairRest.y + maxYStretch);

		transform.position = newFixedPosition;

		subPlat.UpdateAttachmentBar(newFixedPosition);
	}

	public void OnMouseUp() {
		if(interactable) {
			ChairClicked();
		}
	}

	public void ChairClicked() {
		dragging = false;

		cursorController.SetDragging(false);

		if(willLaunch) {
			interactable = false;

			cursorController.ReleaseFromDrag();

			matchController.SubstituteAthleteIn(currentSubstitute);
			StartCoroutine(LaunchChairOntoField());
		} else {
			StartCoroutine(AnimateChair());
		}
	}

	public IEnumerator LaunchChairOntoField() {
		//SetSpriteMaskInteraction(false);

		float maxChairStretch;

        Vector2 chairStart = transform.position;
        Vector2 chairEnd = transform.position;
		if(homeSub) {
			chairEnd.x = matchController.homeGoal.GetSubPlatform().GetSubChairEnd().x;

			maxChairStretch = matchController.homeGoal.GetSubPlatform().GetMaxStretch().x;
		} else {
			chairEnd.x = matchController.awayGoal.GetSubPlatform().GetSubChairEnd().x;

			maxChairStretch = matchController.awayGoal.GetSubPlatform().GetMaxStretch().x;
		}

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float duration = 0.3f;
        float timer = 0f;
        while(timer < duration) {
            timer += Time.deltaTime;

            transform.position = Vector2.Lerp(chairStart, chairEnd, timer/duration);

            yield return waiter;
        }

		gameObject.layer = 8;

		//Get Percent force here
		float percentForce = (chairStart.x - subChairRest.x) / (maxChairStretch - subChairRest.x);

        LaunchAthleteFromChair(currentSubstitute, percentForce);

		Vector3 chairReturn = subChairRest;
		chairReturn.y = chairEnd.y;

        duration = 1f;
        timer = 0f;
        while(timer < duration) {
            timer += Time.deltaTime;

            transform.position = Vector2.Lerp(chairEnd, chairReturn, timer/duration);

            yield return waiter;
        }

		//SetSpriteMaskInteraction(true);

		matchController.ResetSubstituteStatus();

		if(currentSubstitute.GetAthlete().GetTeam() == matchController.GetTeam(true)) {
			yield return StartCoroutine(matchController.homeGoal.GetSubPlatform().AnimateSubPlatformClosing());
		} else {
			yield return StartCoroutine(matchController.awayGoal.GetSubPlatform().AnimateSubPlatformClosing());

		}

		if(GetComponentInChildren<AthleteController>() != null) {
			Debug.Log("BRUH BOY WHATCHY ON?)");
		}

        Destroy(gameObject);
    }

	public void LaunchAthleteFromChair(AthleteController ac, float percentForce) {
		ac.gameObject.SetActive(true);
        ac.transform.SetParent(matchController.athleteHolder);
		//ac.transform.position = transform.position;

		ac.ResetBodyLayer();

		//Debug.Log(percentForce);

        float xForce = launchForce * percentForce;
        if(ac.GetAthlete().GetTeam() != matchController.GetTeam(true)) {
            xForce = -xForce;
        }

        //Debug.Log("Launching with force " + xForce);

        ac.AddForce(new Vector2(xForce, 0));
    }

	public IEnumerator GrowChair() {
		
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		yield return waiter;

		/*
		Vector3 fullSize = transform.localScale;
		transform.localScale = Vector3.zero;
		

		float duration = 1.2f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			//transform.localScale = Vector3.Lerp(Vector3.zero, fullSize, timer/duration);

			yield return waiter;
		}
		*/

		StartCoroutine(AnimateChair());
	}

	public IEnumerator AnimateChair() {

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		float timer = 0f;
		float halfLife = 0.8f;
		bool countUp = true;
		while(!dragging) {
			if(countUp) {
				timer += Time.deltaTime;
			} else {
				timer -= Time.deltaTime;
			}

			if(timer >= halfLife) {
				countUp = false;
			} else if(timer <= 0) {
				countUp = true;
			}

			sr.color = Color.Lerp(startColor, Color.white, timer/halfLife);

			yield return waiter;
		}

		sr.color = startColor;
	}

	public void SetSpriteMaskInteraction(bool visibleOnlyInsideMask) {
		if(visibleOnlyInsideMask) {
			sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
		} else {
			sr.maskInteraction = SpriteMaskInteraction.None;
		}
	}

	public void SetInteractable(bool interaction) {
		interactable = interaction;

		if ((homeSub && matchController.GetTeam(true).computerControlled) || (!homeSub && matchController.GetTeam(false).computerControlled)) {
			//Team is computer controlled so you can't interact with it
			interactable = false;
		}
	}

	public Vector2 GetMaxStretch() { //These should always be the same value. But whatevs just in case
		if(homeSub) {
			return matchController.homeGoal.GetSubPlatform().GetMaxStretch();
		} else {
			return matchController.awayGoal.GetSubPlatform().GetMaxStretch();
		}
	}
}
