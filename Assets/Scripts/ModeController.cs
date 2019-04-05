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

    void Start() {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraController.startPosition;
        canvasManager = FindObjectOfType<CanvasManager>();
        audioManager = FindObjectOfType<AudioManager>();
        matchController = FindObjectOfType<MatchController>();

        matchController.SetupCourt(currentRuleSet);

        canvasManager.DisplayPreMatch();

        string modeID = PlayerPrefs.GetString("mode");
        if(modeID == "playNow") {
            SetNewRosters();

            TeamSelectionPhase();

            matchController.GetTeam(false).computerControlled = true;
            matchController.GetTeam(true).computerControlled = true;
        } else if(modeID == "gauntlet") {
            Debug.Log("You're now in the gauntlet.");

            canvasManager.homeSelectionPanel.EnableInteraction(false);
            canvasManager.awaySelectionPanel.EnableInteraction(false);

            SetNewRosters();

            SetupGauntletMatch();
        } else {
            Debug.Log("Null mode selected");
        }
    }

    void Update() {
        if(Input.GetButtonDown("Submit")) {
            /*
            if(!homeReady) {
                ReadyTeam(true);
            }
            if(!awayReady) {
                ReadyTeam(false);
            }
            */
        } else if(Input.GetButtonDown("Cancel")) {
            SceneManager.LoadScene(0);
        } else if(Input.GetKeyDown(KeyCode.E)) {
            matchController.EndMatch();
        } else if(Input.GetKeyDown(KeyCode.M)) {
            ToggleMute();
        } else if(Input.GetKeyDown(KeyCode.W)) {
            matchController.EndMatch();
            matchController.GetTeam(true).wonTheGame = true;
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
            teamList[i].SetNewRoster(currentRuleSet.GetRule("athleteOnFieldCount").value);
        }
    }

    public void TeamSelectionPhase() {
        matchController.SetSide(true, teamList[0]);
        matchController.SetSide(false, teamList[1]);

        //matchController.DisableAllAthleteInteraction();
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

    public void SetupGauntletMatch() {
        matchController.SetSide(true, teamList[0]);
        matchController.SetSide(false, teamList[1]);

        matchController.GetTeam(false).computerControlled = true;
    }

    public void StartGauntletMatch() {
        matchController.StartMatch();
    }

    public void EndGauntletMatch() {
        bool victory;
        
        //This assumes that the player is the home team and also idk if it'll even work
        if(matchController.GetTeam(true).wonTheGame) {
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
		Rule ruleChoice1 = currentRuleSet.GetRandomRuleChange();
		Rule ruleChoice2 = currentRuleSet.GetRandomRuleChange();
		while(ruleChoice2 == ruleChoice1) {
			ruleChoice2 = currentRuleSet.GetRandomRuleChange();
		}
		Rule ruleChoice3 = currentRuleSet.GetRandomRuleChange();
		while(ruleChoice3 == ruleChoice1 || ruleChoice3 == ruleChoice2) {
			ruleChoice2 = currentRuleSet.GetRandomRuleChange();
		}

		Debug.Log(ruleChoice1.ruleID + " to " + ruleChoice1.value);
		Debug.Log(ruleChoice2.ruleID + " to " + ruleChoice2.value);
		Debug.Log(ruleChoice3.ruleID + " to " + ruleChoice3.value);

        canvasManager.DisplayGauntletAdvancement();
        canvasManager.UpdateRuleChangeButton(ruleChoice1, 0);
        canvasManager.UpdateRuleChangeButton(ruleChoice2, 1);
        canvasManager.UpdateRuleChangeButton(ruleChoice3, 2);
	}

    public void SelectNewGauntletRule(Rule rule) {
        currentRuleSet.ChangeRule(rule);

        NextGauntletMatch();
    }

    public void NextGauntletMatch() {
        matchController.SetupCourt(currentRuleSet);
        SetupGauntletMatch();

        canvasManager.DisplayPreMatch();
    }

	public void GauntletOver() {
		Debug.Log("Gauntlet is over. You lose.");	
	}
}
