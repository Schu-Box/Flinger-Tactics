using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class MatchController : MonoBehaviour {

	public GameObject ballPrefab;
    public GameObject athleteStandardPrefab;
    public GameObject shockwavePrefab;

    public int turnCap;

	private Team homeTeam = null;
    private Team awayTeam = null;

    public SpriteRenderer homeSideColor;
    public SpriteRenderer awaySideColor;

    public GoalController homeGoal;
    public GoalController awayGoal;
    public List<Bumper> homeBumpers;
    public List<Bumper> awayBumpers;

	public Transform athleteHolder;
	public Transform ballHolder;
    public Transform substituteHolder;

    public SpawnBox ballSpawnBox;
    public SpawnBox homeSpawnBox;
    public SpawnBox awaySpawnBox;

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

    private List<SubstituteChair> subChairsActive = new List<SubstituteChair>();
    
    //Ready stuff is now unused
    private bool homeReady = false;
    private bool awayReady = false;

    private bool homeSubAvailable = false;
    private bool awaySubAvailable = false;

	private bool unbounded = false;

    private bool matchStarted = false;
    private bool matchEnded = false;
    private bool overtime = false;
    private int turnNumber = 1;
    private bool homeStarts = false;
    private bool homeTurn = false;
    private bool substituteInbound = false;
    private bool turnActive = false;
    private bool ballRespawning = false;

    private bool athleteHovered = false;

    private AthleteController athleteBeingDragged;
    private AthleteController athleteInitiater;

	public static bool simultaneousTurns = true;

    public static bool muted = false;

    private ModeController modeController;
	private CanvasManager canvasManager;
    private CameraController cameraController;
	private AudioManager audioManager;
    private CrowdController crowdController;
    private QuoteManager quoteManager;
    private ParticleManager particleManager;

    private MatchData matchData;
    private RuleSet ruleSet;

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

        modeController = FindObjectOfType<ModeController>();
		canvasManager = FindObjectOfType<CanvasManager>();
        cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
        crowdController = FindObjectOfType<CrowdController>();
        quoteManager = FindObjectOfType<QuoteManager>();
        particleManager = FindObjectOfType<ParticleManager>();
	}

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            CastRay();
        }

        if(Input.GetButtonDown("Cancel")) {
            SceneManager.LoadScene(0);
        }  
    }

    void CastRay() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        for(int i = 0; i < hits.Length; i++) {
            //Debug.Log(hits[i].collider.gameObject.name);
            if (hits[i].collider.gameObject.GetComponent<AthleteController>() != null) {
                AthleteController ac = hits[i].collider.gameObject.GetComponentInParent<AthleteController>();
                if(!ac.IsDisabled()) {
                    ac.MouseClick();
                    break;
                }
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
            ac.MouseEnter();
        }
    }

	public void SetupCourt(RuleSet rules) {
        matchStarted = false;
        turnActive = false;
        matchEnded = false;

        ruleSet = rules;

		homeSpawnBox.SetSpawnBox(ruleSet.GetRule("athleteFieldCount").value);
        awaySpawnBox.SetSpawnBox(ruleSet.GetRule("athleteFieldCount").value);
	}

    public void ClearField() {
        for(int i = allAthletesOnField.Count - 1; i >= 0; i--) {
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
        SetSide(true, homeTeam);
        SetSide(false, awayTeam);
    }

	public void SetSide(bool homeSide, Team teamSelected) {
		if(canvasManager != null) {
			canvasManager.DisplayNewTeamSide(homeSide, teamSelected);
		}

        if(homeSide) {
            homeTeam = teamSelected;
            
            homeGoal.GetComponent<Bumper>().SetColor(teamSelected.primaryColor);
            for(int i = 0; i < homeBumpers.Count; i++) {
                homeBumpers[i].SetColor(teamSelected.GetDarkTint());
            }

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
            awayTeam = teamSelected;

            awayGoal.GetComponent<Bumper>().SetColor(teamSelected.primaryColor);
            for(int i = 0; i < awayBumpers.Count; i++) {
                awayBumpers[i].SetColor(teamSelected.GetDarkTint());
            }

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
    }

    public void AthleteHovered(AthleteController ac) {
        if(canvasManager != null && athleteBeingDragged == null) {
            athleteHovered = true;

            canvasManager.DisplayFootnotePanel(ac.GetAthlete());

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
                    if(matchData.winner == a.GetTeam()) {
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

                if(!athleteBeingDragged && !ac.GetFace().IsExpressing() && !ac.GetSpeaking()) {

				    ac.GetFace().SetFaceSprite("hovered");
			    }

                if(!ac.GetSpokeThisTurn()) {
                    DisplayQuote(ac, quote);
                }
            }
        }
    }

    public void DisplayQuote(AthleteController ac, string quote) {
        Debug.Log("Displaying quote");

        if(ac.gameObject.activeSelf) {
            ac.GetFace().ChangeExpression("speaking", 2.1f);
        }

        ac.SetSpokeThisTurn(true);

        canvasManager.DisplayQuote(ac, quote);
    }

    public void AthleteUnhovered(AthleteController ac) {
        if(canvasManager != null && athleteBeingDragged == null) {
            athleteHovered = false;

            canvasManager.HideFootnotePanel();
        }
    }

    public bool GetAthleteHovered() {
        return athleteHovered;
    }

    public void SetAthleteBeingDragged(AthleteController athlete) {
        if(athlete == null) {
            if(athleteBeingDragged != null) {
                athleteBeingDragged.GetComponent<SortingGroup>().sortingLayerName = "Athletes";
            }
        } else {
            athlete.GetComponent<SortingGroup>().sortingLayerName = "Focal Athlete";

            if(athlete.GetSpeaking()) { //If the athlete is speaking, close the quote box prematurely
                athlete.GetCurrentQuoteBox().PrematurelyClose();
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

	public void StartMatch() {
        turnCap = ruleSet.GetRule("turnCount").value;

        matchStarted = true;
        overtime = false;

        audioManager.Play("matchStart");
        audioManager.AmbientPlay("matchMusic");

        matchData = new MatchData(homeTeam, awayTeam);
        homeTeam.AssignNewMatchData(matchData);
        awayTeam.AssignNewMatchData(matchData);

		if(canvasManager != null) {
        	canvasManager.DisplayStartMatch();
		}

        SetAthletesOnField();

        SpawnBalls();

        homeGoal.SetTeamRelations(homeTeam, awayTeam);
        for(int i = 0; i < homeBumpers.Count; i++) {
            //homeGoals[i].blocker = homeBlockers[i];
            homeBumpers[i].SetTeam(homeTeam);
        }
        awayGoal.SetTeamRelations(awayTeam, homeTeam);
        for(int i = 0; i < awayBumpers.Count; i++) {
            //awayGoals[i].blocker = awayBlockers[i];
            awayBumpers[i].SetTeam(awayTeam);
        }

        if(crowdController != null) {
            crowdController.SetCrowd();
            crowdController.StartCoroutine(crowdController.FlashSteps(Color.white, "matchOverLight", 2, 0.03f));
        }

        //Kind of stupid but this essentially subtracts a turn
        homeTurn = !homeStarts;
        turnNumber = 0;
        StartNextTurn();
    }

    public void SetAthletesOnField() {
        allAthletesOnField = new List<AthleteController>();
        for(int i = 0; i < homeAthletesOnField.Count; i++) { //Assumes home and away team always have the same number
            allAthletesOnField.Add(homeAthletesOnField[i]);
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
        turnActive = false;

        audioManager.Play("nextTurn");

        if(homeTurn) { //If home just went, increase the turn number
            turnNumber++;

            if(canvasManager != null) {
                canvasManager.StartCoroutine(canvasManager.AnimateTurnIndicator());
            }
        }

        homeTurn = !homeTurn;

        //These are relics (I think)
        homeReady = false;
        awayReady = false;

        athleteInitiater = null;

        if(timeoutRequested) {
            timeoutAcceptable = true;
            timeoutRequested = false;
        }

        if(canvasManager != null) {
        	canvasManager.DisplayNextTurn(turnNumber);
		}

        /*
        if(turnNumber >= turnCap) {
            audioManager.AmbientPlay("lastTurn");
        }
        */

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
        

        //Can subs speak?

        AllowSubstitutions(homeTurn);

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

    public void AllowSubstitutions(bool home) {
        Vector2 spawnSpot = Vector2.zero;
        Vector2 endSpot = Vector2.zero;
        Vector3 spawnAngle = Vector3.zero;

        AthleteController athleteSubbingIn = null;

        inboundSubstitutes = new List<AthleteController>();

        if(homeTurn) {
            homeGoal.SetCollidersEnabled(false);

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
        } else {
            awayGoal.SetCollidersEnabled(false);

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

        if(athleteSubbingIn != null) {
            SubstituteChair subChair = Instantiate(substituteChair, spawnSpot, Quaternion.Euler(spawnAngle), substituteHolder).GetComponent<SubstituteChair>();
            subChair.SetChair(home);
            subChairsActive.Add(subChair);

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

    public List<SubstituteChair> GetSubChairsActive() {
        return subChairsActive;
    }

    public void ClearSubChairsActive() {
        for(int i = 0; i < subChairsActive.Count; i++) {
            SubstituteChair subChair = subChairsActive[i];
            /*
            if(subChair.transform.GetComponentInChildren<AthleteController>() != null) {
                subChair.transform.GetComponentInChildren<AthleteController>().transform.SetParent(athleteHolder);
            }
            */
            Destroy(subChair.gameObject);
        }
        subChairsActive = new List<SubstituteChair>();
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

        if(usingSubstitute) {
            SubstituteChair subChair = subChairsActive[0];
            chosenAthlete = subChair.GetCurrentSubstitute();

            adjustedTarget.x = subChair.transform.position.x;

            startVector = subChair.transform.position;

            finalTarget = adjustedTarget;
            finalTarget.x = subChair.transform.position.x + subChair.GetMaxStretch().x;
        } else {
            adjustedTarget = (Vector3)target - chosenAthlete.transform.position; //Get the difference between the target and the athlete
        
            SetAthleteBeingDragged(chosenAthlete);
            chosenAthlete.MouseClick();
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
                //THIS IS ASSuming that there can only be one sub chair active at a time

                subChairsActive[0].AdjustPosition(stepToTarget);
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

                subChairsActive[0].AdjustPosition(stepToTarget);

                yield return waiter;
            }
        }

        if(usingSubstitute) {
            subChairsActive[0].ChairClicked();
        } else {
            chosenAthlete.StartAction();
        }
    }

    public IEnumerator ComputerSubstitution(SubstituteChair subChair) {

        yield return new WaitForSeconds(0.1f);

        subChair.ChairClicked();

    }

    private bool timeoutRequested = false;
    private bool timeoutAcceptable = false;

    public bool IsTimeoutAcceptable() {
        return timeoutAcceptable;
    }

    public void RequestTimeout() {
        timeoutRequested = true;

        StartCoroutine(EndTurn());
    }

    public void UseTimeout() {
        Debug.Log("Timeout used");

        /*
        Team team = GetTurnTeam();
        if(team == homeTeam) {
            matchData.homeTeamData.LoseTimeout();
        } else {
            matchData.awayTeamData.LoseTimeout();
        }
        */

        SetSide(true, homeTeam);
        SetSide(false, awayTeam);

        SetAthletesOnField();

        StartCoroutine(EndTurn());
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
     public void ReadyTeam(bool home) { //Will only be called in simultaneous mode
        if(home) {
            homeReady = true;
        } else {
            awayReady = true;
        }

        if(homeReady && awayReady) {
            if(!turnActive) {
                Fling();
            }
        } else {
            canvasManager.DisplayTeamReadyUp(home);
        }
    }
    */

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

        /*
        if(turnNumber >= turnCap) {
            audioManager.AmbientStop("lastTurn");
        }
        */

        SetAthleteBeingDragged(null);

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

            if(homeTurn) {
                if(homeSubAvailable) {
                    homeGoal.CloseSubPlatform();
                }
                homeGoal.SetCollidersEnabled(true);
            } else {
                if(awaySubAvailable) {
                    awayGoal.CloseSubPlatform();
                }
                awayGoal.SetCollidersEnabled(true);
            }
        }

        if(canvasManager != null) {
            canvasManager.DisplayTurnActive(initiatior.GetAthlete().GetTeam());
        }

        turnActive = true;

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

    //This is no longer a coroutine so I did a lil yieldy yieldy. All the coroutine stuff has been moved to pre-turn
    public IEnumerator EndTurn() {
        Debug.Log("Ending turn");

        yield return new WaitForFixedUpdate();

        timeoutAcceptable = false;

        bool tied = false;
        if(matchData.homeTeamData.GetScore() == matchData.awayTeamData.GetScore()) {
            tied = true;
        }
        
        if(!overtime) {
            if(turnNumber == turnCap && homeTurn) { //If it's the last turn and the homeTeam just went

                if(tied) {
                    overtime = true;

                    if(ruleSet.GetRule("drawResolution").value == 0) {
                        BeginGoldenGoal();
                    } else {
                        BeginOvertime();
                    }
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
        StartNextTurn();
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

		matchData.GetTeamMatchData(teamThatScored).IncreaseScore(1);

		if(canvasManager != null) {
        	canvasManager.StartCoroutine(canvasManager.DisplayScore(teamThatScored));
		}

        if(crowdController != null) {
            bool homeScored;
            if(teamThatScored == homeTeam) {
                homeScored = true;
                 crowdController.StartCoroutine(crowdController.FlashSteps(teamThatScored.primaryColor, "goalHomeLight", 3, 0.15f));
            } else {
                homeScored = false;
                crowdController.StartCoroutine(crowdController.FlashSteps(teamThatScored.primaryColor, "goalAwayLight", 3, 0.15f));
            }

            crowdController.ExpressEmotion("happy", homeScored);
            crowdController.ExpressEmotion("sad", !homeScored);
        }
    }

    public void AthleteGoal(AthleteController ac) {
        DisplayQuote(ac, ac.GetAthlete().GetQuote("goal"));
    }

    public void AthleteOwnGoal(AthleteController ac) {
        DisplayQuote(ac, ac.GetAthlete().GetQuote("ownGoal"));
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

                ball.SetScoredByTeam(null);

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
         
         /*
         for(int i = 0; i < homeGoals.Count; i++) {
            if(homeGoals[i].GetBallEntered()) {
                blockersMoved = true;
                homeGoals[i].blocker.RestoreBlock();
                homeGoals[i].SetBallEntered(false);
            }
        }

        for(int i = 0; i < awayGoals.Count; i++) {
            if(awayGoals[i].GetBallEntered()) {
                blockersMoved = true;
                awayGoals[i].blocker.RestoreBlock();
                awayGoals[i].SetBallEntered(false);
             }
        }
        */
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

        cameraController.AddTrauma(1f);

        int homeScore = matchData.GetTeamMatchData(homeTeam).GetScore();
        int awayScore = matchData.GetTeamMatchData(awayTeam).GetScore();

        Team winner = null;
        if(homeScore > awayScore) {
            winner = homeTeam;
            audioManager.Play("victory");
        } else if(awayScore > homeScore) {
            winner = awayTeam;
            audioManager.Play("defeat");
        } else {
            Debug.Log("Tie Game but this message should NEVER display, right?");
            audioManager.Play("overtime");
        }

        if(winner != null) {
            matchData.SetWinner(winner);
        }

        if(canvasManager != null) {
            canvasManager.DisplayEndMatch();
        }

        if(crowdController != null) {
            crowdController.StartCoroutine(crowdController.FlashSteps(Color.white, "matchOverLight", 2, 0.03f));
        }

        DeterminePostMatchFaceStates(winner);

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
}
