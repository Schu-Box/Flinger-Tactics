using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuleChangeButton : MonoBehaviour {

	private ModeController modeController;

	private Image basePanel;
	private Transform topGroup;
	private Image backPanel;
	private Image frontPanel; //Should be unused
	private Image regressiveRulePanel;
	private TextMeshProUGUI regressiveChangeText;
	private Image progressiveRulePanel;
	private TextMeshProUGUI progressiveChangeText;
	private TextMeshProUGUI descriptionText;
	private Image progressionArrow;
	//private Image progressionArrowInlet; //Should be unused
	private Transform progressionSlotHolder;
	private Sprite activeRuleCircle;
	private Sprite inactiveRuleCircle;

	private Vector3 panelOffset;

	private Team team;
	private Color inactiveGreyColor;

	private Rule presentRule;
	private Rule potentialRule;
	private bool progressive;

	public static bool buttonClicked = false;

	public void OnMouseEnter() {
		if(!buttonClicked) {
			basePanel.color = team.secondaryColor;
			backPanel.color = team.secondaryColor;

			if(progressive) {
				progressiveRulePanel.color = team.primaryColor;
			} else {
				regressiveRulePanel.color = team.primaryColor;
			}

			//progressionArrow.color = team.secondaryColor;

			progressionSlotHolder.GetChild(potentialRule.ruleIndex).GetComponent<Image>().color = team.primaryColor;
		}
	}

	public void OnMouseExit() {
		if(!buttonClicked) {
			ResetColors();
		}
	}

	public void ResetColors() {
		basePanel.color = inactiveGreyColor;
		backPanel.color = inactiveGreyColor;

		progressiveRulePanel.color = inactiveGreyColor;
		regressiveRulePanel.color = inactiveGreyColor;

		progressionArrow.color = inactiveGreyColor;

		progressionSlotHolder.GetChild(potentialRule.ruleIndex).GetComponent<Image>().color = Color.white;
	}

	public void OnMouseDown() {
		PressDown();
	}

	public void PressDown() {
		backPanel.transform.localPosition = Vector3.zero;
	}

	public void OnMouseUp() {
		Unpress();
	}

	public void Unpress() {
		if(!buttonClicked) {
			backPanel.transform.localPosition = panelOffset;
		}
	}

	private void OnMouseUpAsButton() {
		buttonClicked = true;

		progressionSlotHolder.GetChild(presentRule.ruleIndex).GetComponent<Image>().sprite = inactiveRuleCircle;
		progressionSlotHolder.GetChild(potentialRule.ruleIndex).GetComponent<Image>().sprite = activeRuleCircle;

		modeController.SelectNewGauntletRule(potentialRule);
	}

	public void SetRuleCircleSprites(Sprite act, Sprite inact) {
		inactiveRuleCircle = inact;
		activeRuleCircle = act;
	}

	public void SetTeam(Team gauntletTeam) {
		team = gauntletTeam;

		modeController = FindObjectOfType<ModeController>();

		basePanel = GetComponent<Image>();
		topGroup = transform.GetChild(0);
		backPanel = topGroup.gameObject.GetComponent<Image>();
		frontPanel = topGroup.GetChild(0).gameObject.GetComponent<Image>();
		regressiveRulePanel = topGroup.GetChild(1).gameObject.GetComponent<Image>();
		regressiveChangeText = topGroup.GetChild(1).gameObject.GetComponentInChildren<TextMeshProUGUI>();
		progressiveRulePanel = topGroup.GetChild(2).gameObject.GetComponent<Image>();;
		progressiveChangeText = topGroup.GetChild(2).gameObject.GetComponentInChildren<TextMeshProUGUI>();
		descriptionText = topGroup.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

		progressionArrow = topGroup.GetChild(4).gameObject.GetComponent<Image>();
		progressionSlotHolder = topGroup.GetChild(5);


		panelOffset = backPanel.transform.localPosition;

		inactiveGreyColor = backPanel.color;
	}

	public void SetRuleChangeButton(Rule currentRule, Rule newRule) {
		buttonClicked = false;
		Unpress();

		presentRule = currentRule;
		potentialRule = newRule;

		Rule pro;
		Rule reg;
		if(currentRule.ruleIndex < newRule.ruleIndex) {
			progressive = true;

			pro = newRule;
			reg = currentRule;

			progressionArrow.transform.localEulerAngles = Vector3.zero;
		} else {
			progressive = false;

			pro = currentRule;
			reg = newRule;

			progressionArrow.transform.localEulerAngles = new Vector3(0, 180, 0);
		}

		progressiveChangeText.text = pro.title;
		regressiveChangeText.text = reg.title;

        descriptionText.text = newRule.description;

		int numCircles = modeController.GetRuleSet().GetRuleSlot(currentRule.ruleID).possibleRules.Count;

		for(int i = 0; i < progressionSlotHolder.childCount; i++) {
			GameObject slot = progressionSlotHolder.GetChild(i).gameObject;
			Image img = slot.GetComponent<Image>();
			if(i < numCircles) {
				slot.SetActive(true);
				img.color = Color.white;

				if(currentRule.ruleIndex == i) {
					img.sprite = activeRuleCircle;
				} else {
					img.sprite = inactiveRuleCircle;
				}


			} else {
				slot.SetActive(false);
			}
		}

		ResetColors();

		/*
        ruleButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ruleButton.GetComponent<Button>().onClick.AddListener(() => modeController.SelectNewGauntletRule(newRule));
		*/
	}
}
