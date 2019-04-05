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
    public TextMeshProUGUI turnLabelText;
    public TurnButton turnButton;
    public TextMeshProUGUI turnCapText;
    public TextMeshProUGUI homeScoreText;
    public TextMeshProUGUI awayScoreText;
    private float scoreFontSize;

    private FootnoteManager footnoteManager;

    public RaisedButton timeoutButton;


    [Header("Post Match UI")]
    public GameObject postMatchPanel;
    public TextMeshProUGUI homeFinalScore;
    public TextMeshProUGUI awayFinalScore;
    public GameObject homeNamePlate;
    public GameObject awayNamePlate;
    public ShowcaseAthleteUI showcaseAthlete_home;
    public ShowcaseAthleteUI showcaseAthlete_away;
    public Button arrowButton_home;
    public Button arrowButton_away;
    public Button continueButton;
    public GameObject statTable;

    [Header("Team Post Match Panel")]
    public TeamPostMatchPanel teamPostMatchPanel;

    [Header("Gauntlet Panel")]
    public GameObject gauntletPanel;
    public TextMeshProUGUI gauntletTitleText;
    public Transform ruleChangeHolder;
    


    [Header("Field UI")]
	public TextMeshProUGUI homeFieldText;
    public TextMeshProUGUI awayFieldText;

    private ModeController modeController;
	private MatchController matchController;
	private CameraController cameraController;

	private Team home;
	private Team away;

	void Awake() {
        modeController = FindObjectOfType<ModeController>();
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();

		scoreFontSize = homeScoreText.fontSize;

        footnoteManager = FindObjectOfType<FootnoteManager>();

		postMatchPanel.SetActive(false);

        timeoutButton.gameObject.SetActive(false);
	}

	public void DisplayPreMatch() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.startPosition));
        
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
        float endSize = scoreFontSize * 1.2f;

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

        footnoteManager.Hide();

        TeamMatchData teamData;
        if(matchController.GetTurnTeam() == matchController.GetTeam(true)) {
            teamData = matchController.GetMatchData().homeTeamData;
        } else {
            teamData = matchController.GetMatchData().awayTeamData;
        }

        if(teamData.HasTimeout()) {
            timeoutButton.gameObject.SetActive(true);
            timeoutButton.SetForTeam(matchController.GetTurnTeam());

            timeoutButton.RaiseButton();

            if(matchController.IsTimeoutAcceptable()) {
                timeoutButton.SetText("Accept Timeout");
                timeoutButton.StartFlash();
            } else {
                timeoutButton.SetText("Request Timeout");
            }
        }
	}

    public void TimeoutButtonClicked() {
        if(matchController.IsTimeoutAcceptable()) {
            matchController.UseTimeout();
        } else {
            matchController.RequestTimeout();
        }
    }

    public void DisplayTurnActive(Team initiatingTeam) {
        turnButton.TurnActive(initiatingTeam);

        timeoutButton.gameObject.SetActive(false);
    }

    public void DisplayFootnotePanel(Athlete athlete) {
        if(!matchController.IsTurnActive()) {
            footnoteManager.Display(athlete);
        }
    }

    public void UpdateFootnotePanel(StatType type, Athlete athlete) {
        footnoteManager.UpdateIncreasedStat(type, athlete);
    }

    public void HideFootnotePanel() {
        if(!matchController.IsTurnActive()) {
            footnoteManager.Hide();
        }
    }

	public void DisplayEndMatch() {
		turnLabelText.text = "";
        turnCapText.text = "";

        turnButton.PostMatch();
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

        arrowButton_home.GetComponent<Image>().color = home.primaryColor;
        arrowButton_away.GetComponent<Image>().color = away.primaryColor;
        arrowButton_home.onClick.AddListener(() => DisplayTeamPanelPostMatch(true));
        arrowButton_away.onClick.AddListener(() => DisplayTeamPanelPostMatch(false));

        continueButton.onClick.RemoveAllListeners();
        if(PlayerPrefs.GetString("mode") == "playNow") {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Replay";
            continueButton.onClick.AddListener(() => modeController.Rematch());
        } else {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
            continueButton.onClick.AddListener(() => modeController.EndGauntletMatch());
        }

        SetShowcaseAthletes();
        SetStatTable();

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));
    }

    public void SetShowcaseAthletes() {

        Athlete mvp = matchController.GetMatchData().GetMVP();

        Athlete homeMVP = matchController.GetMatchData().GetHomeMVP();
        showcaseAthlete_home.SetAthlete(homeMVP);
        if(homeMVP == mvp) {
            showcaseAthlete_home.GetComponent<AthleteImage>().SetMVP();
        }

        Athlete awayMVP = matchController.GetMatchData().GetAwayMVP();
        showcaseAthlete_away.SetAthlete(awayMVP);
        if(awayMVP == mvp) {
            showcaseAthlete_away.GetComponent<AthleteImage>().SetMVP();
        }
    }

    public void SetStatTable() {
        MatchData matchData = matchController.GetMatchData();
        List<Stat> homeStats = matchData.homeTeamData.GetTeamTotalStatList();
        List<Stat> awayStats = matchData.awayTeamData.GetTeamTotalStatList();
        for(int i = 0; i < homeStats.Count; i++) {
            Transform statBox = statTable.transform.GetChild(i);
            statBox.GetChild(0).GetComponent<Image>().color = home.primaryColor;
            statBox.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = homeStats[i].GetCount().ToString();
            statBox.GetChild(1).GetComponent<TextMeshProUGUI>().text = homeStats[i].GetStatName();
            statBox.GetChild(2).GetComponent<Image>().color = away.primaryColor;
            statBox.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = awayStats[i].GetCount().ToString();

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

    public void DisplayGauntletAdvancement() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downPosition));
    }

    public void UpdateRuleChangeButton(Rule newRule, int num) {
        GameObject ruleButton = ruleChangeHolder.GetChild(num).gameObject;
        ruleButton.GetComponentInChildren<TextMeshProUGUI>().text = newRule.description;
        ruleButton.GetComponent<Button>().onClick.RemoveAllListeners();
        ruleButton.GetComponent<Button>().onClick.AddListener(() => modeController.SelectNewGauntletRule(newRule));
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
