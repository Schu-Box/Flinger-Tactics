using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData {
	public TeamMatchData homeTeamData;
	public TeamMatchData awayTeamData;

	public MatchData(Team homeTeam, Team awayTeam) {
		homeTeamData = new TeamMatchData(homeTeam);
		awayTeamData = new TeamMatchData(awayTeam);
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

	public Athlete GetMVP() {
		List<AthleteMatchData> allAthletes = new List<AthleteMatchData>();
		for(int i = 0; i < homeTeamData.athleteMatchData.Count; i++) {
			allAthletes.Add(homeTeamData.athleteMatchData[i]);
		}
		for(int i = 0; i < awayTeamData.athleteMatchData.Count; i++) {
			allAthletes.Add(awayTeamData.athleteMatchData[i]);
		}


		Athlete bestAthlete = null;
		int highestTotal = -1;
		for(int a = 0; a < allAthletes.Count; a++) {
			if(allAthletes[a].GetTotalStatSum() > highestTotal) {
				bestAthlete = allAthletes[a].GetAthlete();
				highestTotal = allAthletes[a].GetTotalStatSum();
			}
		}
		
		return bestAthlete;
	}
}

public class TeamMatchData {
	public Team team;

	public List<AthleteMatchData> athleteMatchData = new List<AthleteMatchData>();

	public TeamMatchData(Team t) {
		team = t;
		for(int i = 0; i < team.athletes.Count; i++) {
			athleteMatchData.Add(new AthleteMatchData(team.athletes[i]));
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

	public Athlete GetBestPerformerForStat(StatType stat) {
		Athlete bestAthlete = null;
		int currentHighest = -1;

		for(int a = 0; a < athleteMatchData.Count; a++) { //for each athlete
			if(athleteMatchData[a].GetStatCount(stat) > currentHighest) {
				bestAthlete = athleteMatchData[a].GetAthlete();
				currentHighest = athleteMatchData[a].GetStatCount(stat);
			}
		}

		return bestAthlete;
	}
}

public class AthleteMatchData {
	private Athlete athlete;

	public List<Stat> statList = new List<Stat>();

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

		Debug.Log("Never should have come here.");
		return -1;
	}

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

	/*
	public Stat GetBestStat() {
		Stat bestStat = null;
		int bestValue = 0;
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetPointValueSum() > bestValue) {
				bestValue = statList[i].GetPointValueSum();
				bestStat = statList[i];
			}
		}

		return bestStat;
	}

	public Stat GetSecondBestStat() {
		List<Stat> remainingStatList = new List<Stat>();
		for(int i = 0; i < statList.Count; i++) {
			remainingStatList.Add(statList[i]);
		}
		remainingStatList.Remove(GetBestStat());

		Stat secondBestStat = null;
		int bestValue = 0;
		for(int i = 0; i < remainingStatList.Count; i++) {
			if(remainingStatList[i].GetPointValueSum() > bestValue) {
				bestValue = remainingStatList[i].GetPointValueSum();
				secondBestStat = remainingStatList[i];
			}
		}

		return secondBestStat;
	}
	*/
}
