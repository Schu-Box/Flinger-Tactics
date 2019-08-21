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

    private RuleSet currentRuleSet = new RuleSet();
    public List<Team> teamList = new List<Team>();

    public static Team gauntletTeam;

    private int gauntletRound = 0;

    void Start() {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraController.startPosition;
        canvasManager = FindObjectOfType<CanvasManager>();
        audioManager = FindObjectOfType<AudioManager>();
        matchController = FindObjectOfType<MatchController>();

        canvasManager.DisplayPreMatch();

        string modeID = PlayerPrefs.GetString("mode");
        if(modeID == "playNow") {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);

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
            Debug.Log("Oh we competing, huh?");

            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[3]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);

            SetNewRosters();

            matchController.SetupCourt(currentRuleSet);

            TeamSelectionPhase();

            canvasManager.DisableTeamSelection();
        } else {
            Debug.Log("Null mode selected");
        }
    }

    void Update() {
        if(Input.GetButtonDown("Submit")) {
            //Unused
        } else if(Input.GetKeyDown(KeyCode.E)) {
            matchController.EndMatch();
        } else if(Input.GetKeyDown(KeyCode.M)) {
            ToggleMute();
        } else if(Input.GetKeyDown(KeyCode.W)) {
            matchController.EndMatch();
            
            matchController.GetMatchData().homeTeamData.SetWin(true);
        } else if(Input.GetKeyDown(KeyCode.A) && !matchController.GetMatchStarted()) {
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteFieldCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("athleteRosterCount").possibleRules[2]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("ballCount").possibleRules[1]);
            currentRuleSet.ChangeRule(currentRuleSet.GetRuleSlot("turnCount").possibleRules[9]);

            matchController.GetTeam(true).computerControlled = true;
            matchController.GetTeam(false).computerControlled = true;

            matchController.StartMatch();
        }
    }

    public void ToggleMute() {
        if(!MatchController.muted) {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < audioSources.Length; i++) {
                audioSources[i].mute = true;
            }

            MatchController.muted = true;
        } else {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < audioSources.Length; i++) {
                audioSources[i].mute = false;
            }

            MatchController.muted = false;
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

    public void Exit() {
        SceneManager.LoadScene(0);
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
