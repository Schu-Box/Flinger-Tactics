using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TurnButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

	private GameController gameController;

	private Image border;
	private Image background;
	private TextMeshProUGUI buttonText;

	private Color borderInactiveColor;

	private Coroutine flashCoroutine;

	private bool interactable = false;
	private float raisedY = 10f;

	void Start() {
		gameController = FindObjectOfType<GameController>();

		border = GetComponent<Image>();
		background = GetComponentInChildren<Image>();
		buttonText = GetComponentInChildren<TextMeshProUGUI>();

		borderInactiveColor = border.color;
	}

	public void PreMatch() {
		interactable = true;

		buttonText.color = Color.white;
		buttonText.fontSize = 28;
		buttonText.text = "Begin";

		RaiseButton();

		StartFlash();
	}

	public void DuringMatch() {
		buttonText.color = Color.white;
		buttonText.fontSize = 52;

		border.color = borderInactiveColor;
		
		SetTurnCounter(1);
	}
	
	public void SetTurnCounter(int turnNum) {
		buttonText.text = turnNum.ToString();
	}

	public void PostMatch() {
		interactable = true;

		buttonText.color = Color.white;
		buttonText.fontSize = 32;
		buttonText.text = "Exit";

		RaiseButton();

		StartFlash();
	}

	public void RaiseButton() {
		gameObject.transform.localPosition = new Vector3(0, 10, 0);
	}

	public void LowerButton() {
		interactable = false;

		gameObject.transform.localPosition = Vector3.zero;
	}

	public void StartFlash() {
		if(flashCoroutine != null) {
			StopCoroutine(flashCoroutine);
			flashCoroutine = null;
		}
		flashCoroutine = StartCoroutine(Flashing());
	}

	public IEnumerator Flashing() {
		Color startColor = border.color;
		Color endColor;

		if(startColor == borderInactiveColor ) {
			endColor = Color.white;
		} else {
			endColor = borderInactiveColor;
		}

		float step = 0;
		bool countingUp = true;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
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

			border.color = Color.Lerp(startColor, endColor, step);
			buttonText.color = Color.Lerp(startColor, endColor, step);

			yield return waiter;
		}
	}

	public void StopFlash() {
		if(flashCoroutine != null) {
			StopCoroutine(flashCoroutine);
			flashCoroutine = null;
		}
	}

	#region IPointerClickHandler implementation
	public void OnPointerClick (PointerEventData eventData) {
		if(interactable) {
			if(!gameController.matchStarted) {
				gameController.StartMatch();
			} else if(gameController.matchOver) {
				gameController.DisplayPostMatchPanel();
			}

			LowerButton();
			StopFlash();
		}
	}
	#endregion


	#region IPointerEnterHandler implementation
	public void OnPointerEnter (PointerEventData eventData) {
		if(interactable) {
			StopFlash();

			border.color = Color.white;
			buttonText.color = Color.white;
		}
	}
	#endregion

	#region IPointerExitHandler implementation
	public void OnPointerExit (PointerEventData eventData) {
		if(interactable) {
			StartFlash();
		}
	}
	#endregion
}
