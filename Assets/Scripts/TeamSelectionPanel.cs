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

	private PlayNowController playNowController;

	void Awake() {
		playNowController = FindObjectOfType<PlayNowController>();

		text = GetComponentInChildren<TextMeshProUGUI>();

		leftArrow.onClick.AddListener(() => playNowController.CycleTeamSelected(homeSide, true));
		rightArrow.onClick.AddListener(() => playNowController.CycleTeamSelected(homeSide, false));
	}

	public void SetTeam(Team t) {
		text.text = t.name;
		border.color = t.primaryColor;
	}
}
