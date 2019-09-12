using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CustomRuleChanger : MonoBehaviour {

	private ModeController modeController;
	private CanvasManager canvasManager;

	private RuleSet ruleSet; //unused atm

	private RuleSlot ruleSlot;

	private Transform basePanel;
	private TextMeshProUGUI ruleNameText;
	private TextMeshProUGUI currentRuleText;
	private GameObject leftArrow;
	private GameObject rightArrow;

	private Vector3 restPosition;
	private float bumpDistance = 15f;

	public void SetCustomRuleChanger(RuleSet rules, RuleSlot slot) {
		modeController = FindObjectOfType<ModeController>();
		canvasManager = FindObjectOfType<CanvasManager>();

		basePanel = transform.GetChild(0);
		ruleNameText = basePanel.GetChild(0).GetComponent<TextMeshProUGUI>();
		currentRuleText = basePanel.GetChild(1).GetComponent<TextMeshProUGUI>();
		leftArrow = transform.GetChild(1).gameObject;
		rightArrow = transform.GetChild(2).gameObject;
		
		ruleSet = rules;
		ruleSlot = slot;

		ruleNameText.text = slot.ruleDescriptor + " : ";
		currentRuleText.text = slot.GetCurrentRule().value.ToString();

		restPosition = basePanel.transform.localPosition;

		if(ruleSlot.GetNextRule() == null) {
			rightArrow.SetActive(false);
		}

		if(ruleSlot.GetPreviousRule() == null) {
			leftArrow.SetActive(false);
		}
	}

	public RuleSlot GetRuleSlot() {
		return ruleSlot;
	}

	public void CycleRule(bool increase) {
		Rule newRule;

		if(increase) {
			newRule = ruleSlot.GetNextRule();
		} else {
			newRule = ruleSlot.GetPreviousRule();
		}

		modeController.GetRuleSet().ChangeRule(newRule);
		ruleNameText.text = ruleSlot.ruleDescriptor + ":";
		currentRuleText.text = ruleSlot.GetCurrentRule().value.ToString();

		canvasManager.UpdateCustomRuleChangers();

		StartCoroutine(Bump(increase));

		if(increase) {
			if(ruleSlot.GetNextRule() == null) {
				DisableArrow(true);
			}

			if(!leftArrow.gameObject.activeSelf) {
				EnableArrow(false);
			}
		} else {
			if(ruleSlot.GetPreviousRule() == null) {
				DisableArrow(false);
			}

			if(!rightArrow.gameObject.activeSelf) {
				EnableArrow(true);
			}
		}
	}

	public void DisableArrow(bool right) {
		if(right) {
			rightArrow.SetActive(false);
		} else {
			leftArrow.SetActive(false);
		}
	}

	public void EnableArrow(bool right) {
		if(right) {
			rightArrow.SetActive(true);
		} else {
			leftArrow.SetActive(true);
		}
	}

	private IEnumerator Bump(bool right) {
		
		Vector3 bumpEndPosition = restPosition;
		if(!right) {
			bumpEndPosition.x -= bumpDistance;
		} else {
			bumpEndPosition.x += bumpDistance;
		}

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.05f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			basePanel.transform.localPosition = Vector3.Lerp(restPosition, bumpEndPosition, timer/duration);

			yield return waiter;
		}

		duration = 0.1f;
		timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			basePanel.transform.localPosition = Vector3.Lerp(bumpEndPosition, restPosition, timer/duration);

			yield return waiter;
		}

		basePanel.transform.localPosition = restPosition;
	}
}
