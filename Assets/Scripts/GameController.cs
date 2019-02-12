using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour {
    public List<Team> teamList = new List<Team>();

    private AudioManager audioManager;
    private CameraController cameraController;

    public GameObject ballPrefab;
    public GameObject athleteStandardPrefab;
    public GameObject athleteLongchuckPrefab;
    public GameObject athleteGrodwagPrefab;

    public int numAthletesPerTeam;
    public int numBalls;

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

    //public Transform scoreHolders;

    [Header("Post Match UI")]
    public GameObject postMatchPanel;
    public TextMeshProUGUI homeFinalScore;
    public TextMeshProUGUI awayFinalScore;

    public TeamPostMatchPanel teamPostMatchPanel;

    [Header("Objects")]
    public Team homeTeam = null;
    public Team awayTeam = null;

    public TextMeshProUGUI homeFieldText;
    public TextMeshProUGUI awayFieldText;

    public SpriteRenderer homeSideColor;
    public SpriteRenderer awaySideColor;

    public List<GoalController> homeGoals;
    public List<GoalController> awayGoals;
    public List<BlockerController> homeBlockers;
    public List<BlockerController> awayBlockers;

    public SpawnBox ballSpawnBox;
    public SpawnBox homeSpawnBox;
    public SpawnBox awaySpawnBox;

    public List<AudioClip> sounds_Bwomp = new List<AudioClip>();

    private List<AthleteController> homeAthletesOnField = new List<AthleteController>();
    private List<AthleteController> awayAthletesOnField = new List<AthleteController>();
    private List<AthleteController> allAthletesOnField = new List<AthleteController>();
    private List<BallController> ballsOnField = new List<BallController>();

   
    [Header("Other?")]
    public int turnCap;
    public float trauma = 0f;
    private Coroutine screenShake;
    
    private int turnNumber = 1;
    private bool homeStarts = false;
    private bool homeReady = false;
    private bool awayReady = false;
    private bool muted = false;
    //Should be private
    public bool turnActive = false;
    public bool matchStarted = false;
    public bool matchOver = false;

    public static bool blockersMoved = false;

    void Start() {
        audioManager = FindObjectOfType<AudioManager>();
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraController.startPosition;

        ToggleMute();

        postMatchPanel.SetActive(false);

        homeSpawnBox.SetSpawnBox(numAthletesPerTeam);
        awaySpawnBox.SetSpawnBox(numAthletesPerTeam);

        SetNewRosters();

        TeamSelectionPhase();
    }

    void Update() {
        if(Input.GetButtonDown("Submit")) {
            if(!homeReady) {
                ReadyTeam(true);
            }
            if(!awayReady) {
                ReadyTeam(false);
            }
        } else if(Input.GetButtonDown("Cancel")) {
            SceneManager.LoadScene(0);
        } else if(Input.GetKeyDown(KeyCode.E)) {
            EndMatch();
        } else if(Input.GetKeyDown(KeyCode.M)) {
            ToggleMute();
        }
    }

    public void ToggleMute() {
        if(!muted) {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < audioSources.Length; i++) {
                audioSources[i].mute = true;
            }

            muted = true;
        } else {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            for(int i = 0; i < audioSources.Length; i++) {
                audioSources[i].mute = false;
            }

            muted = false;
        }
    }

    public void SetNewRosters() {
        for(int i = 0; i < teamList.Count; i++) {
            teamList[i].SetNewRoster(numAthletesPerTeam);
        }
    }

    public void TeamSelectionPhase() {
        homeReadyButton.gameObject.SetActive(false);
        awayReadyButton.gameObject.SetActive(false);

        SetSide(true, teamList[0]);
        SetSide(false, teamList[1]);

        turnLabelText.text = "";
        turnCapText.text = "";
        homeScoreText.text = "";
        awayScoreText.text = "";

        turnButton.PreMatch();
    }

    public void CycleTeamSelected(bool homeSide, bool cycleDown) {
        Team previousTeam;
        Team opponentTeam;
        if(homeSide) {
            previousTeam = homeTeam;
            opponentTeam = awayTeam;
        } else {
            previousTeam = awayTeam;
            opponentTeam = homeTeam;
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

        SetSide(homeSide, teamList[nextNum]);
    }

    public void SetSide(bool homeSide, Team teamSelected) {
        //Spawn Athletes on the proper side
        Color darkTint = Color.Lerp(teamSelected.color, Color.black, 0.2f);
        Color lightTint = Color.Lerp(teamSelected.color, Color.white, 0.7f);

        if(homeSide) {
            homeTeam = teamSelected;

            homeSelectionPanel.SetTeam(teamSelected);

            for(int i = 0; i < homeGoals.Count; i++) { //Assumes goals and blockers are always equal in count
                homeGoals[i].GetComponent<SpriteRenderer>().color = teamSelected.color;
                homeBlockers[i].GetComponentInChildren<SpriteRenderer>().color = darkTint;
            }

            homeFieldText.text = teamSelected.fieldString;
            homeSideColor.color = lightTint;

            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                Destroy(homeAthletesOnField[i].gameObject);
            }
            homeAthletesOnField = new List<AthleteController>();
        } else {
            awayTeam = teamSelected;

            awaySelectionPanel.SetTeam(teamSelected);

            for(int i = 0; i < awayGoals.Count; i++) {
                awayGoals[i].GetComponent<SpriteRenderer>().color = teamSelected.color;
                awayBlockers[i].GetComponentInChildren<SpriteRenderer>().color = darkTint;
            }

            awayFieldText.text = teamSelected.fieldString;
            awaySideColor.color = lightTint;

            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                Destroy(awayAthletesOnField[i].gameObject);
            }
            awayAthletesOnField = new List<AthleteController>();
        }

        SpawnAthletes(homeSide);
    }

    public void SpawnAthletes(bool homeSide) {

        for(int i = 0; i < numAthletesPerTeam; i++) {
            Athlete athlete;
            
            Vector3 spawnSpot;
            Vector3 spawnAngle;
            if(homeSide) {
                athlete = homeTeam.athletes[i];

                spawnSpot = new Vector3(homeSpawnBox.transform.localPosition.x, homeSpawnBox.GetSpawnHeights()[i], 0);
                spawnAngle = new Vector3(0, 0, 270);
            } else {
                athlete = awayTeam.athletes[i];

                spawnSpot = new Vector3(awaySpawnBox.transform.localPosition.x, awaySpawnBox.GetSpawnHeights()[i], 0);
                spawnAngle = new Vector3(0, 0, 90);
            }

            GameObject prefabToSpawn;
            if(athlete.athleteType == AthleteType.Longchuck) {
                prefabToSpawn = athleteLongchuckPrefab;
            } else if(athlete.athleteType == AthleteType.Grodwag) {
                prefabToSpawn = athleteGrodwagPrefab;
            } else {
                prefabToSpawn = athleteStandardPrefab;
            }

            GameObject newAthleteObj = Instantiate(prefabToSpawn, spawnSpot, Quaternion.Euler(spawnAngle));
            AthleteController ac = newAthleteObj.GetComponent<AthleteController>();
            ac.SetAthlete(athlete);

            if(homeSide) {
                homeAthletesOnField.Add(ac);
            }  else {
                awayAthletesOnField.Add(ac);
            }
        }
    }


    public void StartMatch() {
        matchStarted = true;

        //UI
        turnLabelText.text = "Turn";
        turnButton.DuringMatch();
        turnCapText.text = "of " + turnCap.ToString();

        homeScoreText.text = homeTeam.score.ToString();
        awayScoreText.text = awayTeam.score.ToString();

        homeScoreText.color = homeTeam.color;
        awayScoreText.color = awayTeam.color;

        homeSelectionPanel.gameObject.SetActive(false);
        awaySelectionPanel.gameObject.SetActive(false);

        homeReadyButton.SetForTeam(homeTeam);
        awayReadyButton.SetForTeam(awayTeam);


        allAthletesOnField = new List<AthleteController>();
        for(int i = 0; i < homeAthletesOnField.Count; i++) {
            allAthletesOnField.Add(homeAthletesOnField[i]);
        }
        for(int i = 0; i < awayAthletesOnField.Count; i++) {
            allAthletesOnField.Add(awayAthletesOnField[i]);
        }


        //Field Mechanics
        ballSpawnBox.SetSpawnBox(numBalls);

        for(int i = 0; i < numBalls; i++) {
            Vector3 ballPosition = new Vector3(0, ballSpawnBox.GetSpawnHeights()[i], 0);
            BallController newBall = Instantiate(ballPrefab, ballPosition, Quaternion.identity, ballSpawnBox.transform).GetComponent<BallController>();
            ballsOnField.Add(newBall);
        }

        ballSpawnBox.Disable();
        homeSpawnBox.Disable();
        awaySpawnBox.Disable();

        for(int i = 0; i < homeGoals.Count; i++) {
            homeGoals[i].goalAttacker = awayTeam;
            homeGoals[i].blocker = homeBlockers[i];
            homeBlockers[i].SetTeam(homeTeam);
        }
        for(int i = 0; i < awayGoals.Count; i++) {
            awayGoals[i].goalAttacker = homeTeam;
            awayGoals[i].blocker = awayBlockers[i];
            awayBlockers[i].SetTeam(awayTeam);
        }

        //Kind of stupid but this essentially subtracts a turn
        homeStarts = !homeStarts; 
        turnNumber--;
        StartNextTurn();
    }


    public void ScoreGoal(Team teamThatScored, BallController ball) {
        //Debug.Log(teamThatScored.name + " scored a goal!");
        audioManager.PlayGoalSound(teamThatScored == homeTeam);

        ball.ScoreBall(teamThatScored);
        
        teamThatScored.score++;

        homeScoreText.text = homeTeam.score.ToString();
        awayScoreText.text = awayTeam.score.ToString();
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

    public void StartNextTurn() {
        turnActive = false;
        
        turnNumber++;
        homeStarts = !homeStarts;

        turnButton.SetTurnCounter(turnNumber);

        homeReady = false;
        awayReady = false;

        SetReadyUpButtons();

        for(int i = 0; i < homeGoals.Count; i++) {
            homeGoals[i].SetTriggerState(false);
        }
        for(int i = 0; i < awayGoals.Count; i++) {
            awayGoals[i].SetTriggerState(false);
        }

        RespawnBalls();

        GreyOutAthletes(!homeStarts);
    }

    public void SetReadyUpButtons() {
        homeReadyButton.MyTurnNow(homeStarts);
        awayReadyButton.MyTurnNow(!homeStarts);
    }

    public void GreyOutAthletes(bool homeTeam) {
        if(homeTeam) {
            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                homeAthletesOnField[i].GreyOutAthlete();
            }
        } else {
            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                awayAthletesOnField[i].GreyOutAthlete();
            }
        }
    }

    public void RestoreAthleteColor(bool homeTeam) {
        if(homeTeam) {
            for(int i = 0; i < homeAthletesOnField.Count; i++) {
                homeAthletesOnField[i].RestoreAthleteColor();
            }
        } else {
            for(int i = 0; i < awayAthletesOnField.Count; i++) {
                awayAthletesOnField[i].RestoreAthleteColor();
            }
        }
    }

     public void ReadyTeam(bool home) {
        if(home) {
            homeReady = true;
        } else {
            awayReady = true;
        }

        if(homeReady && awayReady) {
            if(!turnActive) {
                BothReady();
            }
        } else {
            if(home) {
                awayReadyButton.MyTurnNow(true);
                homeReadyButton.MyTurnNow(false);

                GreyOutAthletes(true);
                RestoreAthleteColor(false);
            } else {
                awayReadyButton.MyTurnNow(false);
                homeReadyButton.MyTurnNow(true);

                GreyOutAthletes(false);
                RestoreAthleteColor(true);
            }
        }
    }
    
    public void BothReady() {
        RestoreAthleteColor(true);
        RestoreAthleteColor(false);

        turnActive = true;

        StartCoroutine(WaitForTurnEnd());

        for(int i = 0; i < allAthletesOnField.Count; i++) {
            allAthletesOnField[i].StartAction();
        }

        for(int i = 0; i < homeGoals.Count; i++) {
            homeGoals[i].SetTriggerState(true);
        }
        for(int i = 0; i < awayGoals.Count; i++) {
            awayGoals[i].SetTriggerState(true);
        }
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
                    if(ballsOnField[i].GetMoving()) {
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

        ResetBlockers();

        if(turnNumber >= turnCap) {
            EndMatch();
        }
        
        if(!matchOver) {
            if(blockersMoved) {
                StartCoroutine(WaitForTurnEnd());
            } else {
                StartNextTurn();
            }
        }
    }

    public void RespawnBalls() {
        ballSpawnBox.Enable();

        for(int i = ballsOnField.Count - 1; i >= 0; i--) {
           BallController ball = ballsOnField[i];

           ball.ResetTouchOrder();

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



     public void ResetBlockers() {
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
    }

    

    /*
    public void RemoveBall(BallController ball, bool home) {
        Debug.Log("Removing ball");

        ball.GetComponent<Rigidbody2D>().simulated = false;

        StartCoroutine(ball.GrowBall(new Vector3(1, 1, 1)));

        ballsOnField.Remove(ball);

        Destroy(ball);

        if(ballsOnField.Count == 0) {
            Debug.Log("MATCH OVER");
            
            EndMatch();
        }
    }
    */

    public void AddTrauma(float traumaAdded) {
        trauma += traumaAdded;
        trauma = Mathf.Clamp01(trauma); //Clamps the trauma value between 0 and 1

        if(screenShake == null) {
            screenShake = StartCoroutine(ShakeScreen());
        }
    }

    public IEnumerator ShakeScreen() {
        //Debug.Log("Shaking Screen");

        Vector3 originalPosition = Camera.main.transform.localPosition;

        float decayRate = 0.02f;

        while(trauma > 0) {
            float shake = Mathf.Pow(trauma, 2);

            float maxAngleOffset = 3f;
            float maxOffset = 0.1f;

            float angle = maxAngleOffset * shake * ((Random.value * 2) - 1f);
            float x = maxOffset * shake * ((Random.value * 2) - 1f);
            float y = maxOffset * shake * ((Random.value * 2) - 1f);

            Camera.main.transform.localPosition = new Vector3(x, y, 0) + originalPosition;
            Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, angle);

            trauma -= decayRate;

            yield return new WaitForFixedUpdate();
        }
        //Debug.Log("Shake Complete");

        Camera.main.transform.localPosition = originalPosition;

        screenShake = null;
    }

    public void EndMatch() {
        matchOver = true;
        turnLabelText.text = "";
        turnCapText.text = "";

        turnButton.PostMatch();
        homeReadyButton.EndMatch(homeTeam.score - awayTeam.score);
        awayReadyButton.EndMatch(awayTeam.score - homeTeam.score);

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

    public void DisplayPostMatchPanel() {
        postMatchPanel.SetActive(true);
        homeFinalScore.text = homeTeam.score.ToString();
        awayFinalScore.text = awayTeam.score.ToString();
        homeFinalScore.color = homeTeam.color;
        awayFinalScore.color = awayTeam.color;

        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));
    }

    public void DisplayTeamPanelPostMatch(bool home) {
        if(home) {
            teamPostMatchPanel.SetTeamPostMatchPanel(true, homeTeam);

            StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upLeftPosition));
        } else {
            teamPostMatchPanel.SetTeamPostMatchPanel(false, awayTeam);

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
}
