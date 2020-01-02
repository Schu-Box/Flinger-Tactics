using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour {

	public SpriteRenderer bodySprite;
	public SpriteRenderer faceBaseSprite;
	public Face face;
	public SpriteRenderer jerseySprite;
	public SpriteRenderer tailTipSprite;
	public List<SpriteRenderer> legList;

	private Athlete athlete;
	private FocalObject focalObject;
	private bool focusChanging = false;
	private Vector3 oldFocalPoint;
	private float timeUntilSwitchedFocus;
	private float timeSinceSwitchedFocus;

	private bool isWatching = false;

	private Vector3 startSize;

	//For crowd movements
	private Vector3 startLocalPosition;
	private Vector3 shiftPosition;
	private float shiftDuration;
	private float shiftElapsed;
	private float waitForShift;

	private bool jumpingForJoy = false;
	private float jumpTimeRemaining = 0f; //Total time spent jumping
	private float jumpDuration; //Time of a single jump cycle
	private float jumpElapsed;
	private Vector3 jumpedSize;

	public void SetCrowdMember(Team favoriteTeam) {

		athlete = new Athlete();
		athlete.SetTeam(favoriteTeam);

		bodySprite.sprite = athlete.athleteData.bodySprite;
		bodySprite.color = athlete.bodySkinColor;
		tailTipSprite.color = athlete.bodySkinColor;
		faceBaseSprite.sprite = athlete.athleteData.faceBaseSprite;
		faceBaseSprite.color = athlete.skinColor;
		face.SetFace();
		face.SetFaceSprite("neutral");
		jerseySprite.sprite = athlete.athleteData.athleteJersey;
		jerseySprite.color = favoriteTeam.primaryColor;

		float standardWidth = legList[0].size.x;
        for(int l = 0; l < legList.Count; l++) {
			legList[l].sprite = athlete.athleteData.legSprite;
			if(l < legList.Count / 2) {
				legList[l].size = new Vector2(standardWidth, athlete.athleteData.frontLegRest * standardWidth);
			} else {
				legList[l].size = new Vector2(standardWidth, athlete.athleteData.backLegRest * standardWidth);
			}
        }

		startSize = transform.localScale;
		startLocalPosition = transform.localPosition;
		SetNewShift();
		
		jumpedSize = transform.localScale;
		jumpedSize.x *= 1.2f;
		jumpedSize.y *= 1.2f;
	}

	public Athlete GetAthlete() {
		return athlete;
	}

	public void SetFocus(FocalObject f) {
		if(focalObject != null) {
			focusChanging = true;

			focalObject.RemoveWatcher(this);

			oldFocalPoint = focalObject.transform.position;
			timeUntilSwitchedFocus = Random.Range(1f, 2.5f);
			timeSinceSwitchedFocus = 0f;
		}

		focalObject = f;
		focalObject.AddWatcher(this);
	}

	public bool GetFocusChanging() {
		return focusChanging;
	}

	public void ChangeTimeSinceSwitchedFocus(float timeChange) {
		timeSinceSwitchedFocus += timeChange;

		if(timeSinceSwitchedFocus >= timeUntilSwitchedFocus) {
			focusChanging = false;
		}
	}
	
	public void StopFocusing() {
		focalObject = null;
	}

	public void StartWatching() {
		isWatching = true;
	}

	public void StopWatching() {
		isWatching = false;
	}

	public bool GetWatching() {
		return isWatching;
	}

	public Quaternion GetFocalQuaternion() {
		Vector3 offset = Vector3.zero;
		if(focusChanging) {
			if(focalObject != null) {
				offset = transform.position - Vector3.Lerp(oldFocalPoint, focalObject.transform.position, timeSinceSwitchedFocus/timeUntilSwitchedFocus);
			}
		} else {
			offset = transform.position - focalObject.transform.position;
		}
		
		Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, offset);
		//newRotation.eulerAngles = new Vector3(newRotation.x, newRotation.y, Mathf.Clamp(newRotation.eulerAngles.z, -30, 30));

		return newRotation;
	}

	public void IncreaseJumpTimer(float timeAdded) {
		jumpTimeRemaining += timeAdded;

		if(!jumpingForJoy) { //Start Jump State
			jumpingForJoy = true;

			jumpDuration = Random.Range(0.7f, 1.2f);
			jumpElapsed = 0f;
		}

		float extraJumpTime = (jumpTimeRemaining % jumpDuration); 
		jumpTimeRemaining += extraJumpTime; //Add extra time so athlete can complete last full jump cycle
	}

	public void ReduceJumpTimer() {
		jumpTimeRemaining -= Time.deltaTime;
	}

	public float GetJumpTimer() {
		return jumpTimeRemaining;
	}

	public float GetJumpDuration() {
		return jumpDuration;
	}

	public void IncreaseJumpElapsed(float increase) {
		jumpElapsed += increase;
	}

	public float GetJumpElapsed() {
		return jumpElapsed;
	}

	public void SetJumpStep(float step) {
		transform.localScale = Vector3.Lerp(startSize, jumpedSize, step);
		//athleteController.SetLegLength(step - 0.2f);
	}

	public void EndJumpState() {
		transform.localScale = startSize;

		//athleteController.RetractLegs();

		jumpingForJoy = false;
		jumpTimeRemaining = 0f;
	}

	public Vector3 GetStartLocalPosition() {
		return startLocalPosition;
	}

	public Vector3 GetShiftPosition() {
		return shiftPosition;
	}

	public void SetNewShift() {
		shiftPosition = startLocalPosition + (Vector3)(Random.insideUnitCircle * 0.07f);

		waitForShift = Random.Range(0, 3f);

		shiftDuration = Random.Range(0.8f, 5f);
		shiftElapsed = 0f;
	}

	public float GetShiftDuration() {
		return shiftDuration;
	}

	public void IncreaseShiftElapsed(float increase) {
		if(waitForShift > 0) {
			waitForShift -= increase;
		} else {
			shiftElapsed += increase;
		}
	}

	public float GetShiftElapsed() {
		return shiftElapsed;
	}

	public float GetShiftWaitTime() {
		return waitForShift;
	}
}
