using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MatchController : MonoBehaviour {

	public GameObject ballPrefab;
    public GameObject athleteStandardPrefab;
    public GameObject bumperPrefab;
    public GameObject shockwavePrefab;

	private Team homeTeam = null;
    private Team awayTeam = null;
    public SpriteRenderer homeSideColor;
    public SpriteRenderer awaySideColor;

    public GoalController homeGoal;
    public GoalController awayGoal;
    public Transform homeBumperHolder;
    public List<Bumper> homeBumpers;
    public Transform awayBumperHolder;
    public List<Bumper> awayBumpers;
    public DefenseZone homeDefenseZone;
    public DefenseZone awayDefenseZone;

	public Transform athleteHolder;
	public Transform ballHolder;
    public Transform substituteHolder;

    public SpawnBox ballSpawnBox;
    public SpawnBox homeSpawnBox;
    public SpawnBox awaySpawnBox;

    public GameObject ballSpawnExplosionPrefab;

    public Transform upperLimit;
    public Transform lowerLimit;

	private List<AthleteController> homeAthletesOnField = new List<AthleteController>();
    private List<AthleteController> awayAthletesOnField = new List<AthleteController>();
    private List<AthleteController> allAthletesOnField = new List<AthleteController>();
    private List<BallController> ballsOnField = new List<BallController>();
    private List<AthleteController> outboundSubstitutes = new List<AthleteController>();
    private List<AthleteController> inboundSubstitutes = new List<AthleteController>();
    private List<AthleteController> inactiveHomeAthletes = new List<AthleteController>();
    private List<AthleteController> inactiveAwayAthletes = new List<AthleteController>();

    private List<Bumper> bumperTriggersLastEntered = new List<Bumper>();

    public GameObject substituteChair;

    private SubstituteChair homeSubChair;
    private SubstituteChair awaySubChair;
    
    //Ready stuff is now unused
    private bool homeReady = false;
    private bool awayReady = false;

    private bool homeSubAvailable = false;
    private bool awaySubAvailable = false;

	private bool unbounded = false;

    private bool matchStarted = false;
    private bool matchEnded = false;
    private bool overtime = false;
    private int turnCap;
    private int turnNumber = 1;
    private bool homeStarts = false;
    private bool homeTurn = false;
    private bool substituteInbound = false;
    private bool turnActive = false;
    private bool ballRespawning = false;

    private bool displayingTutorial = false;
    private float tutorialTimerCap = 1.5f;
    private float tutorialTimer = 0f;

    private AthleteController athleteBeingHovered;
    private AthleteController athleteBeingDragged;
    private AthleteController athleteInitiater;

	public static bool simultaneousTurns = true;

    public static bool muted = false;

    private TitleMenuController titleMenuController;
    private ModeController modeController;
	private CanvasManager canvasManager;
    private CameraController cameraController;
	private AudioManager audioManager;
    private CrowdController crowdController;
    private ParticleManager particleManager;
    private CursorController cursorController;
    private QuoteManager quoteManager;

    private MatchData matchData;
    private RuleSet ruleSet; //could get rid of this and only call from MatchData, but it's whatevs

	void Awake() {
        Physics2D.IgnoreLayerCollision(14, 10, true); //Makes the balls ignore the goals
        Physics2D.IgnoreLayerCollision(14, 11, true);
        Physics2D.IgnoreLayerCollision(15, 10, true);
        Physics2D.IgnoreLayerCollision(15, 11, true);
        Physics2D.IgnoreLayerCollision(9, 10, true); //Makes the crowd ignore the goals
        Physics2D.IgnoreLayerCollision(9, 11, true);
        Physics2D.IgnoreLayerCollision(12, 11, true); //Makes the home team athletes ignore away goals
        Physics2D.IgnoreLayerCollision(13, 10, true); //Makes the away team athletes ignore home goals

        Physics2D.IgnoreLayerCollision(8, 9, true); //Makes the substitutes ignore goals and bumpers and the crowd
        Physics2D.IgnoreLayerCollision(8, 10, true);
        Physics2D.IgnoreLayerCollision(8, 11, true);
        Physics2D.IgnoreLayerCollision(8, 12, true);
        Physics2D.IgnoreLayerCollision(8, 13, true);
        Physics2D.IgnoreLayerCollision(8, 16, true);
        Physics2D.IgnoreLayerCollision(8, 18, true); //Makes substitutes ignore substitute chairs
        Physics2D.IgnoreLayerCollision(18, 9, true); //Makes the substitute chair ignore goals and bumpers and the crowd
        Physics2D.IgnoreLayerCollision(18, 10, true);
        Physics2D.IgnoreLayerCollision(18, 11, true);
        Physics2D.IgnoreLayerCollision(18, 16, true);




        /*
        Physics2D.IgnoreLayerCollision(2, 9, true);
        Physics2D.IgnoreLayerCollision(2, 10, true);
        Physics2D.IgnoreLayerCollision(2, 11, true);
        Physics2D.IgnoreLayerCollision(2, 16, true);
        Physics2D.IgnoreLayerCollision(2, 18, true);

        Physics2D.IgnoreLayerCollision(18, 9, true); //Makes the substitute chair ignore goals and bumpers and the crowd
        Physics2D.IgnoreLayerCollision(18, 10, true);
        Physics2D.IgnoreLayerCollision(18, 11, true);
        Physics2D.IgnoreLayerCollision(18, 16, true);
        */

        

        simultaneousTurns = false;
        matchStarted = false;
        turnActive = false;
        matchEnded = false;

        titleMenuController = FindObjectOfType<TitleMenuController>();
        modeController = FindObjectOfType<ModeController>();
		canvasManager = FindObjectOfType<CanvasManager>();
        cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
        crowdController = FindObjectOfType<CrowdController>();
        particleManager = FindObjectOfType<ParticleManager>();
        cursorController = FindObjectOfType<CursorController>();
        quoteManager = FindObjectOfType<QuoteManager>();

        if(homeDefenseZone != null && awayDefenseZone != null) {
            homeDefenseZone.gameObject.SetActive(false);
            awayDefenseZone.gameObject.SetActive(false);
        }
	}

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            CastRay();

            UndisplayTutorial();
        }
    }

    void CastRay() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        for(int i = 0; i < hits.Length; i++) {
            //Debug.Log(hits[i].collider.gameObject.name);
            if (hits[i].collider.gameObject.GetComponent<AthleteController>() != null) {
                AthleteController ac = hits[i].collider.gameObject.GetComponentInParent<AthleteController>();
                //if(!ac.IsDisabled()) {
                    ac.MouseClick();
                    break;
                //}
            }
        }
    }

    public void PretendMouseJustAppeared() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        AthleteController ac = null;
        for(int i = 0; i < hits.Length; i++) {
            if (hits[i].collider.gameObject.GetComponent<TailTip>() != null) { //If the tail is being hovered
                ac = hits[i].collider.gameObject.GetComponentInParent<AthleteController>();
                break;
            } else if(hits[i].collider.gameObject.GetComponent<AthleteController>() != null) {
                ac = hits[i].collider.gameObject.GetComponent<AthleteController>();
                break;
            }
        }

        if(ac != null) {
            Debug.Log("Pretend the mouse entered!");

            ac.MouseEnter();
        }
    }

    public void SetMatchup(MatchData matchup) { //Only called for tournaments right now
        SetupCourt(modeController.GetRuleSet());

        SetSide(true, matchup.homeTeamData.team);
        SetSide(false, matchup.awayTeamData.team);

        SetMatchData(matchup);
    }

    public void SetMatchData(MatchData match) {
        matchData = match;
    }

	public void SetupCourt(RuleSet rules) {
        matchStarted = false;
        turnActive = false;
        matchEnded = false;
        overtime = false;

        ClearField();

        ruleSet = rules;

		homeSpawnBox.SetSpawnBox(ruleSet.GetRule("athleteFieldCount").value);
        awaySpawnBox.SetSpawnBox(ruleSet.GetRule("athleteFieldCount").value);

        SetBumpers();

        SpawnBalls();
	}

    public void SetBumpers() {
        for(int i = homeBumpers.Count - 1; i > -1; i--) {
            Destroy(homeBumpers[i].gameObject);
        }

        for(int i = awayBumpers.Count - 1; i > -1; i--) {
            Destroy(awayBumpers[i].gameObject);
        }

        homeBumpers = new List<Bumper>();
        awayBumpers = new List<Bumper>();

        int numBumpers = ruleSet.GetRule("bumperCount").value;

        Vector3 adjustedScale = bumperPrefab.transform.localScale;
        adjustedScale.y = adjustedScale.y / numBumpers;

        GameObject fakeBumper = Instantiate(bumperPrefab, Vector3.zero, Quaternion.identity);
        fakeBumper.transform.localScale = adjustedScale;
        float bumperOffset = fakeBumper.GetComponent<SpriteRenderer>().bounds.size.y / 3.33333f; //Really have no clue why 3.333 is the right number. But it is
        Destroy(fakeBumper);

        float spawnUpperLimit = upperLimit.position.y - bumperOffset;
        float spawnLowerLimit = lowerLimit.position.y - bumperOffset;

        for(int i = 0; i < numBumpers; i++) {
            float spawnHeight = Mathf.Lerp(spawnLowerLimit, spawnUpperLimit, ((i + 1f) / (float)numBumpers));

            Vector3 homeSpawnPosition = homeBumperHolder.position;
            homeSpawnPosition.y = spawnHeight;
            GameObject newHomeBumper = Instantiate(bumperPrefab, homeSpawnPosition, Quaternion.identity, homeBumperHolder);
            homeBumpers.Add(newHomeBumper.GetComponent<Bumper>());

            Vector3 awaySpawnPosition = awayBumperHolder.position;
            awaySpawnPosition.y = spawnHeight;
            GameObject newAwayBumper = Instantiate(bumperPrefab, awaySpawnPosition, Quaternion.identity, awayBumperHolder);
            Bumper awayBump = newAwayBumper.GetComponent<Bumper>();
            awayBumpers.Add(awayBump);
            newAwayBumper.transform.localEulerAngles = Vector3.zero;
            
            Vector2 bumpDir = awayBump.bumpDirection;
            awayBump.bumpDirection = -bumpDir;

            newHomeBumper.transform.localScale = adjustedScale;
            newAwayBumper.transform.localScale = adjustedScale;
        }
    }

    public void ClearField() {
        for(int i = allAthletesOnField.Count - 1; i >= 0; i--) {
            if(allAthletesOnField[i].GetSpeaking()) {
                allAthletesOnField[i].GetCurrentQuoteBox().PrematurelyClose();
            } 
            Destroy(allAthletesOnField[i].gameObject);
        }
        allAthletesOnField = new List<AthleteController>();
        homeAthletesOnField = new List<AthleteController>();
        awayAthletesOnField = new List<AthleteController>();
        inactiveAwayAthletes = new List<AthleteController>();
        inactiveHomeAthletes = new List<AthleteController>();
        inboundSubstitutes = new List<AthleteController>();
        outboundSubstitutes = new List<AthleteController>();

        for(int i = ballsOnField.Count - 1; i >= 0; i--) {
            Destroy(ballsOnField[i].gameObject);
        }
        ballsOnField = new List<BallController>();

        bumperTriggersLastEntered = new List<Bumper>();

        //Assumes home == away bumper count
        for(int i = 0; i < homeBumpers.Count; i++) {
            if(homeBumpers[i].GetDisabled()) {
                homeBumpers[i].RestoreBumper();
            }
            if(awayBumpers[i].GetDisabled()) {
                awayBumpers[i].RestoreBumper();
            }
        }

        if(crowdController != null) {
            crowdController.ClearCrowd();
        }
    }

	public Team GetTeam(bool home) {
		if(home) {
			return homeTeam;
		} else {
			return awayTeam;
		}
	}

	public void SetTeam(bool home, Team team) {
		if(home) {
			homeTeam = team;
		} else {
			awayTeam = team;
		}
	}

    public void ResetSides() {
        ClearField();

        SetSide(true, homeTeam);
        SetSide(false, awayTeam);

        SpawnBalls();
    }

	public void SetSide(bool homeSide, Team teamSelected) {
        if(homeSide) {
            if(homeTeam != null) {
                if(PlayerPrefs.GetString("mode") != "tournament") {
                    if(homeTeam.computerControlled) {
                        teamSelected.computerControlled = true;
                    } else {
                        teamSelected.computerControlled = false;
                    }
                }
            }

            homeTeam = teamSelected;
            
            homeGoal.GetComponent<Bumper>().SetColor(teamSelected.primaryColor);
            for(int i = 0; i < homeBumpers.Count; i++) {
                homeBumpers[i].SetColor(teamSelected.GetDarkTint());
                homeBumpers[i].SetTeam(homeTeam);
            }
            homeDefenseZone.SetTeam(teamSelected);

            homeSideColor.color = teamSelected.GetLightTint();

            for(int i = homeAthletesOnField.Count - 1; i >= 0; i--) {
                if(homeAthletesOnField[i].GetSpeaking()) {
                    homeAthletesOnField[i].GetCurrentQuoteBox().PrematurelyClose();
                }
                Destroy(homeAthletesOnField[i].gameObject);
            }
            homeAthletesOnField = new List<AthleteController>();

            for(int i = inactiveHomeAthletes.Count - 1; i >= 0; i--) {
                Destroy(inactiveHomeAthletes[i].gameObject);
            }
            inactiveHomeAthletes = new List<AthleteController>();

            if(crowdController != null) {
                crowdController.SetStadiumSeats(homeTeam);
            }
        } else {
            if(awayTeam != null) {
                if(PlayerPrefs.GetString("mode") != "tournament") {
                    if(awayTeam.computerControlled) {
                        teamSelected.computerControlled = true;
                    } else {
                        teamSelected.computerControlled = false;
                    }
                }
            }

            awayTeam = teamSelected;

            awayGoal.GetComponent<Bumper>().SetColor(teamSelected.primaryColor);
            for(int i = 0; i < awayBumpers.Count; i++) {
                awayBumpers[i].SetColor(teamSelected.GetDarkTint());
                awayBumpers[i].SetTeam(awayTeam);
            }
            awayDefenseZone.SetTeam(teamSelected);

            awaySideColor.color = teamSelected.GetLightTint();

            for(int i = awayAthletesOnField.Count - 1; i >= 0; i--) {
                if(awayAthletesOnField[i].GetSpeaking()) {
                    awayAthletesOnField[i].GetCurrentQuoteBox().PrematurelyClose();
                }
                Destroy(awayAthletesOnField[i].gameObject);
            }
            awayAthletesOnField = new List<AthleteController>();

            for(int i = inactiveAwayAthletes.Count - 1; i >= 0; i--) {
                Destroy(inactiveAwayAthletes[i].gameObject);
            }
            inactiveAwayAthletes = new List<AthleteController>();
        }

        if(canvasManager != null) {
			canvasManager.DisplayNewTeamSide(homeSide, teamSelected);
		}

        SpawnAthletes(homeSide);

        homeSpawnBox.Disable();
        awaySpawnBox.Disable();
    }

	public void SpawnAthletes(bool homeSide) {
        int totalAthletes = ruleSet.GetRule("athleteRosterCount").value;
        int totalSpawnedAthletes = ruleSet.GetRule("athleteFieldCount").value;

        List<float> spawnHeights = homeSpawnBox.GetSpawnHeights();

        for(int i = 0; i < totalAthletes; i++) {
            Athlete athlete;

            Vector3 spawnSpot = Vector3.zero;
            Vector3 spawnAngle = Vector3.zero;

            //This shit doesn't actually work rn if you go beyond the number of spawnHeights.Count

            if(homeSide) {
                athlete = homeTeam.athletes[i];

                if(i < totalSpawnedAthletes) {

                    float spawnX = homeSpawnBox.transform.localPosition.x;
                    float spawnY = 0f;

                    spawnX += (i / spawnHeights.Count * 2f);
                    spawnY = spawnHeights[i % spawnHeights.Count];

                    spawnSpot = new Vector3(spawnX, spawnY, 0);
                    spawnAngle = new Vector3(0, 0, 90);
                }
            } else {
                athlete = awayTeam.athletes[i];

                if(i < totalSpawnedAthletes) {
                    
                    float spawnX = awaySpawnBox.transform.localPosition.x;
                    float spawnY = 0f;

                    spawnX += (i / spawnHeights.Count * 0.1f);
                    spawnY = spawnHeights[i % spawnHeights.Count];

                    spawnSpot = new Vector3(spawnX, spawnY, 0);
                    spawnAngle = new Vector3(0, 0, 270);
                }
            }

            GameObject prefabToSpawn = athleteStandardPrefab;

            GameObject newAthleteObj = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(spawnAngle), athleteHolder);
			newAthleteObj.transform.localPosition = spawnSpot;
            AthleteController ac = newAthleteObj.GetComponent<AthleteController>();
            ac.SetAthlete(athlete);

            if(homeSide) {
                ac.SetBodyLayer(12);
                
                if(i < totalSpawnedAthletes) {
                    homeAthletesOnField.Add(ac);
                } else {
                    inactiveHomeAthletes.Add(ac);
                    ac.gameObject.SetActive(false);
                }
            }  else {
                ac.SetBodyLayer(13);

                if(i < totalSpawnedAthletes) {
                    awayAthletesOnField.Add(ac);
                } else {
                    inactiveAwayAthletes.Add(ac);
                    ac.gameObject.SetActive(false);
                }
            }

            if(!simultaneousTurns) {
                ac.SetInstantInteraction(true);
            }

            if(!unbounded) {
                ac.DisableInteraction();
            } else {
                ac.EnableInteraction();
            }
        }

        SetAthletesOnField();
    }

    public void AthleteHovered(AthleteController ac) {
        //Debug.Log("Athlete hovered");

        if(athleteBeingDragged == null) {
            athleteBeingHovered = ac;

            if(canvasManager != null) {
                canvasManager.DisplayFootnotePanel(ac.GetAthlete());
            }

            if(!turnActive && !ac.IsDisabled() && !ac.GetAthlete().GetTeam().computerControlled && matchStarted) {
                cursorController.SetHover(true);
            }

            Athlete a = ac.GetAthlete();

            if(!turnActive && !ac.GetParalyzed()) {
                string quote = "";
                if(!matchStarted) {
                    if(!a.GetTeam().computerControlled) { //If it's not AI, assume it's a player
                        quote = a.GetQuote("preMatchTeam");
                    } else {
                        quote = a.GetQuote("preMatchOpponent");
                    }
                } else if(matchEnded) {
                    if(matchData.GetWinner() == a.GetTeam()) {
                        quote = a.GetQuote("postMatchWin");
                    } else {
                        quote = a.GetQuote("postMatchLose");
                    }
                }else {
                    if(a.GetTeam() == GetTurnTeam()) {
                        quote = a.GetQuote("preFlingTeam");
                    } else {
                        quote = a.GetQuote("preFlingOpponent");
                    }
                }

                if(!athleteBeingDragged && !ac.GetFace().IsExpressing() && !ac.GetSpeaking() && !matchEnded) {
				    ac.GetFace().SetFaceSprite("hovered");
			    }

                if(!unbounded) {
                    if(!ac.GetSpokeThisTurn()) {
                        DisplayQuote(ac, quote);
                    }
                } else {
                    /*
                    if(!ac.IsDisabled()) {
                        DisplayQuote(ac, titleMenuController.GetTutorialQuote());
                    }
                    */
                }
            }
        }
    }

    public void DisplayQuote(AthleteController ac, string quote) {
        if(ac.gameObject.activeSelf) {
            ac.GetFace().ChangeExpression("speaking", 2.1f);
        }

        ac.SetSpokeThisTurn(true);

        if(!ac.GetSpeaking()) {
            quoteManager.DisplayQuote(ac, quote);
        }
    }

    public void AthleteUnhovered(AthleteController ac) {
        if(athleteBeingDragged == null) {
            athleteBeingHovered = null;

            cursorController.SetHover(false);

            if(canvasManager != null && !turnActive) {
                canvasManager.HideFootnotePanel();
            }
        }
    }

    public AthleteController GetAthleteBeingHovered() {
        return athleteBeingHovered;
    }

    public void SetAthleteBeingDragged(AthleteController athlete) {
        if(athlete == null) {
            if(athleteBeingDragged != null) {
                athleteBeingDragged.GetComponent<SortingGroup>().sortingLayerName = "Athletes";

                if(!athleteBeingDragged.GetAthlete().GetTeam().computerControlled) {
                    cursorController.SetDragging(false);
                }
            }
        } else {
            athlete.GetComponent<SortingGroup>().sortingLayerName = "Focal Athlete";


            /*
            if(athlete.GetSpeaking()) { //If the athlete is speaking, close the quote box prematurely
                athlete.GetCurrentQuoteBox().PrematurelyClose();
            }
            */
            
            if(!athlete.GetAthlete().GetTeam().computerControlled) {
                cursorController.SetDragging(true);
            }
        }

        athleteBeingDragged = athlete;
    }

    public AthleteController GetAthleteBeingDragged() {
        return athleteBeingDragged;
    }

	public void SetUnbounded(bool isUnboundedByTurns) {
		unbounded = isUnboundedByTurns;
	}

    public bool GetUnbounded() {
        return unbounded;
    }

	public void StartMatch() {
        turnCap = ruleSet.GetRule("turnCount").value;

        matchStarted = true;
        overtime = false;

        audioManager.Play("matchStart");
        audioManager.AmbientPlay("matchMusic");

        if(matchData == null) {
            SetMatchData(new MatchData(homeTeam, awayTeam, ruleSet));
        }

		if(canvasManager != null) {
        	canvasManager.DisplayStartMatch();
		}

        if(crowdController != null) {
            crowdController.SetCrowd();
            crowdController.StartCoroutine(crowdController.FlashSteps(Color.white, "matchOverLight", 2, 0.03f));
        }

        homeGoal.SetTeamRelations(homeTeam, awayTeam);
        awayGoal.SetTeamRelations(awayTeam, homeTeam);
        SpawnSubstituteChairs();

        //Kind of stupid but this essentially subtracts a turn
        homeTurn = !homeStarts;
        turnNumber = 0;
        StartNextTurn();
    }

    public void SetAthletesOnField() {
        allAthletesOnField = new List<AthleteController>();
        for(int i = 0; i < homeAthletesOnField.Count; i++) {
            allAthletesOnField.Add(homeAthletesOnField[i]);
        }

        for(int i = 0; i < awayAthletesOnField.Count; i++) {
            allAthletesOnField.Add(awayAthletesOnField[i]);
        }
    }

    public void SpawnBalls() {
        int numBalls = ruleSet.GetRule("ballCount").value;
        ballSpawnBox.SetSpawnBox(numBalls);

        for(int i = 0; i < numBalls; i++) {
            Vector3 ballPosition = new Vector3(0, ballSpawnBox.GetSpawnHeights()[i], 0);
            BallController newBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, ballHolder).GetComponent<BallController>();
			newBall.transform.localPosition = ballPosition;
            ballsOnField.Add(newBall);
        }
        ballSpawnBox.Disable();
    }

    public MatchData GetMatchData() {
        return matchData;
    }

    public bool GetMatchStarted() {
        return matchStarted;
    }
    
    public bool GetMatchEnded() {
        return matchEnded;
    }

    public List<BallController> GetBallsOnField() {
        return ballsOnField;
    }

    public List<AthleteController> GetAllAthletesOnField() {
        return allAthletesOnField;
    }

    public IEnumerator WaitToStartTurn() {
        if(ballRespawning) {
            ResetBumpers();
            RespawnBalls();
            //yield return new WaitForSeconds(0.5f);
        }

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();

        while(CheckForMovement()) {
            yield return waiter;
        }

        StartNextTurn();
    }

	public void StartNextTurn() {
        Debug.Log("Starting turn");

        turnActive = false;

        audioManager.Play("nextTurn");

        //StartCoroutine(cameraController.ZoomToSize(4.2f));

        if(homeTurn) { //If home just went, increase the turn number
            turnNumber++;

            if(canvasManager != null) {
                canvasManager.StartCoroutine(canvasManager.AnimateTurnIndicator());
            }
        }

        if(turnNumber >= turnCap) {
            if(crowdController != null) {
                crowdController.StartCoroutine(crowdController.FlashSteps(Color.white, "matchOverLight", 2, 0.01f));
            }
        }

        homeTurn = !homeTurn;

        //These are relics (I think)
        homeReady = false;
        awayReady = false;

        athleteInitiater = null;

        if(canvasManager != null) {
        	canvasManager.DisplayNextTurn(turnNumber);

            UndisplayTutorial();
		}

        /*
        if(turnNumber >= turnCap) {
            audioManager.AmbientPlay("lastTurn");
        }
        */

        homeDefenseZone.gameObject.SetActive(false);
        awayDefenseZone.gameObject.SetActive(false);

        for(int i = 0; i < ballsOnField.Count; i++) {
            ballsOnField[i].ResetTouchOrder();
        }

		if(!unbounded) {
            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                AthleteController a = homeAthletesOnField[i];
                
                a.PrepareForNextTurn(homeTurn);
            }
            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                AthleteController a = awayAthletesOnField[i];

                a.PrepareForNextTurn(!homeTurn);
                }
		} else {
            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                homeAthletesOnField[i].EnableInteraction();
            }
            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                awayAthletesOnField[i].EnableInteraction();
            }
        }

        for(int i = 0; i < homeAthletesOnField.Count; i++) {
            homeAthletesOnField[i].SetSpokeThisTurn(false);
        }
        for(int i = 0; i < awayAthletesOnField.Count; i++) {
            awayAthletesOnField[i].SetSpokeThisTurn(false);
        }
        for(int i = 0; i < inactiveHomeAthletes.Count; i++) {
            inactiveHomeAthletes[i].SetSpokeThisTurn(false);
        }
        for(int i = 0; i < inactiveAwayAthletes.Count; i++) {
            inactiveAwayAthletes[i].SetSpokeThisTurn(false);
        }
        

        AllowSubstitutions(homeTurn);
        if(unbounded) {
            AllowSubstitutions(!homeTurn);
        }

        PretendMouseJustAppeared();
        
        if(crowdController != null) {
            crowdController.SetFocus();
        }

        if(!matchEnded) {
            if(homeTurn) {
                if(homeTeam.computerControlled) {
                   StartCoroutine(ComputerTurn(true, (Vector2)awayGoal.transform.position));
                }
            } else {
                if(awayTeam.computerControlled) {
                    StartCoroutine(ComputerTurn(false, (Vector2)homeGoal.transform.position));
                }
            }
        }
    }

    public void SpawnSubstituteChairs() {
        Vector2 spawnSpot = homeGoal.GetSubPlatform().GetSubChairOrigin();
        Vector2 endSpot = homeGoal.GetSubPlatform().GetSubChairRest();
        Vector3 spawnAngle = new Vector3(0, 0, 90);
        homeSubChair = Instantiate(substituteChair, spawnSpot, Quaternion.Euler(spawnAngle), substituteHolder).GetComponent<SubstituteChair>();
        
        spawnSpot = awayGoal.GetSubPlatform().GetSubChairOrigin();
        endSpot = awayGoal.GetSubPlatform().GetSubChairRest();
        spawnAngle = new Vector3(0, 0, 270);
        awaySubChair = Instantiate(substituteChair, spawnSpot, Quaternion.Euler(spawnAngle), substituteHolder).GetComponent<SubstituteChair>();

        DisableSubChair(true);
        DisableSubChair(false);

        homeSubChair.SetChair(true);
        awaySubChair.SetChair(false);
  }

    public void AllowSubstitutions(bool home) {
        Vector2 spawnSpot = Vector3.zero;
        Vector2 endSpot = Vector3.zero;
        Vector3 spawnAngle = Vector3.zero;

        AthleteController athleteSubbingIn = null;

        inboundSubstitutes = new List<AthleteController>();

        if(home) {
            homeGoal.SetCollidersEnabled(false);
            if(!homeGoal.GetSubstituteReadyForLaunch()) {
                int substitutesNeeded = ruleSet.GetRule("athleteFieldCount").value - homeAthletesOnField.Count;
                if(substitutesNeeded > 0) {
                    homeSubAvailable = true;

                    homeGoal.OpenSubPlatform();

                    spawnSpot = homeGoal.GetSubPlatform().GetSubChairOrigin();
                    endSpot = homeGoal.GetSubPlatform().GetSubChairRest();
                    spawnAngle = new Vector3(0, 0, 90);

                    athleteSubbingIn = inactiveHomeAthletes[0];

                    inboundSubstitutes.Add(athleteSubbingIn);
                } //Not sure yet for multiple subs
            }
        } else {
            awayGoal.SetCollidersEnabled(false);
            if(!awayGoal.GetSubstituteReadyForLaunch()) {
                int substitutesNeeded = ruleSet.GetRule("athleteFieldCount").value - awayAthletesOnField.Count;
                if(substitutesNeeded > 0) {
                    awaySubAvailable = true;
                    
                    awayGoal.OpenSubPlatform();

                    spawnSpot = awayGoal.GetSubPlatform().GetSubChairOrigin();
                    endSpot = awayGoal.GetSubPlatform().GetSubChairRest();
                    spawnAngle = new Vector3(0, 0, 270);

                    athleteSubbingIn = inactiveAwayAthletes[0];

                    inboundSubstitutes.Add(athleteSubbingIn);
                }
            }
        }

        if(athleteSubbingIn != null) {
            SubstituteChair subChair = null;
            if(home) {
                subChair = homeSubChair;
            } else {
                subChair = awaySubChair;
            }

            subChair.gameObject.SetActive(true);

            subChair.transform.position = spawnSpot;

            //subChair.SetChair(home);

            athleteSubbingIn.PrepareSubstitute(spawnSpot, spawnAngle, subChair.transform);

            StartCoroutine(MoveSubChairTo(subChair, endSpot));
        }
    }

    public IEnumerator MoveSubChairTo(SubstituteChair subChair, Vector2 endPos) {
        Vector3 startPos = subChair.transform.position;

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float duration = 0.5f;
        float timer = 0f;
        while(timer < duration) {
            timer += Time.deltaTime;

            subChair.transform.position = Vector3.Lerp(startPos, endPos, timer/duration);

            yield return waiter;
        }

        subChair.SetInteractable(true);
    }

    public void DisableSubChair(bool home) {
        if(home) {
            homeSubChair.gameObject.SetActive(false);
        } else {
            awaySubChair.gameObject.SetActive(false);
        }
    }

    public IEnumerator ComputerTurn(bool home, Vector2 targetGoal) {
        Debug.Log("Computer turn");
        //Incorportate Subs

        yield return new WaitForSeconds(0.5f);

        AthleteController chosenAthlete = null;
        Vector2 target = Vector2.zero;

        
        bool usingSubstitute;

        if(home && homeSubAvailable) {
            usingSubstitute = true;

            BallController closestBall = null;
            float closestDistance = 999f;
            for(int i = 0; i < ballsOnField.Count; i++) {
                float difference = ballsOnField[i].transform.position.x - homeGoal.transform.position.x;
                difference = Mathf.Abs(difference);
                if(difference < closestDistance) {
                    closestBall = ballsOnField[i];
                    closestDistance = difference;

                    target = closestBall.transform.position;
                } 
            }
        } else if(!home && awaySubAvailable) {
            usingSubstitute = true;

            BallController closestBall = null;
            float closestDistance = 999f;
            for(int i = 0; i < ballsOnField.Count; i++) {
                float difference = ballsOnField[i].transform.position.x - awayGoal.transform.position.x;
                difference = Mathf.Abs(difference);
                if(difference < closestDistance) {
                    closestBall = ballsOnField[i];
                    closestDistance = difference;

                    target = closestBall.transform.position;
                } 
            }
        } else {
            usingSubstitute = false;

            List<AthleteController> athletes;
            if(home) {
                athletes = homeAthletesOnField;
            } else {
                athletes = awayAthletesOnField;
            }

            AthleteController athleteClosestToBall = null;
            float howCloseTheyBe = 999f;
            for(int a = 0; a < athletes.Count; a++) {
                AthleteController checkedAthlete = athletes[a];
                if(!checkedAthlete.GetParalyzed()) {
                    for(int b = 0; b < ballsOnField.Count; b++) {
                        BallController checkedBall = ballsOnField[b];
                        if(targetGoal.x < 0) { //If it's the home goal (and they're the away team)
                            if(checkedBall.transform.position.x < checkedAthlete.transform.position.x) {
                                Vector2 distance = checkedBall.transform.position - checkedAthlete.transform.position;
                                float difference = Mathf.Abs(distance.magnitude);
                                if(difference < howCloseTheyBe) {
                                    target = checkedBall.transform.position;
                                    athleteClosestToBall = checkedAthlete;
                                    howCloseTheyBe = difference;
                                }
                            } // else the athlete is in between the ball and their target goal
                        } else {
                            if(checkedBall.transform.position.x > checkedAthlete.transform.position.x) {
                                Vector2 distance = checkedBall.transform.position - checkedAthlete.transform.position;
                                float difference = Mathf.Abs(distance.magnitude);
                                if(difference < howCloseTheyBe) {
                                    target = checkedBall.transform.position;
                                    athleteClosestToBall = checkedAthlete;
                                    howCloseTheyBe = difference;
                                }
                            } // else the athlete is in between the ball and their target goal
                        }
                    }
                } //else they're paralyzed and you should move to the next athlete
            }
            chosenAthlete = athleteClosestToBall;
            
            if(chosenAthlete == null) {
                AthleteController athleteClosestToGoal = null;
                float howClose = 999f;
                for(int a = 0; a < athletes.Count; a++) {
                    AthleteController checkedAthlete = athletes[a];
                    if(targetGoal.x < 0) { //If it's the home goal (and they're the away team)
                        if(targetGoal.x < checkedAthlete.transform.position.x) {
                            Vector2 distance = targetGoal - (Vector2)checkedAthlete.transform.position;
                            float difference = Mathf.Abs(distance.magnitude);
                            if(difference < howClose) {
                                athleteClosestToGoal = checkedAthlete;
                                howClose = difference;
                            }
                        }
                    } else {
                        if(targetGoal.x > checkedAthlete.transform.position.x) {
                            Vector2 distance = targetGoal - (Vector2)checkedAthlete.transform.position;
                            float difference = Mathf.Abs(distance.magnitude);
                            if(difference < howClose) {
                                athleteClosestToGoal = checkedAthlete;
                                howClose = difference;
                            }
                        }
                    }
                }

                target = targetGoal;
                chosenAthlete = athleteClosestToGoal;
            }
        }

        Vector3 adjustedTarget = (Vector3)target;
        Vector3 finalTarget = Vector3.zero;
        Vector3 startVector = Vector3.zero;

        SubstituteChair subChair = null;

        if(usingSubstitute) {
            if(home) {
                subChair = homeSubChair;
            } else {
                subChair = awaySubChair;
            }
            chosenAthlete = subChair.GetCurrentSubstitute();

            adjustedTarget.x = subChair.transform.position.x;

            startVector = subChair.transform.position;

            finalTarget = adjustedTarget;
            finalTarget.x = subChair.transform.position.x + subChair.GetMaxStretch().x;
        } else {
            adjustedTarget = (Vector3)target - chosenAthlete.transform.position; //Get the difference between the target and the athlete
        
            SetAthleteBeingDragged(chosenAthlete);
            chosenAthlete.ComputerClick();
        }

        Vector2 randomOffset = Random.insideUnitCircle / 3;
        target += randomOffset;

        if(canvasManager != null) {
            canvasManager.DisplayFootnotePanel(chosenAthlete.GetAthlete());
        }

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        float timer = 0f;
        float duration = 0.8f;
        while(timer < duration) {
            timer += Time.deltaTime;

            Vector3 stepToTarget = Vector3.Lerp(startVector, adjustedTarget, timer/duration);

            if(usingSubstitute) {
                subChair.AdjustPosition(stepToTarget);
            } else {
                chosenAthlete.TailAdjusted(stepToTarget);
            }

            yield return waiter;
        }

        if(usingSubstitute) {
            timer = 0f;
            duration = 0.6f;
            while(timer < duration) {
                timer += Time.deltaTime;

                Vector3 stepToTarget = Vector3.Lerp(adjustedTarget, finalTarget, timer/duration);

                subChair.AdjustPosition(stepToTarget);

                yield return waiter;
            }
        }

        if(usingSubstitute) {
            subChair.ChairClicked();
        } else {
            chosenAthlete.StartAction();
        }
    }

    public IEnumerator ComputerSubstitution(SubstituteChair subChair) {

        yield return new WaitForSeconds(0.1f);

        subChair.ChairClicked();

    }

    public Team GetTurnTeam() {
        Team team;

        if(homeTurn) {
            team = homeTeam;
        } else {
            team = awayTeam;
        }

        return team;
    }

    public int GetTurn() {
        return turnNumber;
    }

	 public bool IsHomeTurn() {
        return homeTurn;
    }

    public void DisableAllAthleteInteraction() {
        for(int i = 0; i < homeAthletesOnField.Count; i++) {
            homeAthletesOnField[i].DisableInteraction();
        }
        for(int i = 0; i < awayAthletesOnField.Count; i++) {
            awayAthletesOnField[i].DisableInteraction();
        }
    }

    /*
    public void DetermineGravityFields(AthleteController initiator) {
        if(initiator != null) {
            for(int i = 0; i < allAthletesOnField.Count; i++) {
                AthleteController a = allAthletesOnField[i];

                string typeString = a.GetAthlete().athleteData.classString;
                if(typeString == "Circle") {
                    if(a.GetAthlete().GetTeam() == initiator.GetAthlete().GetTeam() && a != initiator) {
                        a.EnableGravityField();
                    }
                } else if(typeString == "Triangle") {
                    if(a == initiator) {
                        a.EnableGravityField();
                    }
                }
            }
        } else {
            for(int i = 0; i < allAthletesOnField.Count; i++) {
                allAthletesOnField[i].DisableGravityField();
            }
        }
    }
    */

//Fling
    public void BeginActiveTurnPhase(AthleteController initiatior) {
        athleteInitiater = initiatior;

        turnActive = true;

        homeGoal.SetCollidersEnabled(true);
        awayGoal.SetCollidersEnabled(true);   

        /*
        if(turnNumber >= turnCap) {
            audioManager.AmbientStop("lastTurn");
        }
        */

        //StartCoroutine(cameraController.ZoomToSize(5f));

        SetAthleteBeingDragged(null);
        AthleteUnhovered(initiatior);

		if(!unbounded) {
            for(int i = 0; i < allAthletesOnField.Count; i++) {
                AthleteController a = allAthletesOnField[i];

                a.BeginActiveTurn();

                /*
                if(a.IsGravityFieldEnabled()) {
                    a.ActivateGravityField();
                }
                */
            }

            homeDefenseZone.gameObject.SetActive(true);
            awayDefenseZone.gameObject.SetActive(true);

            if(homeTurn) {
                if(homeSubAvailable) {
                    homeGoal.CloseSubPlatform();
                }
            } else {
                if(awaySubAvailable) {
                    awayGoal.CloseSubPlatform();
                }
            }   
        }

        if(canvasManager != null) {
            canvasManager.DisplayTurnActive(initiatior.GetAthlete().GetTeam());
        }

        if(titleMenuController != null) {
            titleMenuController.DisableReturnButton();
        }

        if(waitForTurnEndCoroutine == null) {
            waitForTurnEndCoroutine = StartCoroutine(WaitForTurnEnd());
        } //else the turn is already being ended
    }

    public bool IsTurnActive() {
        return turnActive;
    }

    public AthleteController GetAthleteInitiater() {
        return athleteInitiater;
    }

    private Coroutine waitForTurnEndCoroutine;

    public IEnumerator WaitForTurnEnd() {

        yield return new WaitForSeconds(0.1f);

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
       
        while(CheckForMovement() || substituteInbound) {
            yield return waiter;
        }

        /*
        for(int i = 0; i < outboundSubstitutes.Count; i++) {
            if(outboundSubstitutes[i].GetAthlete().GetTeam() == homeTeam) {
               yield return StartCoroutine(WaitForSubstitute(true));
            } else {
               yield return StartCoroutine(WaitForSubstitute(false));
            }
        }

        outboundSubstitutes = new List<AthleteController>();
        */

        StartCoroutine(EndTurn());
        waitForTurnEndCoroutine = null;
    }

    public bool CheckForMovement() {
        bool athleteStillMoving = false;
        for(int i = 0; i < allAthletesOnField.Count; i++) {
            if(allAthletesOnField[i].GetMoving()) {
                athleteStillMoving = true;
                break;
            }
        }

        bool ballStillMoving = false;
        for(int i = 0; i < ballsOnField.Count; i++) {
            if(ballsOnField[i].gameObject.activeSelf) {
                if(ballsOnField[i].GetMoving() || ballsOnField[i].GetScoreAnimationInProgress()) {
                    ballStillMoving = true;
                    break;
                }
            }
        }

        bool bumperStillMoving = false;
        for(int i = 0; i < homeBumpers.Count; i++) {
            if(homeBumpers[i].GetImmunue()) {
                bumperStillMoving = true;
            }
        }
        for(int i = 0; i < awayBumpers.Count; i++) {
            if(awayBumpers[i].GetImmunue()) {
                bumperStillMoving = true;
            }
        }

        if(athleteStillMoving || ballStillMoving || bumperStillMoving) {
            return true;
        } else {
            return false;
        }
    }

    public void PrematurelyEndTurn() {
        if(turnActive) {
            Debug.Log("Prematurely ended turn");

            for(int i = 0; i < allAthletesOnField.Count; i++) {
                allAthletesOnField[i].StoppedMoving();
            }

            for(int i = 0; i < ballsOnField.Count; i++) {
                ballsOnField[i].StopMovement();
            }

            StartCoroutine(EndTurn());
        }
    }

    //This is no longer a coroutine so I did a lil yieldy yieldy. All the coroutine stuff has been moved to pre-turn
    public IEnumerator EndTurn() {
        Debug.Log("Ending turn");

        yield return new WaitForFixedUpdate();

        if(titleMenuController != null) {
            titleMenuController.EnableReturnButton();
        }

        bool tied = false;
        if(matchData.homeTeamData.GetScore() == matchData.awayTeamData.GetScore()) {
            tied = true;
        }
        
        if(!overtime) {
            if(turnNumber == turnCap && homeTurn) { //If it's the last turn and the homeTeam just went

                if(tied) {
                    overtime = true;
                    
                    BeginOvertime();
                } else {
                    EndMatch();
                }
            } else {
                StartCoroutine(WaitToStartTurn());
            }
        } else {
            //Settings for golden goal
            if(tied || !homeTurn) { //If it's tied or the away team just went
                StartCoroutine(WaitToStartTurn());
            } else {
                EndMatch();
            }            
        }
    }

    public void BeginGoldenGoal() {
        StartNextTurn();
    }

    public void BeginOvertime() {
        StartCoroutine(WaitToStartTurn());
    }

	public void ScoreGoal(Team teamThatScored, BallController ball, Transform goal) {
        //Debug.Log(teamThatScored.name + " scored a goal!");
        Vector3 goalCenter = goal.position;

        ballRespawning = true;

        particleManager.PlayGoal(ball.transform, goal, teamThatScored);

        cameraController.AddTrauma(1f);
        if(teamThatScored == homeTeam) {
            audioManager.Play("goalHomeHorn");
        } else {
            audioManager.Play("goalAwayHorn");
        }

        if(ball.GetLastBumper() == null) {
            Debug.Log("Ball didn't enter a bumper zone");
        } else {
            bumperTriggersLastEntered.Add(ball.GetLastBumper());
        }


        ball.ScoreBall(teamThatScored, athleteInitiater, goalCenter);

		if(canvasManager != null) {
        	canvasManager.StartCoroutine(canvasManager.DisplayScore(teamThatScored));
		}

        if(crowdController != null) {
            if(teamThatScored == homeTeam) {
                crowdController.StartCoroutine(crowdController.FlashAwaySteps(teamThatScored.primaryColor, "goalHomeLight", 3, 0.15f));
            } else {
                crowdController.StartCoroutine(crowdController.FlashHomeSteps(teamThatScored.primaryColor, "goalAwayLight", 3, 0.15f));
            }

            crowdController.GoalReaction(teamThatScored);
        }

        CallSlowFrame();
    }

    public void AthleteGoal(AthleteController ac) {
        DisplayQuote(ac, ac.GetAthlete().GetQuote("goal"));

        Team team = ac.GetAthlete().GetTeam();
        matchData.GetTeamMatchData(team).IncreaseScore(1);
    }

    public void AthleteOwnGoal(AthleteController ac) {
        DisplayQuote(ac, ac.GetAthlete().GetQuote("ownGoal"));

        Team team;
        if(ac.GetAthlete().GetTeam() == homeTeam) { //If the own goaler is on the home team, award goal to the away team
            team = awayTeam;
        } else { //else the own goaler is on the away team and goal is awarded to home team
            team = homeTeam;
        }
        matchData.GetTeamMatchData(team).IncreaseScore(1);
    }

    public void RespawnBalls() {
        ballSpawnBox.Enable();

        for(int i = ballsOnField.Count - 1; i >= 0; i--) {
           BallController ball = ballsOnField[i];

           if(ball.GetScoredByTeam() != null) {
               ball.gameObject.SetActive(true);

                List<float> spawnHeights = ballSpawnBox.GetSpawnHeights();
                Vector3 spawnPos = Vector3.zero;
                for(int j = 0; j < spawnHeights.Count; j++) {
                    bool openSpace = true;
                    Vector3 chosenSpawn = Vector3.zero;
                    for(int b = 0; b < ballsOnField.Count; b++) {
                        chosenSpawn = new Vector3(0, spawnHeights[j], 0);
                        if(ballsOnField[b].transform.localPosition == chosenSpawn) {
                            openSpace = false;
                        }
                    }

                    if(openSpace) {
                        spawnPos = chosenSpawn;
                        break;
                    }
                }

                ball.RespawnBall(spawnPos);

                audioManager.Play("ballSpawn");
            }
        }

        ballSpawnBox.Disable();
    }

    public void ResetBumpers() {
        for(int i = 0; i < bumperTriggersLastEntered.Count; i++) {
            bumperTriggersLastEntered[i].RestoreBumper();
        }

        bumperTriggersLastEntered = new List<Bumper>();
    }

    public void RestoreBumper(Bumper bumper) {
        if(bumper != null) {
            bumper.RestoreBumper();
        } else {
            Debug.Log("Bumper is null");
        }
    }

	public void EndMatch() {
    	matchEnded = true;

        turnActive = false;

        if(titleMenuController == null) { //If not on the title screen
            cameraController.AddTrauma(1f);
        }

        matchData.FinalizeMatchData();

        int homeScore = matchData.GetTeamMatchData(homeTeam).GetScore();
        int awayScore = matchData.GetTeamMatchData(awayTeam).GetScore();

        audioManager.AmbientStop("matchMusic");
        if(PlayerPrefs.GetString("mode") == null) {
            if(homeScore > awayScore) {
                audioManager.Play("victory");
            } else if(awayScore > homeScore) {
                audioManager.Play("defeat");
            } else {
                Debug.Log("Tie Game but this message should NEVER display, right?");
                //audioManager.Play("overtime");
            }
        }

        if(canvasManager != null) {
            canvasManager.DisplayEndMatch();
        }

        if(crowdController != null) {
            crowdController.StartCoroutine(crowdController.FlashSteps(Color.white, "matchOverLight", 2, 0.03f));
        }

        homeDefenseZone.gameObject.SetActive(false);
        awayDefenseZone.gameObject.SetActive(false);

        DeterminePostMatchFaceStates(matchData.GetWinner());

        if(modeController != null) {
            modeController.EndMatch(matchData);
        }
    }

    public void DeterminePostMatchFaceStates(Team winningTeam) {
        for(int i = 0; i < allAthletesOnField.Count; i++) {
            if(allAthletesOnField[i].GetAthlete().GetTeam() == winningTeam) {
                allAthletesOnField[i].SetFaceSprite("victory");
            } else {
                allAthletesOnField[i].SetFaceSprite("defeat");
            }
        }

        for(int i = 0; i < inactiveHomeAthletes.Count; i++) {
            if(homeTeam == winningTeam) {
                inactiveHomeAthletes[i].SetFaceSprite("victory");
            } else {
                inactiveHomeAthletes[i].SetFaceSprite("defeat");
            }
        }
        for(int i = 0; i < inactiveAwayAthletes.Count; i++) {
            if(awayTeam == winningTeam) {
                inactiveAwayAthletes[i].SetFaceSprite("victory");
            } else {
                inactiveAwayAthletes[i].SetFaceSprite("defeat");
            }
        }

        if(crowdController != null) {
            crowdController.SetPostMatchFaces();
        }
    }

    /*
    public List<Athlete> GetAthletesOnField(bool home) {
        List<Athlete> athleteList = new List<Athlete>();
        if(home) {
            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                athleteList.Add(homeAthletesOnField[i].GetAthlete());
            }
        } else {
            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                athleteList.Add(awayAthletesOnField[i].GetAthlete());
            }
        }
        return athleteList;
    }
    */

    public void UpdateStats(StatType type, Athlete athlete) {
        if(canvasManager != null) {
            canvasManager.UpdateFootnotePanel(type, athlete);
        }
    }

    public void SubstituteAthleteOut(AthleteController ac, Vector3 goalCenter) {
        cameraController.AddTrauma(0.8f);

        if(audioManager != null) {
            audioManager.Play("subOut");
        }

        outboundSubstitutes.Add(ac);

        allAthletesOnField.Remove(ac);
        if(ac.GetAthlete().GetTeam() == homeTeam) {
            homeAthletesOnField.Remove(ac);
            inactiveHomeAthletes.Add(ac);
        } else {
            awayAthletesOnField.Remove(ac);
            inactiveAwayAthletes.Add(ac);
        }

        if(ac.GetSpeaking()) {
            ac.GetCurrentQuoteBox().PrematurelyClose();
        }

        StartCoroutine(ac.RemoveAthleteFromField(goalCenter));
    }

    public void SubstituteAthleteIn(AthleteController ac) {
        if(audioManager != null) {
            audioManager.Play("subIn");
        }

        inboundSubstitutes.Remove(ac);

        allAthletesOnField.Add(ac);

        substituteInbound = true;

        if(ac.GetAthlete().GetTeam() == homeTeam) {
            homeAthletesOnField.Add(ac);

            inactiveHomeAthletes.Remove(ac);

            bool noMoreSubs = true;
            for(int i = 0; i < inboundSubstitutes.Count; i++) {
                if(inboundSubstitutes[i].GetAthlete().GetTeam() == homeTeam) {
                    noMoreSubs = false;
                    break;
                }
            }
            if(noMoreSubs) {
                homeSubAvailable = false;
            }
        } else {
            awayAthletesOnField.Add(ac);

            inactiveAwayAthletes.Remove(ac);

            bool noMoreSubs = true;
            for(int i = 0; i < inboundSubstitutes.Count; i++) {
                if(inboundSubstitutes[i].GetAthlete().GetTeam() == awayTeam) {
                    noMoreSubs = false;
                    break;
                }
            }
            if(noMoreSubs) {
                awaySubAvailable = false;
            }
        }

        BeginActiveTurnPhase(ac);
    }

    public void ResetSubstituteStatus() {
        substituteInbound = false;
    }

    public void IncreaseTutorialTimer(AthleteController ac, float increment) {
        tutorialTimer += increment;
        
        if(!displayingTutorial && tutorialTimer >= tutorialTimerCap) {
            displayingTutorial = true;
            canvasManager.DisplayTutorialText(ac);
        }
    }

    public void UndisplayTutorial() {
        tutorialTimer = 0f;

        if(displayingTutorial) {
            displayingTutorial = false;
            canvasManager.UndisplayTutorialText();
        }
    }

    /*
    public IEnumerator AutoHideTutorialText(float timeUntilUndisplay) {
        yield return new WaitForSeconds(timeUntilUndisplay);

        UndisplayTutorial();
    }
    */

    public void CallSlowFrame() {
        StartCoroutine(SlowFrame());
    }

    private IEnumerator SlowFrame() {
        Time.timeScale = 0.2f;

        float timeFrozen = 0.1f;
        yield return new WaitForSeconds(timeFrozen);

        Time.timeScale = 1f;
    }

     public void CallFreezeFrame() {
        StartCoroutine(FreezeFrame());
    }

    private IEnumerator FreezeFrame() {
        Time.timeScale = 0f;

        float timeFrozen = 0.05f;
        yield return new WaitForSecondsRealtime(timeFrozen);

        Time.timeScale = 1f;
    }
}
