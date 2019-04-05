using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionPanel : MonoBehaviour {

	public bool homeSide = true;

	public Button leftArrow;
	public Button rightArrow;
	public Image border;
	private TextMeshProUGUI text;

	private ModeController modeController;

	void Awake() {
		modeController = FindObjectOfType<ModeController>();

		text = GetComponentInChildren<TextMeshProUGUI>();

		leftArrow.onClick.AddListener(() => modeController.CycleTeamSelected(homeSide, true));
		rightArrow.onClick.AddListener(() => modeController.CycleTeamSelected(homeSide, false));
	}

	public void SetTeam(Team t) {
		text.text = t.name;
		border.color = t.primaryColor;
	}

	public void EnableInteraction(bool enabled) {
		if(enabled) {
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(true);
		} else {
			leftArrow.gameObject.SetActive(false);
			rightArrow.gameObject.SetActive(false);
		}
	}
}
