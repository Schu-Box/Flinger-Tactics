using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Team {
    public string name;
	public string fieldString;
    public Color primaryColor;
	private Color lightTint;
	private Color darkTint;

	private List<MatchData> matchData = new List<MatchData>();

    public List<Athlete> athletes = new List<Athlete>();

    //In Game
    public int score = 0;
    public bool wonTheGame = false;

	public void SetNewRoster(int num) {
		for(int i = 0; i < num; i++) {
			athletes.Add(new Athlete());
			athletes[i].SetTeam(this);
		}

		lightTint = Color.Lerp(primaryColor, Color.white, 0.7f);
		darkTint = Color.Lerp(primaryColor, Color.black, 0.2f);
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

[System.Serializable]
public class AthleteType {
	public string typeString;

	public float bumperModifier = 1f;

	public Sprite bodySprite;
	public Sprite faceBaseSprite;
	public Sprite frontLegSprite;
	public Sprite backLegSprite;

	public float frontLegRest = 10f;
	public float frontLegMin = 10.5f;
	public float frontLegMax = 12.5f;

	public float backLegRest = 10f;
	public float backLegMin = 10.5f;
	public float backLegMax = 12.5f;
}

public class Athlete {

	public AthleteData standardAthleteData;

	public string name;
	public AthleteType athleteType;
	public Color skinColor;
	public Color tongueColor;

	public int jerseyNumber;

	public float flingForce = 700f;

	public float minPull = 0.3f;
	public float maxPull = 1.2f;

	private Team team;

	public List<Stat> statList = new List<Stat>();

	public Sprite moodFace;

	public Athlete() {

		standardAthleteData = Resources.Load<AthleteData>("StandardAthleteData");

		List<string> nameList = standardAthleteData.nameList;
		name = nameList[Random.Range(0, nameList.Count)];

		List<Color> colorList = standardAthleteData.skinColorList;
		skinColor = colorList[Random.Range(0, colorList.Count)];

		tongueColor = colorList[Random.Range(0, colorList.Count)];

		List<AthleteType> athleteTypes = standardAthleteData.athleteTypes;

		float rando = Random.value;
		if(rando > 0.5f) {
			athleteType = athleteTypes[0];
		} else if(rando > 0.25) {
			athleteType = athleteTypes[1];
		} else {
			athleteType = athleteTypes[2];
		}

		rando = Random.value;
		if(rando > 0.6) {
			jerseyNumber = Random.Range(0, 22);
		} else if(rando > 0.3) {
			jerseyNumber = Random.Range(0, 36);
		} else {
			jerseyNumber = Random.Range(36, 100);
		}
		
		

		flingForce = 700f;

		//Stats
		statList.Add(new Stat(StatType.Goals, 30));
		statList.Add(new Stat(StatType.Assists, 20));
		statList.Add(new Stat(StatType.Breaks, 10));
		statList.Add(new Stat(StatType.Touches, 3));
		statList.Add(new Stat(StatType.Tackles, 2));
		statList.Add(new Stat(StatType.Flings, 1));
		statList.Add(new Stat(StatType.Bumps, 1));

		//statList.Add(new Stat("Bounces", 2));
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
}

public enum StatType {
	Goals,
	Assists,
	Breaks,
	Touches,
	Tackles,
	Flings,
	Bumps

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
