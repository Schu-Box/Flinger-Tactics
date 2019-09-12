using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crowd : MonoBehaviour {

	private AthleteController athleteController;
	private FocalObject focalObject;
	private Coroutine watchingCoroutine;

	public void SetCrowdMember() {
		athleteController = GetComponent<AthleteController>();

		athleteController.SetAthlete(null);
	}

	public void SetFocus(FocalObject f) {
		if(watchingCoroutine != null) {
			StopCoroutine(watchingCoroutine);
		}

		watchingCoroutine = StartCoroutine(ChangeFocus(f));
	}
	
	public void StopFocusing() {
		focalObject = null;
	}

	public void StartWatching() {
		if(watchingCoroutine != null) {
			StopCoroutine(watchingCoroutine);
		}

		watchingCoroutine = StartCoroutine(Watch());
	}

	public void StopWatching() {
		if(watchingCoroutine != null) {
			StopCoroutine(watchingCoroutine);
			watchingCoroutine = null;
		}
	}

	public AthleteController GetAthleteController() {
		return athleteController;
	}

	public IEnumerator ChangeFocus(FocalObject newFocus) {
		
		Quaternion startQuaternion;
		if(focalObject != null) {
			focalObject.RemoveWatcher(this);
			startQuaternion = GetFocalQuaternion();
		} else {
			startQuaternion = Quaternion.identity;
		}

		focalObject = newFocus;
		focalObject.AddWatcher(this);

		Quaternion endQuaternion = GetFocalQuaternion();

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float timer = 0f;
		float duration = 0.2f + Random.value;
		while(timer < duration) {
			timer += Time.deltaTime;
			
			transform.rotation = Quaternion.Lerp(startQuaternion, endQuaternion, timer/duration);

			yield return waiter;
		}
	}
	
	public IEnumerator Watch() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		int frameInterval = 2;
		while(true) {

			if(Time.frameCount % frameInterval == 0) {
				if(focalObject == null) {
					StopWatching();
				} else {
					transform.rotation = GetFocalQuaternion();
				}
			}

			yield return waiter;
		}
	}

	public Quaternion GetFocalQuaternion() {
		Vector3 offset = transform.position - focalObject.transform.position;
		Quaternion newRotation = Quaternion.LookRotation(Vector3.forward, offset);
		//newRotation.eulerAngles = new Vector3(newRotation.x, newRotation.y, Mathf.Clamp(newRotation.eulerAngles.z, -30, 30));

		return newRotation;
	}
}
