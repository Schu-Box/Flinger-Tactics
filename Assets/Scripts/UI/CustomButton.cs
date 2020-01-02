using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomButton : MonoBehaviour {

	private AudioManager audioManager;

	private Image basePlate;
	private Button button;
	private TextMeshProUGUI text;

	private Vector3 buttonOffset;

	private bool mouseEntered;
	private bool disabledInteraction;

	private void Start() {
		audioManager = FindObjectOfType<AudioManager>();
		basePlate = GetComponent<Image>();
		button = GetComponentInChildren<Button>();
		text = button.GetComponentInChildren<TextMeshProUGUI>();

		buttonOffset = button.transform.localPosition;
	}

	private void OnMouseEnter() {
		mouseEntered = true;
	}

	private void OnMouseExit() {
		mouseEntered = false;
	}

	private void OnMouseDown() {
		if(!disabledInteraction) {
			button.transform.localPosition = Vector3.zero;
		}
	}
	

	private void OnMouseUp() {
		if(!disabledInteraction) {
			button.transform.localPosition = buttonOffset;
		}
	}

	public void OnMouseUpAsButton() {
		if(!disabledInteraction) {
			audioManager.Play("buttonClick");
		}
	}

	public void DisableButton() {
		disabledInteraction = true;
		button.transform.localPosition = Vector3.zero;
		button.interactable = false;
	}

	public void EnableButton() {
		disabledInteraction = false;
		button.transform.localPosition = buttonOffset;
		button.interactable = true;
	}

	/*
	private IEnumerator ResetButton() {
		yield return WaitForSeconds(1f);

		button.transform.localPosition = Vector3.
	}
	*/
}
