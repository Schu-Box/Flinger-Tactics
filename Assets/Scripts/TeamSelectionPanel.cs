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

	private GameController gameController;

	void Awake() {
		gameController = FindObjectOfType<GameController>();

		text = GetComponentInChildren<TextMeshProUGUI>();

		leftArrow.onClick.AddListener(() => gameController.CycleTeamSelected(homeSide, true));
		rightArrow.onClick.AddListener(() => gameController.CycleTeamSelected(homeSide, false));
	}

	public void SetTeam(Team t) {
		text.text = t.name;
		border.color = t.color;
	}
}
