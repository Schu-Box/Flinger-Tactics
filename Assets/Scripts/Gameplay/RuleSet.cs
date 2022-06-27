using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RuleSet {
	public List<RuleSlot> ruleSlotList = new List<RuleSlot>();

	public RuleSet() {
		ruleSlotList.Add(new RuleSlot("ballCount", "Ball Count", new List<Rule> { 
			new Rule(1, "Single Ball", "Reduce the number of balls on the field to 1."),
			new Rule(2, "Double Ball", "Change the number of balls on the field to 2."),
			new Rule(3, "Triple Ball", "Change the number of balls on the field to 3."),
			new Rule(4, "Quadruple Ball", "Change the number of balls on the field to 4."),
			new Rule(5, "Maximum Balls", "Increase the number of balls on the field to 5.")
		}));

		ruleSlotList.Add(new RuleSlot("bumperCount", "Bumper Count", new List<Rule> {
			new Rule(1, "Bumper Wall", "this isn't used"),
			new Rule(2, "Double Bumpers", ""),
			new Rule(3, "", ""),
			new Rule(4, "", ""),
			new Rule(5, "", ""),
			new Rule(6, "", ""),
			new Rule(7, "", ""),
			new Rule(8, "", "")
		}));

		//This one should never go down (in career play, otherwise I'd have to do hella extra stuff)
		ruleSlotList.Add(new RuleSlot("athleteRosterCount", "Athlete Roster Count", new List<Rule> {
			new Rule(1, "Solo Roster", "Decrease the number of athletes on each team's roster to 1."),
			new Rule(2, "Duo Roster", "Increase the number of athletes on each team's roster to 2."),
			new Rule(3, "Trio Roster", "Increase the number of athletes on each team's roster to 3."),
			new Rule(4, "Four-Athlete Roster", "Increase the number of athletes on each team's roster to 4."),
			new Rule(5, "Five-Athlete Roster", "Increase the number of athletes on each team's roster to 5."),
			new Rule(6, "Six-Athlete Roster", "Increase the number of athletes on each team's roster to 6.")
		}));

		ruleSlotList.Add(new RuleSlot("athleteFieldCount", "Athlete Field Count", new List<Rule> { 
			new Rule(1, "Solo Athletes", "Reduce the number of athletes on the field per team to 1."),
			new Rule(2, "Duo Athletes", "Change the number of athletes on the field per team to 2."),
			new Rule(3, "Trio Athletes", "Change the number of athletes on the field per team to 3."),
			new Rule(4, "Four Athletes", "Change the number of athletes on the field per team to 4."),
			new Rule(5, "Five Athletes", "Change the number of athletes on the field per team to 5."),
			new Rule(6, "Maximum Athletes", "Increase the number of athletes on the field per team to 6.")
		}));

		ruleSlotList.Add(new RuleSlot("turnCount", "Turn Count", new List<Rule> { 
			new Rule(5, "5 Turns", "Decrease the number of turns to 5."),
			new Rule(10, "10 Turns", "Change the number of turns to 10."),
			new Rule(15, "15 Turns", "Change the number of turns to 15."),
			new Rule(20, "20 Turns", "Change the number of turns to 20."),
			new Rule(25, "25 Turns", "Change the number of turns to 25."),
			new Rule(30, "30 Turns", "Change the number of turns to 30."),
			new Rule(50, "50 Turns", "Increase the number of turns to 50.")
			
			//new Rule(36, "36 Turns", "Change the number of turns to 36."),
			//new Rule(40, "40 Turns", "Change the number of turns to 40."),
			//new Rule(44, "44 Turns", "Change the number of turns to 44."),
			//new Rule(48, "48 Turns", "Change the number of turns to 48.")
		}));

		ruleSlotList.Add(new RuleSlot("teamCount", "Team Count", new List<Rule> {
			new Rule(4, "4 Teams", ""),
			new Rule(8, "8 Teams", ""),
			new Rule(16, "16 Teams", "")
		}));

		/*
		ruleSlotList.Add(new RuleSlot("drawResolution", new List<Rule> { 
			new Rule(0, "Golden Goal", "At the end of regulation if the score is tied, the next team to score in extra time wins."),
			new Rule(1, "Overtime Period", "At the end of regulation if the score is tied, an overtime period of 5 turns shall take place.")
		}));
		*/
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

	public RuleSlot GetRuleSlot(string id) {
		for(int i = 0; i < ruleSlotList.Count; i++) {
			if(ruleSlotList[i].ruleID == id) {
				return ruleSlotList[i];
			}
		}

		Debug.Log("No that ain't a rule, fool.");
		return null;
	}

	public Rule GetRandomRuleChange() {
		Rule randoRule = ruleSlotList[Random.Range(0, ruleSlotList.Count)].GetRuleChange();

		return randoRule;
	}

	public List<Rule> GetAvailableRuleChanges() {
		List<Rule> rules = new List<Rule>();

		rules.Add(GetRuleSlot("ballCount").GetRuleChange());

		if(GetRule("athleteRosterCount").value > GetRule("athleteFieldCount").value) {
			rules.Add(GetRuleSlot("athleteFieldCount").GetRuleChange());
		}
		
		rules.Add(GetRuleSlot("athleteRosterCount").GetRuleChange());
		rules.Add(GetRuleSlot("turnCount").GetRuleChange());
		rules.Add(GetRuleSlot("drawResolution").GetRuleChange());

		return rules;
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
	public string ruleDescriptor;
	public List<Rule> possibleRules = new List<Rule>();
	private Rule currentRule;

	public RuleSlot(string id, string desc, List<Rule> rules) {
		ruleID = id;
		ruleDescriptor = desc;
		possibleRules = rules;

		for(int i = 0; i < possibleRules.Count; i++) { //Sets each rule's ID to be the same as the RuleSlot's ID
			possibleRules[i].ruleID = ruleID;
			int index = i;
			possibleRules[i].ruleIndex = index;
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

	public Rule GetNextRule() {
		int index = currentRule.ruleIndex;
		if(index < possibleRules.Count - 1) {
			return possibleRules[index + 1];
		} else {
			//return possibleRules[0];
			return null;
		}
	}

	public Rule GetPreviousRule() {
		int index = currentRule.ruleIndex;
		if(index > 0) {
			return possibleRules[index - 1];
		} else {
			//return possibleRules[possibleRules.Count - 1];
			return null;
		}
	}
}

public class Rule {
	public string ruleID;
	public int ruleIndex;
	public int value;
	public string title;
	public string description;

	public Rule(int v, string tit, string desc) {
		value = v;
		title = tit;
		description = desc;
	}
}
