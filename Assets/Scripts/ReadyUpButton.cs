using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ReadyUpButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	public bool belongsToHome;

	private MatchController matchController;

	private Image backPlate;
	private GameObject frontPlate;
	private TextMeshProUGUI buttonText;

	private float raisedY = 0.1f;
	private Color backPlateInactiveColor;
	private Color backPlateWaitingColor;

	private Coroutine flashCoroutine;

	private bool interactable = true;
	private bool myTurn = false;

	void Awake() {
		matchController = FindObjectOfType<MatchController>();

		backPlate = GetComponent<Image>();
		frontPlate = transform.GetChild(1).gameObject;
		buttonText = frontPlate.GetComponentInChildren<TextMeshProUGUI>();

		raisedY = frontPlate.transform.localPosition.y;

		backPlateInactiveColor = backPlate.color;
		backPlateWaitingColor = Color.gray;
	}

	public void SetForTeam(Team team) {
		gameObject.SetActive(true);
		gameObject.transform.GetChild(0).GetComponent<Image>().color = team.primaryColor;
		gameObject.transform.GetChild(1).GetComponent<Image>().color = team.primaryColor;
	}

	public void RaiseButton() {
		frontPlate.transform.localPosition = new Vector3(0, raisedY, 0);
		
		interactable = true;
	}

	public void LowerButton() {
		StopFlash();

		frontPlate.transform.localPosition = Vector3.zero;

		interactable = false;

		buttonText.color = backPlateInactiveColor;
		backPlate.color = backPlateInactiveColor;
	}

	public void MyTurnNow(bool isMine) {
		myTurn = isMine;

		if(buttonText.enabled) {
			buttonText.text = "Ready Up";
		}

		if(myTurn) {
			RaiseButton();
			StartFlash();
		} else {
			LowerButton();
			StopFlash();
		}
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
		Color startColor = backPlateWaitingColor;

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

			backPlate.color = Color.Lerp(startColor, Color.white, step);
			buttonText.color = Color.Lerp(startColor, Color.white, step);

			yield return waiter;
		}
	}
	
	public void StopFlash() {
		if(flashCoroutine != null) {
			StopCoroutine(flashCoroutine);
			flashCoroutine = null;
		}
	}

	public void EndMatch(int goalDifferential) {
		gameObject.SetActive(true);
		
		LowerButton();

		buttonText.color = Color.white;
		
		if(goalDifferential < 0) {
			buttonText.text = "Defeat";
		} else if (goalDifferential == 0) {
			buttonText.text = "Draw";
		} else {
			buttonText.text = "Victory";
		}
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData) {
		if(interactable) {
			myTurn = false;

			//matchController.ReadyTeam(belongsToHome);

			buttonText.text = "Ready";

			LowerButton();
			StopFlash();
		}
	}
	#endregion


	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData) {
		if(interactable) {
			StopFlash();

			backPlate.color = Color.white;
			buttonText.color = Color.white;
		}
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData) {
		if(interactable) {
			if(myTurn) {
				ResumeFlash();
			} else {
				buttonText.color = backPlateWaitingColor;
				backPlate.color = backPlateWaitingColor;
			}
		}
	}
	#endregion
}
