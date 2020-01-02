using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentTeamSelectPanel : MonoBehaviour {

    private ModeController modeController;
    private Team currentTeam;
   
   public TextMeshProUGUI teamNameText;
   public GameObject rosterAthletePrefab;
   public Transform rosterAthleteHolder;

   public Image leftArrow;
   public Image rightArrow;

   public Image computerToggleBackground;
   public Image computerToggleBorder;
   public TextMeshProUGUI computerToggleText;
   
   private void Start() {
       modeController = FindObjectOfType<ModeController>();
   }

   public void SetForTeam(Team team) {
        currentTeam = team;

        teamNameText.text = team.name;
        teamNameText.color = team.primaryColor;

        leftArrow.color = team.secondaryColor;
        rightArrow.color = team.secondaryColor;

        for(int i = 0; i < rosterAthleteHolder.childCount; i++) {
            if(team.athletes.Count > i) {
                rosterAthleteHolder.GetChild(i).gameObject.SetActive(true);

                Athlete athlete = team.athletes[i];

                AthleteImage athleteImage = rosterAthleteHolder.GetChild(i).GetComponent<AthleteImage>();

                athleteImage.SetImages(athlete);

                athleteImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = athlete.name;
            } else {
                rosterAthleteHolder.GetChild(i).gameObject.SetActive(false);
            }
        }

        DetermineComputerControl();
   }

   public void ToggleComputerControl() {
       currentTeam.computerControlled = !currentTeam.computerControlled;

        DetermineComputerControl();
   }

    public void DetermineComputerControl() {
       if(currentTeam.computerControlled) {
            computerToggleBackground.color = Color.grey;
            computerToggleBorder.color = Color.grey;
            computerToggleText.text = "Computer Controlled";
        } else {
            computerToggleBackground.color = currentTeam.primaryColor;
            computerToggleBorder.color = currentTeam.secondaryColor;
            computerToggleText.text = "Player Controlled";
        }
   }

   public void CycleTeam(bool left) {
        int teamInt = 1;
        for(int i = 0; i < modeController.teamList.Count; i++) {
            if(modeController.teamList[i] == currentTeam) {
                teamInt = i;
                break;
            }
        }

        if(left) {
            teamInt--;
        } else {
            teamInt++;
        }

        int teamCount = modeController.teamList.Count;

        teamInt = (teamInt % teamCount + teamCount) % teamCount; //Super stupid way of getting the actual modulus for when teamInt goes negative

        Team newTeam = modeController.teamList[teamInt];

        SetForTeam(newTeam);
    }

    public Team GetCurrentTeam() {
        return currentTeam;
    }
}
