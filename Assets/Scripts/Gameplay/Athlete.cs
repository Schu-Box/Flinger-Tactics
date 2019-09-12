﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Team {

	public bool computerControlled; //Needs to be private
    public string name;
	public string nameLocation;
	public string nameNickname;
	public string fieldString;
    public Color primaryColor;
	public Color secondaryColor;
	private Color lightTint;
	private Color darkTint;

	private int jerseyNum = -1;

	private List<MatchData> matchData = new List<MatchData>();

    public List<Athlete> athletes = new List<Athlete>();

	public void SetNewRoster(int numAthletes) {
		athletes = new List<Athlete>();
		
		for(int i = 0; i < numAthletes; i++) {
			athletes.Add(new Athlete());
			athletes[i].SetTeam(this);
		}

		lightTint = Color.Lerp(primaryColor, Color.white, 0.7f);
		darkTint = Color.Lerp(primaryColor, Color.black, 0.2f);
	}

	public void AddAthleteToRoster() {
		athletes.Add(new Athlete());
		athletes[athletes.Count - 1].SetTeam(this);
	}

	public void AssignNewMatchData(MatchData match) {
		matchData.Add(match);
	}

	public MatchData GetCurrentMatchData() {
		return matchData[matchData.Count - 1];
	}

	public Color GetLightTint() {
		return lightTint;
	}

	public Color GetDarkTint() {
		return darkTint;
	}
}

public class Athlete {

	public AthleteData athleteData;

	private Personality personality;

	public string name;
	public Color skinColor;
	public Color bodySkinColor;
	public Color tongueColor;

	public int jerseyNumber;

	public float flingForce = 800f;

	public float minPull = 0.3f;
	public float maxPull = 1.2f;

	private Team team;

	public List<Stat> statList = new List<Stat>();

	public Sprite moodFace;

	public Athlete() {
		AthleteData standardAthleteData = Resources.Load<AthleteData>("StandardAthleteData");

		float rando = Random.value;
		if(rando > 0.5f) {
			athleteData = Resources.Load<AthleteData>("CircleAthleteData");
		} else if(rando > 0.25) {
			athleteData = Resources.Load<AthleteData>("TriangleAthleteData");
		} else {
			athleteData = Resources.Load<AthleteData>("BoxAthleteData");
		} 
		//athleteData.AddSkinColors(standardAthleteData.skinColorList);

		List<string> nameList = athleteData.nameList;
		name = nameList[Random.Range(0, nameList.Count)];

		List<Color> colorList = athleteData.skinColorList;
		skinColor = colorList[Random.Range(0, colorList.Count)];
		
		float randito = (Random.value / 4) + 0.2f; //Between 0.2 and 0.45
		bodySkinColor = Color.Lerp(skinColor, Color.black, randito);

		tongueColor = colorList[Random.Range(0, colorList.Count)];

		rando = Random.value;
		if(rando == 1) {
			jerseyNumber = 420;
		} else if(rando > 0.6) {
			jerseyNumber = Random.Range(0, 22);
		} else if(rando > 0.3) {
			jerseyNumber = Random.Range(0, 36);
		} else {
			jerseyNumber = Random.Range(36, 100);
		}

		//Stats
		statList.Add(new Stat(StatType.Goals, 30));
		statList.Add(new Stat(StatType.Assists, 20));
		statList.Add(new Stat(StatType.Breaks, 10));
		statList.Add(new Stat(StatType.Clears, 5));
		statList.Add(new Stat(StatType.Touches, 3));
		statList.Add(new Stat(StatType.Tackles, 2));
		statList.Add(new Stat(StatType.Flings, 1));
		statList.Add(new Stat(StatType.Bumps, 1));

		if(athleteData.classString == "Circle") {
			statList.Add(new Stat(StatType.Shockwaves, 3));
		} else if(athleteData.classString == "Box") {
			statList.Add(new Stat(StatType.Repairs, 10));
		} else if(athleteData.classString == "Triangle") {
			statList.Add(new Stat(StatType.Knockouts, 5));
		}

		//statList.Add(new Stat("Bounces", 2));

		List<Personality> personalityList = athleteData.personalityList;
		personality = personalityList[Random.Range(0, personalityList.Count)];
	}

	public void SetTeam(Team t) {
		team = t;
	}

	public Team GetTeam() {
		return team;
	}

	public void IncreaseStat(StatType statType) {
		Stat stat = null;
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetStatType() == statType) {
				stat = statList[i];
				break;
			}
		}

		if(stat != null) {
			stat.IncreaseCount();
		} else {
			Debug.Log("That stat doesn't exist bruh");
		}

		Stat matchStat = null;
		for(int i = 0; i < GetTeam().GetCurrentMatchData().GetTeamMatchData(team).athleteMatchData.Count; i++) {
			AthleteMatchData amd = GetTeam().GetCurrentMatchData().GetTeamMatchData(team).athleteMatchData[i];
			if(amd.GetAthlete() == this) {
				for(int j = 0; j < amd.statList.Count; j++) {
					if(amd.statList[j].GetStatType() == statType) {
						matchStat = amd.statList[j];
						break;
					}
				}
			}
		}

		if(matchStat != null) {
			matchStat.IncreaseCount();
		} else {
			Debug.Log("Match stat don't exist either.");
		}
	}

	public int GetStatPointTotal() {
		int total = 0;
		for(int i = 0; i < statList.Count; i++) {
			total += statList[i].GetPointValueSum();
		}
		return total;
	}

	public Personality GetPersonality() {
		return personality;
	}

	public string GetQuote(string quoteID) {
		List<string> possibleQuotes = new List<string>();
		
		switch(quoteID) {
			case "preMatchTeam":
				possibleQuotes = personality.preMatchTeamQuoteList;
				break;
			case "preMatchOpponent":
				possibleQuotes = personality.preMatchOpponentQuoteList;
				break;
			case "preFlingTeam":
				possibleQuotes = personality.preFlingTeamQuoteList;
				break;
			case "preFlingOpponent":
				possibleQuotes = personality.preFlingOpponentQuoteList;
				break;
			case "goal":
				possibleQuotes = personality.goalQuoteList;
				break;
			case "ownGoal":
				possibleQuotes = personality.ownGoalQuoteList;
				break;
			case "postMatchWin":
				possibleQuotes = personality.postMatchWinQuoteList;
				break;
			case "postMatchLose":
				possibleQuotes = personality.postMatchLoseQuoteList;
				break;
			case "substitute":
				possibleQuotes = personality.substituteQuoteList;
				break;

			default:
				return "That quote doesn't exist";
		}

		return possibleQuotes[Random.Range(0, possibleQuotes.Count)];
	}
}

public enum StatType {
	Goals,
	Assists,
	Breaks,
	Clears,
	Touches,
	Tackles,
	Flings,
	Bumps,
	Shockwaves,
	Repairs,
	Knockouts
}

public class Stat {

	StatType statType;
	string statString;
	int count;

	int pointValue;

	public Stat(StatType type, int points) {
		statType = type;
		statString = statType.ToString();
		count = 0;
		pointValue = points;
	}

	public string GetStatName() {
		return statString;
	}
	
	public StatType GetStatType() {
		return statType;
	}

	public int GetCount() {
		return count;
	}

	public void IncreaseCount() {
		count++;
	}
	
	public void IncreaseByNum(int num) {
		count += num;
	}

	public void ResetCount() { //Should only be used for copying MatchData
		count = 0;
	}

	public int GetPointValueSum() {
		return (count * pointValue);
	}

	public int GetPointValueInteger() {
		return pointValue;
	}
}