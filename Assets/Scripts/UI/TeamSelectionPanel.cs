using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamSelectionPanel : MonoBehaviour {

	public bool homeSide = true;

	public Button leftArrow;
	public Button rightArrow;
	private Image fill;
	public Image border;
	public Button computerToggleButton;
	public Image computerToggleBorder;
	public Image winCounterBody;
	public Image winCounterBorder;
	public TextMeshProUGUI winCounterTextLabel;
	public TextMeshProUGUI winCounterTextNum;

	private TextMeshProUGUI text;

	private ModeController modeController;

	private Team team;

	void Awake() {
		modeController = FindObjectOfType<ModeController>();

		fill = GetComponent<Image>();
		text = GetComponentInChildren<TextMeshProUGUI>();
		
		leftArrow.onClick.AddListener(() => modeController.CycleTeamSelected(homeSide, true));
		rightArrow.onClick.AddListener(() => modeController.CycleTeamSelected(homeSide, false));
	}
	public void SetTeam(Team t) {
		team = t;

		text.text = t.name;
		fill.color = t.primaryColor;
		border.color = t.secondaryColor;

		leftArrow.GetComponent<Image>().color = t.secondaryColor;
		rightArrow.GetComponent<Image>().color = t.secondaryColor;

		UpdateComputerToggleText();

		winCounterBody.color = t.primaryColor;
		winCounterBorder.color = t.secondaryColor;
		winCounterTextLabel.text = "Wins Today: ";
		winCounterTextNum.text = PlayerPrefs.GetInt(t.name, 0).ToString();
	}

	public void ToggleComputerControl() {
		modeController.ToggleComputerControl(team);
		UpdateComputerToggleText();
	}


	public void UpdateComputerToggleText() {
		if(team.computerControlled) {
			computerToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Computer";
			computerToggleButton.GetComponent<Image>().color = Color.grey;
			computerToggleBorder.color = Color.grey;
		} else {
			computerToggleButton.GetComponentInChildren<TextMeshProUGUI>().text = "Player";
			computerToggleButton.GetComponent<Image>().color = team.primaryColor;
			computerToggleBorder.color = team.secondaryColor;
		}
	}

	public void EnableInteraction(bool enabled) {
		if(enabled) {
			leftArrow.gameObject.SetActive(true);
			rightArrow.gameObject.SetActive(true);

			computerToggleButton.gameObject.SetActive(true);
		} else {
			leftArrow.gameObject.SetActive(false);
			rightArrow.gameObject.SetActive(false);

			computerToggleButton.gameObject.SetActive(false);
		}
	}
}
