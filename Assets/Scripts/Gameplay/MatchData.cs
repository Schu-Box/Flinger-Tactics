using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeasonData {
	public List<MatchData> matches;

	public SeasonData(List<MatchData> startMatches) {
		matches = startMatches;
	}

	public void AddMatches(List<MatchData> additionalMatches) {
		for(int i = 0; i < additionalMatches.Count; i++) {
			matches.Add(additionalMatches[i]);
		}
	}

	
}

public class MatchData {
	public TeamMatchData homeTeamData;
	public TeamMatchData awayTeamData;

	private Team winner;

	private RuleSet ruleSet;

	public MatchData(Team homeTeam, Team awayTeam, RuleSet rulesUsed) {
		homeTeamData = new TeamMatchData(homeTeam);
		awayTeamData = new TeamMatchData(awayTeam);

		homeTeam.AssignNewMatchData(this);
		awayTeam.AssignNewMatchData(this);

		ruleSet = rulesUsed;
	}

	public TeamMatchData GetTeamMatchData(Team team) {
		if(team == homeTeamData.team) {
			return homeTeamData;
		} else if(team == awayTeamData.team) {
			return awayTeamData;
		} else {
			Debug.Log("That team wasn't in there");
			return null;
		}
	}

	public Athlete GetBestAthlete(List<AthleteMatchData> athleteDataList) {
		Athlete bestAthlete = null;
		int highestTotal = -1;
		for(int a = 0; a < athleteDataList.Count; a++) {
			if(athleteDataList[a].GetTotalStatSum() > highestTotal) {
				bestAthlete = athleteDataList[a].GetAthlete();
				highestTotal = athleteDataList[a].GetTotalStatSum();
			}
		}

		return bestAthlete;
	}

	public Athlete GetMVP() {
		List<AthleteMatchData> allAthletes = new List<AthleteMatchData>();
		for(int i = 0; i < homeTeamData.athleteMatchData.Count; i++) {
			allAthletes.Add(homeTeamData.athleteMatchData[i]);
		}
		for(int i = 0; i < awayTeamData.athleteMatchData.Count; i++) {
			allAthletes.Add(awayTeamData.athleteMatchData[i]);
		}

		return GetBestAthlete(allAthletes);
	}

	public Athlete GetHomeMVP() {
		return GetBestAthlete(homeTeamData.athleteMatchData);
	}

	public Athlete GetAwayMVP() {
		return GetBestAthlete(awayTeamData.athleteMatchData);
	}

	public void FinalizeMatchData() {
		if(homeTeamData.GetScore() > awayTeamData.GetScore()) {
			winner = homeTeamData.team;
			homeTeamData.SetWin(true);
			awayTeamData.SetWin(false);
		} else if (homeTeamData.GetScore() == awayTeamData.GetScore()) {
			Debug.Log("Error: The game is tied at the end of the match");
		} else {
			winner = awayTeamData.team;
			homeTeamData.SetWin(false);
			awayTeamData.SetWin(true);
		}

		homeTeamData.SetMatchDescriptors();
		awayTeamData.SetMatchDescriptors();
	}

	public Team GetWinner() {
		return winner;
	}

	public void SimulateMatch() {

		//Right now it's pretty much just random results - can also include impossible results (4 goals, 0 breaks)
		for(int i = 0; i < ruleSet.GetRule("turnCount").value; i++) {
			Athlete homeAthlete = homeTeamData.team.athletes[Random.Range(0, homeTeamData.team.athletes.Count)];
			SimulateAthleteTurn(homeAthlete);
			
			Athlete awayAthlete = awayTeamData.team.athletes[Random.Range(0, awayTeamData.team.athletes.Count)];
			SimulateAthleteTurn(awayAthlete);
		}

		if(homeTeamData.GetScore() == awayTeamData.GetScore()) {
			Debug.Log("The teams are still tied in simulation, I'll fix it tho");
			Athlete fakeScorer = homeTeamData.team.athletes[0];
			fakeScorer.IncreaseStat(StatType.Goals);
			homeTeamData.IncreaseScore(1);
		}

		FinalizeMatchData();
	}

	public void SimulateAthleteTurn(Athlete a) {
		if(Random.value > 0.1f) { //else it was a sub
			a.IncreaseStat(StatType.Flings);
		}

		float rando = Random.value;
		if(rando > 0.8) { //goal 20%
			a.IncreaseStat(StatType.Goals);
			a.GetTeam().GetCurrentMatchData().GetTeamMatchData(a.GetTeam()).IncreaseScore(1);

			Athlete randomTeammate = a;
			while(randomTeammate == a) { //Should never cause a error cuz teams will always consist of at the very least 2 athletes
				randomTeammate = a.GetTeam().athletes[Random.Range(0, a.GetTeam().athletes.Count)];
			}
			if(Random.value > 0.9) {
				randomTeammate.IncreaseStat(StatType.Assists);
			}
		} else if (rando > 0.4) { // break 40%
			a.IncreaseStat(StatType.Breaks);
		} else if (rando > 0.15) { // clear 25%
			a.IncreaseStat(StatType.Clears);
		}

		for(int r = 0; r < Random.Range(0, 3); r++) {
			a.IncreaseStat(StatType.Touches);
		}

		for(int r = 0; r < Random.Range(0, 3); r++) {
			a.IncreaseStat(StatType.Tackles);
		}

		for(int r = 0; r < Random.Range(0, 8); r++) {
			a.IncreaseStat(StatType.Bumps);
		}

		switch(a.athleteData.classString) {
			case "Circle":
				if(Random.value > 0.65) { //35%
					a.IncreaseStat(StatType.Shockwaves);
				}
				break;
			case "Box":
				if(Random.value > 0.8) { //20%
					a.IncreaseStat(StatType.Repairs);
				}
				break;
			case "Triangle":
				if(Random.value > 0.6) { //40%
					a.IncreaseStat(StatType.Knockouts);
				}
				break;
			default:
				Debug.Log("Class doesn't exist.");
				break;
		}
	}
}

public class TeamMatchData {
	public Team team;
	public List<AthleteMatchData> athleteMatchData = new List<AthleteMatchData>(); //should probably be AMDlist

	private bool wonThisGame = false;
	private int score = 0;
	private int numTimeouts = 1;

	//There's definitely a much better way of doing this but whatever
	public List<StatType> statTypesList = new List<StatType>();

	public TeamMatchData(Team t) {
		team = t;

		for(int i = 0; i < team.athletes.Count; i++) {
			athleteMatchData.Add(new AthleteMatchData(team.athletes[i]));
		}

		//Set the statTypesList (although this doesn't need to be repeated for every team and match so it should probably live elsewhere
		for(int i = 0; i < athleteMatchData.Count; i++) {
			for(int j = 0; j < athleteMatchData[i].statList.Count; j++) {
				StatType st = athleteMatchData[i].statList[j].GetStatType();
				bool alreadyOnList = false;
				for(int s = 0; s < statTypesList.Count; s++) {
					if(st == statTypesList[s]) {
						alreadyOnList = true;
						break;
					}
				}
				if(!alreadyOnList) {
					statTypesList.Add(st);
				}
			}
		}
	}

	public AthleteMatchData GetAthleteMatchData(Athlete athlete) {
		AthleteMatchData amd = null;
		for(int i = 0; i < athleteMatchData.Count; i++) {
			if(athleteMatchData[i].GetAthlete() == athlete) {
				amd = athleteMatchData[i];
			}
		}
		return amd;
	}

	public List<Stat> GetTeamTotalStatList() {
		List<Stat> teamTotalStatList = new List<Stat>();

		Athlete ath = team.athletes[0];
		for(int i = 0; i < ath.statList.Count; i++) {
			teamTotalStatList.Add(new Stat(ath.statList[i].GetStatType(), ath.statList[i].GetPointValueInteger()));
			for(int a = 0; a < athleteMatchData.Count; a++) {
				teamTotalStatList[i].IncreaseByNum(athleteMatchData[a].statList[i].GetCount());
			}
		}

		return teamTotalStatList;
	}

	public bool DidTeamWin() {
		return wonThisGame;
	}

	public void SetWin(bool wonIt) {
		wonThisGame = wonIt;
	}

	public int GetScore() {
		return score;
	}

	public void IncreaseScore(int num) {
		score += num;
	}

	public bool HasTimeout() {
		if(numTimeouts > 0) {
			return true;
		} else {
			return false;
		}
	}

	public void LoseTimeout() {
		numTimeouts--;

		if(numTimeouts < 0) {
			Debug.Log("You used an extra timeout. You can't do that. There's literally less than zero.");
		}
	}

	public void SetMatchDescriptors() {
		for(int i = 0; i < statTypesList.Count; i++) {
			List<AthleteMatchData> bestAtStat = GetBestAtStat(statTypesList[i]);
			for(int a = 0; a < bestAtStat.Count; a++) {
				bestAtStat[a].AddTopStat(statTypesList[i]);
			}
		}

		List<AthleteMatchData> athletesNotInOrder = new List<AthleteMatchData>();
		for(int i = 0; i < athleteMatchData.Count; i++) {
			athletesNotInOrder.Add(athleteMatchData[i]);
		}

		List<AthleteMatchData> descriptorDeterminatorOrder = new List<AthleteMatchData>();
		for(int a = 0; a < athleteMatchData.Count; a++) {
			int lowestTopStats = 999;
			AthleteMatchData lowestAthlete = null;
			for(int i = 0; i < athletesNotInOrder.Count; i++) {
				AthleteMatchData amd = athletesNotInOrder[i];
				int count = amd.topStats.Count;
				if(count < lowestTopStats) {
					lowestTopStats = count;
					lowestAthlete = amd;
				}
			}

			descriptorDeterminatorOrder.Add(lowestAthlete);
			athletesNotInOrder.Remove(lowestAthlete);
		}


		List<StatType> statTypesUsed = new List<StatType>();
		for(int i = 0; i < descriptorDeterminatorOrder.Count; i++) {
			AthleteMatchData amd = descriptorDeterminatorOrder[i];
			int topStatCount = amd.topStats.Count;
			if(topStatCount == 0) {
				bool noRecordedStats = true;
				for(int j = 0; j < amd.statList.Count; j++) {
					if(amd.statList[j].GetCount() > 0) { //this assumes that all stats begin at 0
						noRecordedStats = false;
						break;
					}
				}

				if(noRecordedStats) {
					amd.AddNewDescriptor("Bench Warmer");
				} else {
					if(amd.GetStatCount(StatType.Flings) == 0){
						amd.AddNewDescriptor("Feels Neglected");
					}

					if(amd.GetStatCount(StatType.Goals) > 0) {
						amd.AddNewDescriptor("Striker");
					}

					if(amd.GetStatCount(StatType.Assists) > 0) {
						amd.AddNewDescriptor("Playmaker");
					}

					if(amd.GetStatCount(StatType.Breaks) > 0) {
						amd.AddNewDescriptor("Breaker");
					}

					if(amd.GetStatCount(StatType.Clears) > 0) {
						amd.AddNewDescriptor("Defender");
					}

					if(amd.firstDescriptor == "") {
						amd.AddNewDescriptor("Participant");
					}
				}
			} else if(topStatCount == 1) {
				amd.AddNewDescriptor(amd.topStats[0]);
				statTypesUsed.Add(amd.topStats[0]);
			} else if(topStatCount == 2) {
				amd.AddNewDescriptor(amd.topStats[0]);
				statTypesUsed.Add(amd.topStats[0]);
				amd.AddNewDescriptor(amd.topStats[1]);
				statTypesUsed.Add(amd.topStats[1]);
			} else {
				List<StatType> priorityStatTypes = new List<StatType>();
				for(int s = 0; s < amd.topStats.Count; s++) {
					StatType st = amd.topStats[s];
					bool alreadyUsed = false;
					for(int k = 0; k < statTypesUsed.Count; k++) {
						if(st == statTypesUsed[k]) {
							alreadyUsed = true;
							break;
						}
					}
					if(!alreadyUsed) {
						priorityStatTypes.Add(st);
					}
				}

				if(priorityStatTypes.Count >= 2) {
					StatType firstStat = priorityStatTypes[0];
					priorityStatTypes.Remove(firstStat);
					amd.AddNewDescriptor(firstStat);
					statTypesUsed.Add(firstStat);

					StatType secondStat = priorityStatTypes[0];
					amd.AddNewDescriptor(secondStat);
					statTypesUsed.Add(secondStat);
				} else if(priorityStatTypes.Count == 1) {
					amd.AddNewDescriptor(priorityStatTypes[0]);
					statTypesUsed.Add(priorityStatTypes[0]);
				} else {
					StatType firstStat = amd.topStats[0];
					amd.AddNewDescriptor(firstStat);
					
					StatType secondStat;
					if(amd.topStats.Count > 1) {
						secondStat = amd.topStats[1];
						amd.AddNewDescriptor(secondStat);
					} else {
						Debug.Log("need to come up with a proper second stat?");
					}
				}
			}
		}
	}

	public List<AthleteMatchData> GetBestAtStat(StatType stat) {
		int mostOfStat = 1;
		List<AthleteMatchData> topAthletes = new List<AthleteMatchData>();
		for(int i = 0; i < athleteMatchData.Count; i++) {
			AthleteMatchData a = athleteMatchData[i];
			int statcount = a.GetStatCount(stat);
			if(statcount > mostOfStat) {
				topAthletes = new List<AthleteMatchData>();
				topAthletes.Add(a);
				mostOfStat = statcount;
			} else if(statcount == mostOfStat) {
				topAthletes.Add(a);
			}
		}

		return topAthletes;
	}
}

public class AthleteMatchData {
	private Athlete athlete;

	public List<Stat> statList = new List<Stat>();

	public List<StatType> topStats = new List<StatType>();

	public string firstDescriptor = "";
	public string secondDescriptor = "";

	public AthleteMatchData(Athlete a) {
		athlete = a;
		for(int i = 0; i < athlete.statList.Count; i++) {
			statList.Add(new Stat(athlete.statList[i].GetStatType(), athlete.statList[i].GetPointValueInteger()));
			statList[i].ResetCount();
		}
	}

	public Athlete GetAthlete() {
		return athlete;
	}

	/*
	public Stat GetStat(StatType type) {
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetStatType() == type) {
				return statList[i];
			}
		}

		Debug.Log("What stat is that?");
		return null;
	}
	*/

	public int GetTotalStatSum() {
		int total = 0;
		for(int i = 0; i < statList.Count; i++) {
			total += statList[i].GetPointValueSum();
		}

		return total;
	}

	public int GetStatCount(StatType statType) {
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetStatType() == statType) {
				return statList[i].GetCount();
			}
		}

		//Debug.Log("Never should have come here with that stat you call " + statType.ToString());
		return -1;
	}

	public int GetStatPointsValue(StatType statType) {
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetStatType() == statType) {
				return statList[i].GetPointValueSum();
			}
		}
		
		Debug.Log("Yo there is no stat there homie you wildin");
		return -1;
	}

	/*
	public Stat GetNthBestStat(int n) {
		List<Stat> remainingStats = new List<Stat>();
		for(int i = 0; i < statList.Count; i++) {
			remainingStats.Add(statList[i]);
		}

		Stat nthStat = null;
		for(int i = 0 ; i < n; i++) {
			Stat bestStat = null;
			int bestValue = -1;
			for(int j = 0; j < remainingStats.Count; j++) {
				if(remainingStats[j].GetPointValueSum() > bestValue) {
					bestValue = remainingStats[j].GetPointValueSum();
					bestStat = remainingStats[j];
				}
			}

			nthStat = bestStat;
			remainingStats.Remove(bestStat);
		}

		return nthStat;
	}
	*/

	public void AddTopStat(StatType st) {
		topStats.Add(st);
	}

	public void AddNewDescriptor(StatType st) {
		switch(st.ToString()) {
			case "Goals":
				AddNewDescriptor("Top Striker");
				break;
			case "Assists":
				AddNewDescriptor("Elite Playmaker");
				break;
			case "Breaks":
				AddNewDescriptor("Break Master");
				break;
			case "Clears":
				AddNewDescriptor("Lockdown Defender");
				break;
			case "Touches":
				AddNewDescriptor("Ball Chaser");
				break;
			case "Tackles":
				AddNewDescriptor("Tough Tackler");
				break;
			case "Flings":
				AddNewDescriptor("Coach's Favorite");
				break;
			case "Bumps":
				AddNewDescriptor("Best Bumper");
				break;
			case "Shockwaves":
				AddNewDescriptor("Super Shockwaver");
				break;
			case "Repairs":
				AddNewDescriptor("Expert Mechanic");
				break;
			case "Knockouts":
				AddNewDescriptor("Most Annoying");
				break;
			default:
				AddNewDescriptor("I dunno?");
				break;
		}
	}

	public void AddNewDescriptor(string descriptor) {
		if(firstDescriptor == "") {
			firstDescriptor = descriptor;
		} else if(secondDescriptor == "") {
			secondDescriptor = descriptor;
		} //else just ignore it
	}
}
