using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class RaisedButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public bool belongsToHome;

	private MatchController matchController;
	private CanvasManager canvasManager;

	private Image backPlate;
	private Image frontPlate;
	private Image frontInner;
	private TextMeshProUGUI buttonText;

	private float raisedY = 0.1f;

	private Color unhoveredColor;
	private Color hoveredColor;

	private Coroutine flashCoroutine;

	private bool interactable = true;
	private bool myTurn = false;


	void Awake() {
		matchController = FindObjectOfType<MatchController>();
		canvasManager = FindObjectOfType<CanvasManager>();

		backPlate = GetComponent<Image>();
		frontPlate = transform.GetChild(0).GetComponent<Image>();
		frontInner = transform.GetChild(0).GetChild(0).GetComponent<Image>();
		buttonText = frontPlate.GetComponentInChildren<TextMeshProUGUI>();

		raisedY = frontPlate.transform.localPosition.y;
	}

	public void SetForTeam(Team team) {
		unhoveredColor = team.primaryColor;
		hoveredColor = team.GetDarkTint();

		frontPlate.color = unhoveredColor;
		backPlate.color = hoveredColor;
		buttonText.color = Color.white;
	}

	public void RaiseButton() {
		frontPlate.transform.localPosition = new Vector3(0, raisedY, 0);
		
		interactable = true;
	}

	public void LowerButton() {
		StopFlash();

		frontPlate.transform.localPosition = Vector3.zero;

		buttonText.color = Color.white;

		interactable = false;
	}

	public void StartFlash() {
		StopFlash();
		flashCoroutine = StartCoroutine(Flashing(0f));
	}

	public void ResumeFlash() {
		StopFlash();
		flashCoroutine = StartCoroutine(Flashing(1f));
	}

	public IEnumerator Flashing(float startStep) {
		Color startColor = unhoveredColor;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		bool countingUp = true;
		float step = startStep;
		while(true) {
			if(countingUp) {
				step += Time.deltaTime;

				if(step >= 1f) {
					countingUp = false;
				}
			} else {
				step -= Time.deltaTime;

				if(step <= 0f) {
					countingUp = true;
				}
			}

			frontPlate.color = Color.Lerp(startColor, Color.white, step);
			buttonText.color = Color.Lerp(startColor, Color.white, step);

			yield return waiter;
		}
	}
	
	public void StopFlash() {
		if(flashCoroutine != null) {
			StopCoroutine(flashCoroutine);
			flashCoroutine = null;

			buttonText.color = Color.white;
		}
	}


	//I think I should only use the button
	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData) {
		if(interactable) {
			//myTurn = false;
			//matchController.ReadyTeam(belongsToHome);
			//buttonText.text = "Ready";

			LowerButton();
			StopFlash();

			//if it's the timeout raised button
			canvasManager.TimeoutButtonClicked();
		}
	}
	#endregion


	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData) {
		if(interactable) {
			StopFlash();

			frontPlate.color = Color.white;
		}
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData) {
		if(interactable) {
			if(matchController.IsTimeoutAcceptable()) {
				ResumeFlash();
			} else {
				frontPlate.color = unhoveredColor;
			}
		}
	}
	#endregion

	public void SetText(string text) {
		buttonText.text = text;
	}

}
