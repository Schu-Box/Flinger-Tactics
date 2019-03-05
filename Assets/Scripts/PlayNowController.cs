using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//Play Now Controller
public class PlayNowController : MonoBehaviour {

    private CameraController cameraController;
    private CanvasManager canvasManager;
    private AudioManager audioManager;
    private MatchController matchController;

    public List<Team> teamList = new List<Team>();

    void Start() {
        cameraController = Camera.main.transform.GetComponent<CameraController>();
        Camera.main.transform.position = cameraController.startPosition;
        canvasManager = FindObjectOfType<CanvasManager>();
        audioManager = FindObjectOfType<AudioManager>();
        matchController = FindObjectOfType<MatchController>();

        //ToggleMute();

        //Debug.Log("Starting Play Now");

        matchController.SetupCourt();

        SetNewRosters();

        //Debug.Log("Selection Phase");

        TeamSelectionPhase();

        //Debug.Log("Start Complete");
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
            teamList[i].SetNewRoster(matchController.numAthletesPerTeam);
        }
    }

    public void TeamSelectionPhase() {
        canvasManager.DisplayTeamSelection();
       
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
}
