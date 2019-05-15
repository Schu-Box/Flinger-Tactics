using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomRuleChanger : MonoBehaviour {

	private ModeController modeController;

	private RuleSlot ruleSlot;
	private TextMeshProUGUI ruleNameText;
	private TextMeshProUGUI currentRuleText;

	public void SetCustomRuleChanger(RuleSlot slot) {
		modeController = FindObjectOfType<ModeController>();
		ruleNameText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		currentRuleText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
		
		ruleSlot = slot;

		ruleNameText.text = slot.ruleID;
		currentRuleText.text = slot.GetCurrentRule().value.ToString();
	}

	public void CycleRule() {
		Rule nextRule = ruleSlot.GetNextRule();

		modeController.GetRuleSet().ChangeRule(nextRule);

		ruleNameText.text = ruleSlot.ruleID;
		currentRuleText.text = ruleSlot.GetCurrentRule().value.ToString();
	}
}
