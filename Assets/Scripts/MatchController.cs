using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour {

	public GameObject ballPrefab;
    public GameObject athleteStandardPrefab;

    public int numAthletesPerTeam = 3;
    public int numBalls;
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

    public SpawnBox ballSpawnBox;
    public SpawnBox homeSpawnBox;
    public SpawnBox awaySpawnBox;

	private List<AthleteController> homeAthletesOnField = new List<AthleteController>();
    private List<AthleteController> awayAthletesOnField = new List<AthleteController>();
    private List<AthleteController> allAthletesOnField = new List<AthleteController>();
    private List<BallController> ballsOnField = new List<BallController>();

    private List<Bumper> bumperTriggersLastEntered = new List<Bumper>();
    
    //Ready stuff is now unused
    private bool homeReady = false;
    private bool awayReady = false;

	private bool unbounded = false;

    private bool matchStarted = false;
    private bool matchEnded = false;
    private int turnNumber = 1;
    private bool homeTurn = false;
    private bool athleteBeingDragged = false;
    private bool turnActive = false;

    private AthleteController athleteInitiater;

    public static bool blockersMoved = false;
	public static bool simultaneousTurns = true;

    public static bool muted = false;

	private CanvasManager canvasManager;
    private CameraController cameraController;
	private AudioManager audioManager;

    private MatchData matchData;

	void Awake() {
        Physics2D.IgnoreLayerCollision(8, 9, true); //Makes the balls ignore the goals

        simultaneousTurns = false;
        matchStarted = false;
        turnActive = false;
        matchEnded = false;

		canvasManager = FindObjectOfType<CanvasManager>();
        cameraController = FindObjectOfType<CameraController>();
		audioManager = FindObjectOfType<AudioManager>();
	}

    void Update () {
        if (Input.GetMouseButtonDown(0)) {
            CastRay();
        }       
    }

    void CastRay() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        for(int i = 0; i < hits.Length; i++) {
            Debug.Log(hits[i].collider.gameObject.name);
            if (hits[i].collider.gameObject.GetComponent<TailTip>() != null) { //If it's the tail being clicked
                hits[i].collider.gameObject.GetComponentInParent<AthleteController>().Clicked();
            }
        }
    }

    public void PretendMouseJustAppeared() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

        AthleteController ac = null;
        bool tailHovered = false;
        for(int i = 0; i < hits.Length; i++) {
            if (hits[i].collider.gameObject.GetComponent<TailTip>() != null) { //If the tail is being hovered
                ac = hits[i].collider.gameObject.GetComponentInParent<AthleteController>();
                tailHovered = true;
                break;
            } else if(hits[i].collider.gameObject.GetComponent<AthleteController>() != null) {
                ac = hits[i].collider.gameObject.GetComponent<AthleteController>();
            }
        }

        if(ac != null) {
            if(tailHovered) {
                ac.MouseEnterTail();
            } else {
                ac.MouseEnterBody();
            }
        }
    }

	public void SetupCourt() {
		homeSpawnBox.SetSpawnBox(numAthletesPerTeam);
        awaySpawnBox.SetSpawnBox(numAthletesPerTeam);
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

            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                Destroy(homeAthletesOnField[i].gameObject);
            }
            homeAthletesOnField = new List<AthleteController>();


        } else {
            awayTeam = teamSelected;

            awayGoal.GetComponent<Bumper>().SetColor(teamSelected.primaryColor);
            for(int i = 0; i < awayBumpers.Count; i++) {
                awayBumpers[i].SetColor(teamSelected.GetDarkTint());
            }

            awaySideColor.color = teamSelected.GetLightTint();

            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                Destroy(awayAthletesOnField[i].gameObject);
            }
            awayAthletesOnField = new List<AthleteController>();
        }

        SpawnAthletes(homeSide);

        homeSpawnBox.Disable();
        awaySpawnBox.Disable();
    }

	public void SpawnAthletes(bool homeSide) {

        for(int i = 0; i < numAthletesPerTeam; i++) {
            Athlete athlete;
            
            Vector3 spawnSpot;
            Vector3 spawnAngle;
            if(homeSide) {
                athlete = homeTeam.athletes[i];

                spawnSpot = new Vector3(homeSpawnBox.transform.localPosition.x, homeSpawnBox.GetSpawnHeights()[i], 0);
                spawnAngle = new Vector3(0, 0, 90);
            } else {
                athlete = awayTeam.athletes[i];

                spawnSpot = new Vector3(awaySpawnBox.transform.localPosition.x, awaySpawnBox.GetSpawnHeights()[i], 0);
                spawnAngle = new Vector3(0, 0, 270);
            }

            GameObject prefabToSpawn = athleteStandardPrefab;

            GameObject newAthleteObj = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(spawnAngle), athleteHolder);
			newAthleteObj.transform.localPosition = spawnSpot;
            AthleteController ac = newAthleteObj.GetComponent<AthleteController>();
            ac.SetAthlete(athlete);

            if(homeSide) {
                homeAthletesOnField.Add(ac);
            }  else {
                awayAthletesOnField.Add(ac);
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

    public void AthleteHovered(Athlete athlete) {
        if(canvasManager != null && !athleteBeingDragged) {
            canvasManager.DisplayFootnotePanel(athlete);
        }
    }

    public void AthleteUnhovered(Athlete athlete) {
        if(canvasManager != null && !athleteBeingDragged) {
            canvasManager.HideFootnotePanel(athlete);
        }
    }

    public void SetAthleteBeingDragged(bool beingDragged) {
        athleteBeingDragged = beingDragged;
    }

    public bool GetAthleteBeingDragged() {
        return athleteBeingDragged;
    }

	public void SetUnbounded(bool isUnboundedByTurns) {
		unbounded = isUnboundedByTurns;
	}

	public void StartMatch() {
        matchStarted = true;

        audioManager.PlaySound("whistle");

        matchData = new MatchData(homeTeam, awayTeam);
        homeTeam.AssignNewMatchData(matchData);
        awayTeam.AssignNewMatchData(matchData);

		if(canvasManager != null) {
        	canvasManager.DisplayStartMatch();
		}

        allAthletesOnField = new List<AthleteController>();
        for(int i = 0; i < homeAthletesOnField.Count; i++) { //Assumes home and away team always have the same number
            allAthletesOnField.Add(homeAthletesOnField[i]);
            allAthletesOnField.Add(awayAthletesOnField[i]);
        }


        //Field Mechanics
        ballSpawnBox.SetSpawnBox(numBalls);

        for(int i = 0; i < numBalls; i++) {
            Vector3 ballPosition = new Vector3(0, ballSpawnBox.GetSpawnHeights()[i], 0);
            BallController newBall = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, ballHolder).GetComponent<BallController>();
			newBall.transform.localPosition = ballPosition;
            ballsOnField.Add(newBall);
        }
        ballSpawnBox.Disable();
        

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

        //Kind of stupid but this essentially subtracts a turn
        homeTurn = !homeTurn; 
        turnNumber--;
        StartNextTurn();
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

	public void StartNextTurn() {
        turnActive = false;
        
        turnNumber++;
        homeTurn = !homeTurn;

        athleteInitiater = null;

		if(canvasManager != null) {
        	canvasManager.DisplayNextTurn(turnNumber);
		}

        homeReady = false;
        awayReady = false;

        //I shouldn't be calling this every turn. Only when they've actually been scored
        RespawnBalls();

        for(int i = 0; i < ballsOnField.Count; i++) {
            ballsOnField[i].ResetTouchOrder();
        }

		if(!unbounded) {
            if(homeTurn) {
                for(int i = 0; i < homeAthletesOnField.Count; i++) {
                    homeAthletesOnField[i].EnableInteraction();
                    homeAthletesOnField[i].IgnoreRaycasts(false);
                    homeAthletesOnField[i].DisableBody();
                }
                for(int i = 0; i < awayAthletesOnField.Count; i++) {
                    awayAthletesOnField[i].DisableInteraction();
                    awayAthletesOnField[i].IgnoreRaycasts(true);
                    awayAthletesOnField[i].DimAthleteColor();
                    awayAthletesOnField[i].EnableBody();
                }
            } else {
                for(int i = 0; i < homeAthletesOnField.Count; i++) {
                    homeAthletesOnField[i].DisableInteraction();
                    homeAthletesOnField[i].IgnoreRaycasts(true);
                    homeAthletesOnField[i].DimAthleteColor();
                    homeAthletesOnField[i].EnableBody();
                }
                for(int i = 0; i < awayAthletesOnField.Count; i++) {
                    awayAthletesOnField[i].EnableInteraction();
                    awayAthletesOnField[i].IgnoreRaycasts(false);
                    awayAthletesOnField[i].DisableBody();
                }
            }
		}

        PretendMouseJustAppeared();
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
    
    public void Fling(AthleteController flungAthlete) { //Turn is now active
		if(!unbounded) {
            for(int i = 0; i < allAthletesOnField.Count; i++) {
                allAthletesOnField[i].DisableInteraction();
                allAthletesOnField[i].EnableBody();
                allAthletesOnField[i].RestoreAthleteColor();
            }
        }

        if(canvasManager != null) {
            canvasManager.DisplayTurnActive(flungAthlete.GetAthlete().GetTeam());
        }

        turnActive = true;

        StartCoroutine(WaitForTurnEnd());
    }

    public bool IsTurnActive() {
        return turnActive;
    }

    public void SetAthleteInitiater(AthleteController ac) {
        athleteInitiater = ac;
    }

    public AthleteController GetAthleteInitiater() {
        return athleteInitiater;
    }

    public IEnumerator WaitForTurnEnd() {

        yield return new WaitForSeconds(1f);

        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        bool turnComplete = false;
        bool allAthletesStopped = false;
        bool allBallsStopped = false;
        while(!turnComplete) {

            bool athleteStillMoving = false;
            for(int i = 0; i < allAthletesOnField.Count; i++) {
                if(allAthletesOnField[i].GetMoving()) {
                    athleteStillMoving = true;
                    break;
                }
            }
            if(!athleteStillMoving) {
                allAthletesStopped = true;
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
            if(!ballStillMoving) {
                allBallsStopped = true;
            }

            turnComplete = (allAthletesStopped && allBallsStopped);

            yield return waiter;
        }

        EndTurn();
    }

     public void EndTurn() {
        blockersMoved = false;

        ResetBumpers();

        if(turnNumber >= turnCap) {
            EndMatch();
        }
        
        if(!matchEnded) {
            if(blockersMoved) {
                StartCoroutine(WaitForTurnEnd());
            } else {
                StartNextTurn();
            }
        }
    }

	public void ScoreGoal(Team teamThatScored, BallController ball, Vector3 goalCenter) {
        //Debug.Log(teamThatScored.name + " scored a goal!");
        cameraController.AddTrauma(1f);
		audioManager.PlaySound("goal");

        if(ball.GetLastBumper() == null) {
            Debug.Log("Ball didn't enter a bumper zone");
        } else {
            bumperTriggersLastEntered.Add(ball.GetLastBumper());
        }


        ball.ScoreBall(teamThatScored, athleteInitiater, goalCenter);

		teamThatScored.score++;

		if(canvasManager != null) {
        	StartCoroutine(canvasManager.DisplayScore(teamThatScored));
		}
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
                        if(ballsOnField[b].transform.position == chosenSpawn) {
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
            }
        }

        ballSpawnBox.Disable();
    }

     public void ResetBumpers() {
         for(int i = 0; i < bumperTriggersLastEntered.Count; i++) {
             //Can lead to an error
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

	public void EndMatch() {
    	matchEnded = true;
        
        canvasManager.DisplayEndMatch();

        audioManager.PlaySound("whistle");

        if(homeTeam.score > awayTeam.score) {
            homeTeam.wonTheGame = true;
        } else if(awayTeam.score > homeTeam.score) {
            awayTeam.wonTheGame = true;
        } else {
            Debug.Log("Tie Game");
        }

        for(int i = 0; i < allAthletesOnField.Count; i++) {
           allAthletesOnField[i].FinishMatch();
        }
    }

    public List<Athlete> GetTopThreePerformers() {
        List<Athlete> besties = new List<Athlete>();

        List<Athlete> possibleAthletes = new List<Athlete>();
        for(int i = 0; i < allAthletesOnField.Count; i++) {
            possibleAthletes.Add(allAthletesOnField[i].GetAthlete());
        }

        for(int i = 0; i < 3; i++) {
            Athlete bestAthlete = null;
            int highestValue = 0;
            for(int j = 0; j < possibleAthletes.Count; j++) {
                Athlete a = possibleAthletes[j];
                int totalPointValue = a.GetStatPointTotal();

                if(totalPointValue > highestValue) {
                    highestValue = totalPointValue;
                    bestAthlete = a;
                }
            }

            besties.Add(bestAthlete);
            possibleAthletes.Remove(bestAthlete);
        }

        return besties;
    }

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

    public void UpdateStats(StatType type, Athlete athlete) {
        if(canvasManager != null) {
            canvasManager.UpdateFootnotePanel(type, athlete);
        }
    }
}
