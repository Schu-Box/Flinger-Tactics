using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSet {
	public List<RuleSlot> ruleSlotList = new List<RuleSlot>();

	public RuleSet() {
		ruleSlotList.Add(new RuleSlot("ballCount", new List<Rule> { 
			new Rule(1, "Reduce the number of balls on the field to 1."),
			new Rule(2, "Change the number of balls on the field to 2."),
			new Rule(3, "Change the number of balls on the field to 3."),
			new Rule(4, "Change the number of balls on the field to 4."),
			new Rule(5, "Increase the number of balls on the field to 5.")
		}));
		ruleSlotList.Add(new RuleSlot("athleteOnFieldCount", new List<Rule> { 
			new Rule(1, "Reduce the number of athletes on the field per team to 1."),
			new Rule(2, "Change the number of athletes on the field per team to 2."),
			new Rule(3, "Change the number of athletes on the field per team to 3."),
			new Rule(4, "Change the number of athletes on the field per team to 4."),
			new Rule(5, "Change the number of athletes on the field per team to 5."),
			new Rule(6, "Increase the number of athletes on the field per team to 6.")
		}));

		//This one should never go down
		ruleSlotList.Add(new RuleSlot("athleteOnRosterCount", new List<Rule> { 
			new Rule(1, "Increase the number of athletes on each team's roster to 2."),
			new Rule(2, "Increase the number of athletes on each team's roster to 3."),
			new Rule(3, "Increase the number of athletes on each team's roster to 4."),
			new Rule(4, "Increase the number of athletes on each team's roster to 5."),
			new Rule(5, "Increase the number of athletes on each team's roster to 6."),
			new Rule(6, "Increase the number of athletes on each team's roster to 7.")
		}));
	}

	public Rule GetRule(string id) {
		for(int i = 0; i < ruleSlotList.Count; i++) {
			if(ruleSlotList[i].ruleID == id) {
				return ruleSlotList[i].GetCurrentRule();
			}
		}

		Debug.Log("No that ain't a rule, fool.");
		return null;
	}

	public Rule GetRandomRuleChange() {
		Rule randoRule = ruleSlotList[Random.Range(0, ruleSlotList.Count)].GetRuleChange();

		return randoRule;
	}

	public void ChangeRule(Rule rule) {
		for(int i = 0; i < ruleSlotList.Count; i++) {
			if(ruleSlotList[i].ruleID == rule.ruleID) {
				ruleSlotList[i].SetCurrentRule(rule);
				break;
			}
		}
	}
}

public class RuleSlot {
	public string ruleID;
	public List<Rule> possibleRules = new List<Rule>();
	private Rule currentRule;

	public RuleSlot(string id, List<Rule> rules) {
		ruleID = id;
		possibleRules = rules;

		for(int i = 0; i < possibleRules.Count; i++) { //Sets each rule's ID to be the same as the RuleSlot's ID
			possibleRules[i].ruleID = ruleID;
		}

		currentRule = possibleRules[0];
	}

	public Rule GetCurrentRule() {
		return currentRule;
	}

	public void SetCurrentRule(Rule rule) {
		currentRule = rule;
	}

	public Rule GetRuleChange() {
		int step = 0;
		for(int i = 0; i < possibleRules.Count; i++) {
			if(currentRule == possibleRules[i]) {
				step = i;
			}
		}

		if(step < possibleRules.Count - 1) {
			return possibleRules[step + 1];
		} else {
			return possibleRules[step - 1];
		}
	}
}

public class Rule {
	public string ruleID;
	public int value;
	public string description;

	public Rule(int v, string desc) {
		value = v;
		description = desc;
	}
}
