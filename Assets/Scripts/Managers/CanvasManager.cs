using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CanvasManager : MonoBehaviour {

    public Canvas overlayCanvas;

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

    public TextMeshProUGUI turnIndicatorText;
    public Vector3 turnIndicatorStart;
    public Vector3 turnIndicatorMid;
    public Vector3 turnIndicatorEnd;
    //public GameObject screenFog;
    public GameObject canvasDimmer;

    private FootnoteManager footnoteManager;

    public RaisedButton timeoutButton;

    public Image tutorialPanel;
    public TextMeshProUGUI tutorialText;
    private Vector2 tutorialPanelFullSize;


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
    public Button exitButton;
    public GameObject statTable;

    [Header("Post Match on Field")]
    public VictoryPanel victoryPanel;
    public TextMeshProUGUI bestGoalDifferentialLabelText;
    public TextMeshProUGUI bestGoalDifferentialNumText;

    [Header("Team Post Match Panel")]
    public TeamPostMatchPanel teamPostMatchPanel;

    [Header("Gauntlet Panel")]
    public GameObject gauntletPanel;
    public TextMeshProUGUI gauntletTitleText;
    public Transform ruleChangeHolder;
    public Sprite ruleCircle;
    public Sprite ruleRing;

    [Header("Custom Rules")]
    public GameObject customRulesCustomButton;
    private Button customRulesButton;
    public GameObject customRulesPanel;
    public Button returnFromCustomRulesButton;
    public Transform customRulesHolder;

    [Header("Tournament UI")]
    public GameObject tournamentSetupPanel;
    public Transform tournamentRulesHolder;
    public TournamentTeamSelectPanel tournamentTeamSelectPanel;
    public TournamentBracketPanel tournamentBracketPanel;
    public TournamentWinnerPanel tournamentWinnerPanel;



    [Header("Field UI")]
	public TextMeshProUGUI homeFieldText;
    public TextMeshProUGUI awayFieldText;

    [Header("Escape Menu")]
    public GameObject escapePanel;
    public Slider musicVolumeSlider;
    public Slider soundEffectsVolumeSlider;

    private ModeController modeController;
	private MatchController matchController;
	private CameraController cameraController;
    private ParticleManager particleManager;

	private Team home;
	private Team away;

	void Awake() {
        modeController = FindObjectOfType<ModeController>();
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
        particleManager = FindObjectOfType<ParticleManager>();

		scoreFontSize = homeScoreText.fontSize;

        footnoteManager = FindObjectOfType<FootnoteManager>();

		postMatchPanel.SetActive(false);

        timeoutButton.gameObject.SetActive(false);
        //customRulesButon.gameObject.SetActive(false);
        customRulesPanel.SetActive(false);
        gauntletPanel.SetActive(false);

        turnIndicatorText.transform.localPosition = turnIndicatorStart;
        turnIndicatorText.gameObject.SetActive(false);

        victoryPanel.gameObject.SetActive(false);
        bestGoalDifferentialLabelText.gameObject.SetActive(false);
        bestGoalDifferentialNumText.gameObject.SetActive(false);

        customRulesButton = customRulesCustomButton.GetComponentInChildren<Button>();

        escapePanel.SetActive(false);

        tutorialPanelFullSize = tutorialPanel.rectTransform.sizeDelta;
        tutorialPanel.gameObject.SetActive(false);

        canvasDimmer.gameObject.SetActive(false);

        /*
        quoteBox.SetQuoteBox();
        quoteBox.gameObject.SetActive(false);
        */

        musicVolumeSlider.value = AudioManager.musicVolume;
        soundEffectsVolumeSlider.value = AudioManager.soundEffectsVolume;
	}

	public void DisplayPreMatch() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.startPosition));
        
        turnLabelText.text = "";
        turnCapText.text = "";
        homeScoreText.text = "";
        awayScoreText.text = "";

        turnButton.PreMatch();

        victoryPanel.gameObject.SetActive(false);
        footnoteManager.Hide();

        ToggleDimmer(true);
	}

	public void DisplayNewTeamSide(bool homeSide, Team team) {
		if(homeSide) {
			homeSelectionPanel.SetTeam(team);
			homeFieldText.text = team.fieldString;
            homeFieldText.color = team.secondaryColor;

			home = team;

            
            ColorBlock cb = customRulesButton.colors;
            cb.highlightedColor = team.primaryColor;
            cb.pressedColor = team.primaryColor;
            cb.selectedColor = team.GetDarkTint();
            customRulesButton.colors = cb;

            ColorBlock cb2 = returnFromCustomRulesButton.colors;
            cb2.highlightedColor = team.primaryColor;
            cb2.pressedColor = team.primaryColor;
            cb2.selectedColor = team.GetDarkTint();
            returnFromCustomRulesButton.colors = cb2;
		} else {
			awaySelectionPanel.SetTeam(team);
			awayFieldText.text = team.fieldString;
            awayFieldText.color = team.secondaryColor;

			away = team;
		}
	}

	public void DisplayStartMatch() {
		turnLabelText.text = "Turn";
        turnButton.DuringMatch();
        turnCapText.text = "of " + modeController.GetRuleSet().GetRule("turnCount").value;

        homeScoreText.text = home.GetCurrentMatchData().GetTeamMatchData(home).GetScore().ToString();
        awayScoreText.text = away.GetCurrentMatchData().GetTeamMatchData(away).GetScore().ToString();

        homeScoreText.color = home.primaryColor;
        awayScoreText.color = away.primaryColor;

        homeSelectionPanel.gameObject.SetActive(false);
        awaySelectionPanel.gameObject.SetActive(false);
        customRulesCustomButton.gameObject.SetActive(false);

        /*
        Debug.Log("Debugger victory particles");
        particleManager.PlayVictoryConfetti(Vector3.zero, home);
        */

        ToggleDimmer(false);
	}

	public IEnumerator DisplayScore(Team teamThatScored) {
		TextMeshProUGUI textAltered = homeScoreText;
        if(teamThatScored == home) {
            textAltered = homeScoreText;
        } else { //Assumes that it's scored by the awayTeam
            textAltered = awayScoreText;
        }

		textAltered.text = teamThatScored.GetCurrentMatchData().GetTeamMatchData(teamThatScored).GetScore().ToString();
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

        /*
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
        */
	}

    public IEnumerator AnimateTurnIndicator() {
        turnIndicatorText.gameObject.SetActive(true);

        turnIndicatorText.transform.localPosition = turnIndicatorStart;
        
        int turn = matchController.GetTurn();
        int turnCap = modeController.GetRuleSet().GetRule("turnCount").value;
        if(turn == turnCap) {
            turnIndicatorText.text = "Final Turn";
        } else if(turn > turnCap) {
            turnIndicatorText.text = "Overtime";
        } else {
            turnIndicatorText.text = "Turn " + matchController.GetTurn();
        }

        Vector3 startSize = turnIndicatorText.transform.localScale;

        float fontStart = turnIndicatorText.fontSize;
        float fontIncreaseSize = fontStart + 20f;

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float timer = 0f;
        float duration = 0.4f; //Moving onto screen
        while(timer < duration) {
            timer += Time.deltaTime;

            turnIndicatorText.transform.localPosition = Vector3.Lerp(turnIndicatorStart, turnIndicatorMid, timer/duration);

            yield return waiter;
        }

        timer = 0f;
        duration = 0.6f; //Increasing in size
        while(timer < duration) {
            timer += Time.deltaTime;

            turnIndicatorText.fontSize = Mathf.Lerp(fontStart, fontIncreaseSize, timer/duration);

            yield return waiter;
        }

        timer = 0f;
        duration = 0.4f; //Decreasing in size
        while(timer < duration) {
            timer += Time.deltaTime;

            turnIndicatorText.fontSize = Mathf.Lerp(fontIncreaseSize, fontStart, timer/duration);

            yield return waiter;
        }

        timer = 0f;
        duration = 0.3f; //Moving off screen
        while(timer < duration) {
            timer += Time.deltaTime;

            turnIndicatorText.transform.localPosition = Vector3.Lerp(turnIndicatorMid, turnIndicatorEnd, timer/duration);
            turnIndicatorText.transform.localScale = Vector3.Lerp(startSize, Vector3.zero, timer/duration);

            yield return waiter;
        }

        turnIndicatorText.transform.localPosition = turnIndicatorEnd;
        turnIndicatorText.transform.localScale = startSize;
        turnIndicatorText.gameObject.SetActive(false);
    }

    public void DisplayTurnActive(Team initiatingTeam) {
        turnButton.TurnActive(initiatingTeam);

        //timeoutButton.gameObject.SetActive(false);
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
        turnIndicatorText.text = "";
		turnLabelText.text = "";
        turnCapText.text = "";

        turnButton.PostMatch();

        Team victor = matchController.GetMatchData().GetWinner();

        victoryPanel.SetVictoryTeam(victor);

        if(PlayerPrefs.GetString("mode") == "competitive") {
            bestGoalDifferentialLabelText.gameObject.SetActive(true);
            bestGoalDifferentialNumText.gameObject.SetActive(true);
        }

        ToggleDimmer(true);
	}

	public void DisplayPostMatchPanel() {
        postMatchPanel.SetActive(true);
        homeFinalScore.text = home.GetCurrentMatchData().GetTeamMatchData(home).GetScore().ToString();
        awayFinalScore.text = away.GetCurrentMatchData().GetTeamMatchData(away).GetScore().ToString();
        homeFinalScore.color = home.primaryColor;
        awayFinalScore.color = away.primaryColor;

        homeNamePlate.GetComponent<Image>().color = home.primaryColor;
        awayNamePlate.GetComponent<Image>().color = away.primaryColor;

        TextMeshProUGUI homeNameText = homeNamePlate.GetComponentInChildren<TextMeshProUGUI>();
        homeNameText.text = home.nameLocation + '\n' + home.nameNickname;
        homeNameText.color = home.primaryColor;

        TextMeshProUGUI awayNameText = awayNamePlate.GetComponentInChildren<TextMeshProUGUI>();
        awayNameText.text = away.nameLocation + '\n' + away.nameNickname;
        awayNameText.color = away.primaryColor;

        arrowButton_home.GetComponent<Image>().color = home.primaryColor;
        arrowButton_away.GetComponent<Image>().color = away.primaryColor;
        arrowButton_home.onClick.AddListener(() => DisplayTeamPanelPostMatch(true));
        arrowButton_away.onClick.AddListener(() => DisplayTeamPanelPostMatch(false));

        continueButton.onClick.RemoveAllListeners();
        string modeID = PlayerPrefs.GetString("mode");
        if(modeID == "playNow" || modeID == "competitive") {
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "REPLAY";
            continueButton.onClick.AddListener(() => modeController.Rematch());
        } else { //assume it's the tournament mode
            continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "CONTINUE";
            continueButton.onClick.AddListener(() => modeController.DisplayTournamentBracket());
            /*
            Team playerTeam = matchController.GetTeam(true);
            if(playerTeam.GetCurrentMatchData().GetTeamMatchData(playerTeam).DidTeamWin()) {
                continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "CONTINUE";
            } else {
                continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "RESTART";
            }
            
            continueButton.onClick.AddListener(() => modeController.EndGauntletMatch());
            */
        }

        Color winnerColor;
        if(home.GetCurrentMatchData().GetWinner() == home) {
            winnerColor = home.primaryColor;
        } else {
            winnerColor = away.primaryColor;
        }

        ColorBlock ccb = continueButton.colors;
        ccb.highlightedColor = winnerColor;
        ccb.pressedColor = winnerColor;
        continueButton.colors = ccb;

        ColorBlock ecb = exitButton.colors;
        ecb.highlightedColor = winnerColor;
        ecb.pressedColor = winnerColor;
        exitButton.colors = ecb;

        SetShowcaseAthletes();
        SetStatTable();

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));

        particleManager.StopVictoryParticles();
    }

    public void SetShowcaseAthletes() {
        Athlete mvp = matchController.GetMatchData().GetMVP();

        Athlete homeMVP = matchController.GetMatchData().GetHomeMVP();
        showcaseAthlete_home.SetAthlete(homeMVP, home.GetCurrentMatchData().GetTeamMatchData(home).GetAthleteMatchData(homeMVP));
        if(homeMVP == mvp) {
            showcaseAthlete_away.haloText.gameObject.SetActive(false);
            showcaseAthlete_home.haloText.gameObject.SetActive(true);
            showcaseAthlete_home.GetComponent<AthleteImage>().SetMVP();
        }

        Athlete awayMVP = matchController.GetMatchData().GetAwayMVP();
        showcaseAthlete_away.SetAthlete(awayMVP, away.GetCurrentMatchData().GetTeamMatchData(away).GetAthleteMatchData(awayMVP));
        if(awayMVP == mvp) {
            showcaseAthlete_home.haloText.gameObject.SetActive(false);
            showcaseAthlete_away.haloText.gameObject.SetActive(true);
            showcaseAthlete_away.GetComponent<AthleteImage>().SetMVP();
        }
    }

    public void SetStatTable() {
        MatchData matchData = matchController.GetMatchData();
        List<Stat> homeStats = matchData.homeTeamData.GetTeamTotalStatList();
        List<Stat> awayStats = matchData.awayTeamData.GetTeamTotalStatList();

        for(int i = 0; i < statTable.transform.childCount; i++) {
            Stat homeDisplayStat = homeStats[i + 1]; //+1 to ignore goals, since those are in the score (ignoring own goals)
            Stat awayDisplayStat = awayStats[i + 1];

            Transform statBox = statTable.transform.GetChild(i);
            statBox.GetChild(0).GetComponent<Image>().color = home.primaryColor;
            statBox.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = homeDisplayStat.GetCount().ToString();
            statBox.GetChild(1).GetComponent<TextMeshProUGUI>().text = homeDisplayStat.GetStatName();
            statBox.GetChild(2).GetComponent<Image>().color = away.primaryColor;
            statBox.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = awayDisplayStat.GetCount().ToString();
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
        gauntletPanel.SetActive(true);

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downPosition));

        Team team = matchController.GetTeam(true);

        gauntletTitleText.text = "Gauntlet Round " + modeController.GetGauntletRound();
        gauntletTitleText.transform.parent.GetComponent<Image>().color = team.primaryColor;
        gauntletTitleText.transform.parent.GetChild(1).GetComponent<Image>().color = team.secondaryColor;
    }

    public void SetGauntletRuleChangeButtons(Team gauntletTeam) {
        for(int i = 0; i < ruleChangeHolder.childCount; i++) {
            RuleChangeButton ruleChangeButton = ruleChangeHolder.GetChild(i).GetComponent<RuleChangeButton>();
            ruleChangeButton.SetTeam(gauntletTeam);
        }
    }

    public void UpdateRuleChangeButton(Rule currentRule, Rule newRule, int num) {
        RuleChangeButton ruleChangeButton = ruleChangeHolder.GetChild(num).GetComponent<RuleChangeButton>();
        ruleChangeButton.SetRuleCircleSprites(ruleCircle, ruleRing);
        ruleChangeButton.SetRuleChangeButton(currentRule, newRule);
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

    public void DisplayCustomRuleChangers() {
        customRulesPanel.SetActive(true);
        tournamentBracketPanel.gameObject.SetActive(false);

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downPosition));

        for(int i = 0; i < customRulesHolder.childCount; i++) {
            CustomRuleChanger ruleChanger = customRulesHolder.GetChild(i).GetComponent<CustomRuleChanger>();
            ruleChanger.SetCustomRuleChanger(modeController.GetRuleSet(), modeController.GetRuleSet().ruleSlotList[i]);
        }
    }

    public void UpdateCustomRuleChangers() {
        for(int i = 0; i < customRulesHolder.childCount; i++) {
            CustomRuleChanger ruleChanger;
            if(PlayerPrefs.GetString("mode") == "tournament") {
                ruleChanger = tournamentRulesHolder.GetChild(i).GetComponent<CustomRuleChanger>();
            } else {
                ruleChanger = customRulesHolder.GetChild(i).GetComponent<CustomRuleChanger>();
            }
            Rule rule = ruleChanger.GetRuleSlot().GetCurrentRule();
            if(rule.ruleID == "athleteRosterCount") {
                if(rule.value <= modeController.GetRuleSet().GetRule("athleteFieldCount").value) {
                    ruleChanger.DisableArrow(false);
                } else {
                    if(modeController.GetRuleSet().GetRuleSlot("athleteFieldCount").GetPreviousRule() != null) {
                        ruleChanger.EnableArrow(false);
                    }
                }
            } else if(rule.ruleID == "athleteFieldCount") {
                if(rule.value >= modeController.GetRuleSet().GetRule("athleteRosterCount").value) {
                    ruleChanger.DisableArrow(true);
                } else {
                    if(modeController.GetRuleSet().GetRuleSlot("athleteRosterCount").GetPreviousRule() != null) {
                        ruleChanger.EnableArrow(true);
                    }
                }
            }
        }
    }

    public void UndisplayCustomRuleChangers() {
        matchController.SetupCourt(modeController.GetRuleSet());
        modeController.SetNewRosters();
        matchController.ResetSides();

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.startPosition));
    }

    public void DisplayTournamentSetup() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downLeftLeftPosition));
        //Camera.main.transform.position = cameraController.downLeftLeftPosition;

        for(int i = 0; i < tournamentRulesHolder.childCount; i++) {
            CustomRuleChanger ruleChanger = tournamentRulesHolder.GetChild(i).GetComponent<CustomRuleChanger>();
            ruleChanger.SetCustomRuleChanger(modeController.GetRuleSet(), modeController.GetRuleSet().ruleSlotList[i]);
        }
    }

    public void DisplayTournamentTeamSelect() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downLeftPosition));

        Team startTeam = modeController.teamList[0];
        tournamentTeamSelectPanel.SetForTeam(startTeam);
    }

    public void DisplayTournamentBracket() {
        tournamentBracketPanel.gameObject.SetActive(true);

        tournamentBracketPanel.UpdateTournamentBracket();

        tournamentBracketPanel.advanceButton.EnableButton();

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downPosition));
    }

    public void DisplayTournamentWinner(Team winningTeam) {
        tournamentWinnerPanel.SetPanel(winningTeam);

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.downDownPosition));
    }

    public void DisableTeamToggles() {
        awaySelectionPanel.leftArrow.gameObject.SetActive(false);
        awaySelectionPanel.rightArrow.gameObject.SetActive(false);
        awaySelectionPanel.computerToggleButton.gameObject.SetActive(false);

        homeSelectionPanel.leftArrow.gameObject.SetActive(false);
        homeSelectionPanel.rightArrow.gameObject.SetActive(false);
        homeSelectionPanel.computerToggleButton.gameObject.SetActive(false);

        customRulesCustomButton.gameObject.SetActive(false);
    }

    public void SetNewBestScore(Team bestTeam, int bestScore) {
        bestGoalDifferentialNumText.text = bestScore.ToString();
        bestGoalDifferentialNumText.color = bestTeam.primaryColor;
    }

    public void SetCompetitiveMode() {
        awaySelectionPanel.leftArrow.gameObject.SetActive(false);
        awaySelectionPanel.rightArrow.gameObject.SetActive(false);
        awaySelectionPanel.computerToggleButton.gameObject.SetActive(false);

        homeSelectionPanel.computerToggleButton.gameObject.SetActive(false);

        customRulesCustomButton.gameObject.SetActive(false);
    }

    public void ToggleEscapeMenu() {
        if(escapePanel.activeSelf) {
            escapePanel.SetActive(false);
            Time.timeScale = 1f;
        } else {
            escapePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void DisplayTutorialText(AthleteController ac) {
        if(!openingTutorialText) {
            tutorialPanel.gameObject.SetActive(true);

            Vector3 offset;
            if(ac.transform.localPosition.y > -2f) {
                offset = new Vector3(0, -1.5f, 0);
            } else {
                offset = new Vector3(0, 1.5f, 0);
            }

            tutorialPanel.transform.position = ac.transform.position + offset;

            tutorialPanel.color = ac.GetAthlete().GetTeam().GetDarkTint();

            tutorialText.text = ac.GetAthlete().athleteData.abilityTutorialString;

            StartCoroutine(AnimateDisplayTutorialText());
        }
    }

    private bool openingTutorialText = false;
    public IEnumerator AnimateDisplayTutorialText() {
        openingTutorialText = true;

        tutorialText.gameObject.SetActive(false);

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float timer = 0f;
        float duration = 0.1f;

        while(timer <= duration) {
            timer += Time.deltaTime;

            tutorialPanel.rectTransform.sizeDelta = Vector2.Lerp(Vector2.zero, tutorialPanelFullSize, timer/duration);

            yield return waiter;
        }

        tutorialPanel.rectTransform.sizeDelta = tutorialPanelFullSize;
        tutorialText.gameObject.SetActive(true);
        openingTutorialText = false;
    }

    public void UndisplayTutorialText() {
        if(!closingTutorialText) {
            StartCoroutine(AnimateUndisplayTutorialText());
        }
    }

    private bool closingTutorialText = false;
    public IEnumerator AnimateUndisplayTutorialText() {
        closingTutorialText = true;

        tutorialText.gameObject.SetActive(false);

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float timer = 0f;
        float duration = 0.08f;

        while(timer <= duration) {
            timer += Time.deltaTime;

            tutorialPanel.rectTransform.sizeDelta = Vector2.Lerp(tutorialPanelFullSize, Vector2.zero, timer/duration);

            yield return waiter;
        }
        
        tutorialPanel.rectTransform.sizeDelta = tutorialPanelFullSize;
        tutorialPanel.gameObject.SetActive(false);
        closingTutorialText = false;
    }

    public void ToggleDimmer(bool enable) {
        canvasDimmer.SetActive(true);

        Image dimmerImg = canvasDimmer.GetComponent<Image>();
        Color dimmerColor = dimmerImg.color;

        if(enable) {
           dimmerImg.color = Color.clear;
           StartCoroutine(ChangeToColor(dimmerImg, dimmerColor));
        } else {
            StartCoroutine(ChangeToColor(dimmerImg, Color.clear));
        }
    }

    public IEnumerator ChangeToColor(Image img, Color endColor) {
        Color startColor = img.color;

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float duration = 0.4f;
        float timer = 0f;
        while(timer < duration) {
            timer += Time.deltaTime;

            img.color = Color.Lerp(startColor, endColor, timer/duration);

            yield return waiter;
        }

        img.color = endColor;

        if(endColor == Color.clear) {
            img.color = startColor;
            img.gameObject.SetActive(false);
        }
    }
}
