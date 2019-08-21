using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Personality", menuName = "Personality", order = 3)]
public class Personality : ScriptableObject {
	public string id;
	
	public List<string> preMatchTeamQuoteList = new List<string>();
	public List<string> preMatchOpponentQuoteList = new List<string>();
	public List<string> preFlingTeamQuoteList = new List<string>();
	public List<string> preFlingOpponentQuoteList = new List<string>();
	public List<string> goalQuoteList = new List<string>();
	public List<string> ownGoalQuoteList = new List<string>();
	public List<string> postMatchWinQuoteList = new List<string>();
	public List<string> postMatchLoseQuoteList = new List<string>();
	public List<string> substituteQuoteList = new List<string>();
}
