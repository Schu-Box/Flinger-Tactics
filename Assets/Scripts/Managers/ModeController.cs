using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ModeController : MonoBehaviour {

    private CameraController cameraController;
    private CanvasManager canvasManager;
    private AudioManager audioManager;
    private MatchController matchController;
    private ParticleManager particleManager;

    private RuleSet currentRuleSet = new RuleSet();
    public List<Team> teamList = new List<Team>();

    public static Team gauntletTeam;

    private List<Team> tournamentTeamList = new List<Team>();
    private List<MatchData> tournamentMatchDataList = new List<MatchData>();
    private List<MatchData> tournamentActiveMatchDataList = new List<MatchData>();
    private Team tournamentWinner = null;

    private int gauntletRound = 0;

    void Start() {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraController.startPosition;
        canvasManager = FindObjectOfType<CanvasManager>();
        audioManager = FindObjectOfType<AudioManager>();
        matchController = FindObjectOfType<MatchController>();
        particleManager = FindObjectOfType<ParticleManager>();

        canvasManager.DisplayPreMatch();

        string modeID = PlayerPrefs.GetString("mode");
        if(modeID == "playNow") {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("bumperCount").possibleRules[3]);

            SetNewRosters();

            matchController.SetupCourt(currentRuleSet);

            TeamSelectionPhase();
        } else if(modeID == "gauntlet") {
            Debug.Log("You're now in the gauntlet.");

            canvasManager.homeSelectionPanel.EnableInteraction(false);
            canvasManager.awaySelectionPanel.EnableInteraction(false);

            SetNewRosters();

            StartGauntlet();
        } else if (modeID == "competitive") {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("bumperCount").possibleRules[3]);

            SetNewRosters();

            matchController.SetupCourt(currentRuleSet);

            TeamSelectionPhase();

            canvasManager.SetCompetitiveMode();
        } else if (modeID == "tournament") {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("bumperCount").possibleRules[3]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("teamCount").possibleRules[2]);

            canvasManager.DisplayTournamentSetup();
        } else {
            Debug.Log("Null mode selected");
        }
    }

    void Update() {
        if(Input.GetButtonDown("Cancel")) {
            canvasManager.ToggleEscapeMenu();
        } else if(Input.GetButtonDown("Submit")) {
            //Unused
        } else if(Input.GetKeyDown(KeyCode.E)) {
            matchController.GetMatchData().SimulateMatch();
            matchController.EndMatch();
        } else if(Input.GetKeyDown(KeyCode.A) && !matchController.GetMatchStarted()) {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[6]);

            matchController.GetTeam(true).computerControlled = true;
            matchController.GetTeam(false).computerControlled = true;

            matchController.StartMatch();
        } else if(Input.GetKeyDown(KeyCode.S)) {
            CameraController.disableScreenShake = !CameraController.disableScreenShake;
        }
    }

    public void SetNewRosters() {
        for(int i = 0; i < teamList.Count; i++) {
            teamList[i].SetNewRoster(currentRuleSet.GetRule("athleteRosterCount").value);
        }
    }

    public void AddAthleteToEachRoster() {
        for(int i = 0; i < teamList.Count; i++) {
            teamList[i].AddAthleteToRoster();
        }
    }

    public void TeamSelectionPhase() {
        matchController.SetSide(true, teamList[0]);
        matchController.SetSide(false, teamList[1]);
    }

    public void ToggleComputerControl(Team team) {
        team.computerControlled = !team.computerControlled;
    }

    public void CycleTeamSelected(bool homeSide, bool cycleDown) {
        Team previousTeam;
        Team opponentTeam;
        if(homeSide) {
            previousTeam = matchController.GetTeam(true);
            opponentTeam = matchController.GetTeam(false);
        } else {
            previousTeam = matchController.GetTeam(false);
            opponentTeam = matchController.GetTeam(true);
        }


        int previousTeamNum = 0;
        int opponentTeamNum = 1;
        for(int i = 0; i < teamList.Count; i++) {
            if(teamList[i] == previousTeam) {
                previousTeamNum = i;
            } else if(teamList[i] == opponentTeam) {
                opponentTeamNum = i;
            }
        }

        int nextNum;
        if(cycleDown) {
            nextNum = previousTeamNum - 1;

            if(nextNum < 0) {
                nextNum = teamList.Count - 1;
            }

            if(nextNum == opponentTeamNum) {
                nextNum--;
            }
        } else {
            nextNum = previousTeamNum + 1;

            if(nextNum > teamList.Count - 1) {
                nextNum = 0;
            }

            if(nextNum == opponentTeamNum) {
                nextNum++;
            }
        }

        if(nextNum < 0) {
            nextNum = teamList.Count - 1;
        } else if(nextNum > teamList.Count - 1) {
            nextNum = 0;
        }

        matchController.SetSide(homeSide, teamList[nextNum]);
    }

    public void Rematch() {
        SceneManager.LoadScene(1);
    }

    public void ExitToMenu() {
        SceneManager.LoadScene(0);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void EndMatch(MatchData md) {
        if(PlayerPrefs.GetString("mode") == "competitive") {
            int goalDifferential = md.homeTeamData.GetScore() - md.awayTeamData.GetScore();
            if(goalDifferential > PlayerPrefs.GetInt("bestGoalDiffernce", 0)) {
                PlayerPrefs.SetInt("bestGoalDifference", goalDifferential);

                if(canvasManager != null) {
                    canvasManager.SetNewBestScore(md.homeTeamData.team, goalDifferential);
                }
            }
        }

        int totalTeamWins = PlayerPrefs.GetInt(md.GetWinner().name, 0);
        totalTeamWins++;
        PlayerPrefs.SetInt(md.GetWinner().name, totalTeamWins);
    }



    //Tournament Stuff
    public void SetupTournament() {
        SetNewRosters();

        //matchController.SetupCourt(currentRuleSet);

        int tournamentTeamCount = currentRuleSet.GetRule("teamCount").value;
        //Should be based on chosen team count

        List<Team> preShuffleTeamList = new List<Team>();
        for(int i = 0; i < tournamentTeamCount; i++) {
            preShuffleTeamList.Add(teamList[i % teamList.Count]);
            preShuffleTeamList[i].computerControlled = true;
        }

        for(int i = preShuffleTeamList.Count - 1; i > -1; i--) {
            Team selectedTeam = preShuffleTeamList[Random.Range(0, preShuffleTeamList.Count)];
            preShuffleTeamList.Remove(selectedTeam);
            tournamentTeamList.Add(selectedTeam);
        }
        

        for(int i = 0; i < tournamentTeamList.Count / 2; i++) {
            MatchData newMatch = new MatchData(tournamentTeamList[i * 2], tournamentTeamList[(i * 2) + 1], currentRuleSet);
            tournamentMatchDataList.Add(newMatch);
            tournamentActiveMatchDataList.Add(newMatch);
        }

        canvasManager.DisplayTournamentTeamSelect();
    }

    public void SetupTournamentBracket() {
        canvasManager.tournamentBracketPanel.SetTournamentBracket(tournamentMatchDataList);
        
        DisplayTournamentBracket();
    }

    public void DisplayTournamentBracket() {
        

        canvasManager.DisplayTournamentBracket();
    }

    public void AdvanceTournament() {
        if(tournamentWinner == null) {
            canvasManager.tournamentBracketPanel.AdvanceRound();

            StartCoroutine(AdvanceTournamentAnimation());   
        } else {
            Debug.Log("Tournament over we have a winner!");
            tournamentWinner.careerTeamMatchData.SetMatchDescriptors();

            canvasManager.DisplayTournamentWinner(tournamentWinner);
        }
    }

    public IEnumerator AdvanceTournamentAnimation() {
        WaitForFixedUpdate waiter = new WaitForFixedUpdate();

        float duration = 0.6f;
        float timer;

        for(int i = 0; i < tournamentActiveMatchDataList.Count; i++) {
            timer = 0f;

            if(tournamentActiveMatchDataList[i].GetWinner() == null) {
                while(timer < duration) {
                    timer += Time.deltaTime;

                    yield return waiter;
                }

                if(tournamentActiveMatchDataList[i].homeTeamData.team.computerControlled && tournamentActiveMatchDataList[i].awayTeamData.team.computerControlled) { //Simulate the match
                    tournamentActiveMatchDataList[i].SimulateMatch();

                    canvasManager.tournamentBracketPanel.UpdateTournamentBracket();
                } else { //Play the match
                    matchController.SetMatchup(tournamentActiveMatchDataList[i]);

                    canvasManager.DisplayPreMatch();
                    canvasManager.DisableTeamToggles();

                    break;
                }
            } // else the game is already completed and we can ignore it
        }

        if(AllMatchesComplete()) {
            Debug.Log("All matches complete"); 
            AdvanceToNextTournamentRound();
        }
    }

    public bool AllMatchesComplete() {
        for(int i = 0; i < tournamentActiveMatchDataList.Count; i++) {
            if(tournamentActiveMatchDataList[i].GetWinner() == null) {
                return false;
            }
        }

        return true;
    }

    public void AdvanceToNextTournamentRound() {
        List<Team> winners = new List<Team>();
        for(int i = 0; i < tournamentActiveMatchDataList.Count; i++) {
            winners.Add(tournamentActiveMatchDataList[i].GetWinner());
        }

        tournamentActiveMatchDataList = new List<MatchData>();

        if(winners.Count != 1) { 
            for(int i = 0; i < winners.Count / 2; i++) {
                MatchData newMatch = new MatchData(winners[i * 2], winners[(i * 2) + 1], currentRuleSet);
                tournamentMatchDataList.Add(newMatch);
                tournamentActiveMatchDataList.Add(newMatch);
            }
        } else {
            tournamentWinner = winners[0];

            particleManager.PlayFullScreenVictoryConfetti(new Vector3(0, -6f, 0), tournamentWinner);

            cameraController.AddTrauma(2f);
        }

        canvasManager.tournamentBracketPanel.SetNextRound(tournamentActiveMatchDataList);
    }





    //Gauntlet Stuff
    public void StartGauntlet() {
        gauntletRound = 0;
        gauntletTeam = teamList[0];

        canvasManager.SetGauntletRuleChangeButtons(gauntletTeam);

        SetupGauntletMatch();
    }

    public void SetupGauntletMatch() {
        matchController.SetupCourt(currentRuleSet);

        matchController.SetSide(true, gauntletTeam);

        Team gauntletOpponent = teamList[1 + gauntletRound % (teamList.Count - 1)];
        matchController.SetSide(false, gauntletOpponent);

        matchController.GetTeam(false).computerControlled = true;

        gauntletRound++;
    }

    public void EndGauntletMatch() {
        bool victory;
        //This assumes that the player is the home team and also idk if it'll even work
        Team playerTeam = matchController.GetTeam(true);
        if(matchController.GetMatchData().GetTeamMatchData(playerTeam).DidTeamWin()) {
            victory = true;
        } else {
            victory = false;
        }

		if(victory) {
			AdvanceInGauntlet();
		} else {
			GauntletOver();
		}
	}

	public void AdvanceInGauntlet() {
        matchController.ClearField();

        canvasManager.DisplayGauntletAdvancement();

        List<Rule> possibleRules = currentRuleSet.GetAvailableRuleChanges();
        List<Rule> chosenRules = new List<Rule>();
		for(int i = 0; i < 3; i++) {
            if(possibleRules.Count > 0) {
                Rule newRule = possibleRules[Random.Range(0, possibleRules.Count)];
                possibleRules.Remove(newRule);
                chosenRules.Add(newRule);

                Rule currentRule = currentRuleSet.GetRuleSlot(newRule.ruleID).GetCurrentRule();

                canvasManager.UpdateRuleChangeButton(currentRule, newRule, i);
            } else {
                Debug.Log("Not enough rules");
            }
        }
	}

    public RuleSet GetRuleSet() {
        return currentRuleSet;
    }

    public void SelectNewGauntletRule(Rule rule) {
        currentRuleSet.ChangeRule(rule);

        if(rule.ruleID == "athleteRosterCount") {
            AddAthleteToEachRoster();
        }

        StartCoroutine(WaitBeforeAdvancement());
    }

    public IEnumerator WaitBeforeAdvancement() {
        yield return new WaitForSeconds(0.5f);

        NextGauntletMatch();
    }

    public void NextGauntletMatch() {
        SetupGauntletMatch();

        canvasManager.DisplayPreMatch();
    }

    public int GetGauntletRound() {
        return gauntletRound;
    }

	public void GauntletOver() {
		Debug.Log("Gauntlet is over. You lose.");
        SceneManager.LoadScene(1);
	}
}
