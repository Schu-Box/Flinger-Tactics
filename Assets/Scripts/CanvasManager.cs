using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour {

	[Header("Team Selection UI")]
    public TeamSelectionPanel homeSelectionPanel;
    public TeamSelectionPanel awaySelectionPanel;

    [Header("UI")]
    public ReadyUpButton homeReadyButton;
    public ReadyUpButton awayReadyButton;
    public TextMeshProUGUI turnLabelText;
    public TurnButton turnButton;
    public TextMeshProUGUI turnCapText;
    public TextMeshProUGUI homeScoreText;
    public TextMeshProUGUI awayScoreText;
    private float scoreFontSize;

    private FootnoteManager footnoteManager;


    [Header("Post Match UI")]
    public GameObject postMatchPanel;
    public TextMeshProUGUI homeFinalScore;
    public TextMeshProUGUI awayFinalScore;
    public GameObject homeNamePlate;
    public GameObject awayNamePlate;
    public List<ShowcaseAthleteUI> showcaseHomeAthleteUIList = new List<ShowcaseAthleteUI>();
    public List<ShowcaseAthleteUI> showcaseAwayAthleteUIList = new List<ShowcaseAthleteUI>();

    public TeamPostMatchPanel teamPostMatchPanel;

	public TextMeshProUGUI homeFieldText;
    public TextMeshProUGUI awayFieldText;

	private MatchController matchController;
	private CameraController cameraController;

	private Team home;
	private Team away;

	void Start() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();

		scoreFontSize = homeScoreText.fontSize;

        footnoteManager = FindObjectOfType<FootnoteManager>();

		postMatchPanel.SetActive(false);
	}

	public void DisplayTeamSelection() {
		homeReadyButton.gameObject.SetActive(false);
        awayReadyButton.gameObject.SetActive(false);

        turnLabelText.text = "";
        turnCapText.text = "";
        homeScoreText.text = "";
        awayScoreText.text = "";

        turnButton.PreMatch();
	}

	public void DisplayNewTeamSide(bool homeSide, Team team) {
		if(homeSide) {
			homeSelectionPanel.SetTeam(team);
			homeFieldText.text = team.fieldString;

			home = team;
		} else {
			awaySelectionPanel.SetTeam(team);
			awayFieldText.text = team.fieldString;

			away = team;
		}
	}

	public void DisplayStartMatch() {
		turnLabelText.text = "Turn";
        turnButton.DuringMatch();
        turnCapText.text = "of " + matchController.turnCap.ToString();

        homeScoreText.text = home.score.ToString();
        awayScoreText.text = away.score.ToString();

        homeScoreText.color = home.primaryColor;
        awayScoreText.color = away.primaryColor;

        homeSelectionPanel.gameObject.SetActive(false);
        awaySelectionPanel.gameObject.SetActive(false);

		homeReadyButton.SetForTeam(home);
        awayReadyButton.SetForTeam(away);

		if(!MatchController.simultaneousTurns) {
			homeReadyButton.gameObject.SetActive(false);
			awayReadyButton.gameObject.SetActive(false);
		}
	}

	public IEnumerator DisplayScore(Team teamThatScored) {
		TextMeshProUGUI textAltered = homeScoreText;
        if(teamThatScored == home) {
            textAltered = homeScoreText;
        } else { //Assumes that it's scored by the awayTeam
            textAltered = awayScoreText;
        }

		textAltered.text = teamThatScored.score.ToString();
        textAltered.color = Color.white;
        float endSize = scoreFontSize * 1.6f;

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float duration = 20f;
        float timer = 0f;
        while(timer < duration) {
            timer++;

            textAltered.fontSize = Mathf.Lerp(scoreFontSize, endSize, timer/duration);
            
            yield return waiter;
        }

        timer = 0f;
        while(timer < duration) { //Returns the font to it's original size
            timer++;

            textAltered.fontSize = Mathf.Lerp(endSize, scoreFontSize, timer/duration);
            
            yield return waiter;
        }

        textAltered.fontSize = scoreFontSize;
        textAltered.color = teamThatScored.primaryColor;
	}

	public void DisplayNextTurn(int turnNum) {
		turnButton.NextTurn(turnNum);

		if(MatchController.simultaneousTurns) {
			homeReadyButton.MyTurnNow(matchController.IsHomeTurn());
			awayReadyButton.MyTurnNow(!matchController.IsHomeTurn());
		}

        footnoteManager.HideBoth();
	}

    public void DisplayTurnActive(Team initiatingTeam) {
        turnButton.TurnActive(initiatingTeam);
    }

    public void DisplayFootnotePanel(Athlete athlete) {
        if(!matchController.IsTurnActive()) {
            footnoteManager.Display(athlete);
        }
    }

    public void UpdateFootnotePanel(StatType type, Athlete athlete) {
        footnoteManager.UpdateIncreasedStat(type, athlete);
    }

    public void HideFootnotePanel(Athlete athlete) {
        if(!matchController.IsTurnActive()) {
            footnoteManager.Hide(athlete);
        }
    }

	public void DisplayTeamReadyUp(bool home) {
		if(home) {
			awayReadyButton.MyTurnNow(true);
			homeReadyButton.MyTurnNow(false);
		} else {
			awayReadyButton.MyTurnNow(false);
            homeReadyButton.MyTurnNow(true);
		}
	}

	public void DisplayEndMatch() {
		turnLabelText.text = "";
        turnCapText.text = "";

        turnButton.PostMatch();

        homeReadyButton.EndMatch(home.score - away.score);
        awayReadyButton.EndMatch(away.score - home.score);
	}

	public void DisplayPostMatchPanel() {
        postMatchPanel.SetActive(true);
        homeFinalScore.text = home.score.ToString();
        awayFinalScore.text = away.score.ToString();
        homeFinalScore.color = home.primaryColor;
        awayFinalScore.color = away.primaryColor;

        homeNamePlate.GetComponent<Image>().color = home.primaryColor;
        awayNamePlate.GetComponent<Image>().color = away.primaryColor;
        homeNamePlate.GetComponentInChildren<TextMeshProUGUI>().text = home.name;
        awayNamePlate.GetComponentInChildren<TextMeshProUGUI>().text = away.name;

        SetShowcaseAthletes();

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));
    }

    public void SetShowcaseAthletes() {
        /*
        MatchData matchData = matchController.GetTeam(true).GetCurrentMatchData();
        Athlete mostGoals = matchData.GetBestPerformerForStat(StatType.Goals);
        */

        Athlete mvp = matchController.GetMatchData().GetMVP();
        
        List<Athlete> showcaseAthletes = matchController.GetAthletesOnField(true);
        for(int i = 0; i < showcaseHomeAthleteUIList.Count; i++) {
            Athlete selectedAthlete = showcaseAthletes[i];
            
            showcaseHomeAthleteUIList[i].SetAthlete(selectedAthlete);
            
            if(selectedAthlete == mvp) {
                showcaseHomeAthleteUIList[i].GetComponent<AthleteImage>().SetMVP();
            }
        }

        showcaseAthletes = matchController.GetAthletesOnField(false);
        for(int i = 0; i < showcaseAwayAthleteUIList.Count; i++) {
            Athlete selectedAthlete = showcaseAthletes[i];
            
            showcaseAwayAthleteUIList[i].SetAthlete(selectedAthlete);

            if(selectedAthlete == mvp) {
                showcaseAwayAthleteUIList[i].GetComponent<AthleteImage>().SetMVP();
            }
        }
    }

	public void DisplayTeamPanelPostMatch(bool home) {
        if(home) {
            teamPostMatchPanel.SetTeamPostMatchPanel(true, matchController.GetTeam(true));

            StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upLeftPosition));
        } else {
            teamPostMatchPanel.SetTeamPostMatchPanel(false, matchController.GetTeam(false));

            StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upRightPosition));
        }
    }

	public void GoBackToPostMatchPanel() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));
    }

    public void LeaveMatch() {
        Debug.Log("Leaving Match");
        
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downPosition));
    }

	public IEnumerator MoveObjectToPosition(GameObject obj, Vector3 endPos) {
        Vector3 startPos = obj.transform.position;

        float timer = 0f;
        float duration = 0.3f;
        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        while(timer < duration) {
            timer += Time.deltaTime;

            obj.transform.position = Vector3.Lerp(startPos, endPos, timer/duration);

            yield return waiter;
        }

        obj.transform.position = endPos;
    }
}
