using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentBracketPanel : MonoBehaviour {
    
    public GameObject sixteenTeamSetup;
    public GameObject eightTeamSetup;
    public GameObject fourTeamSetup;

    private GameObject currentSetup;
    private Transform finalRoundMatchupHolder;
    private Transform semiFinalRoundMatchupHolder;
    private Transform quarterFinalRoundMatchupHolder;
    private Transform eighthFinalRoundMatchupHolder;

    public CustomButton advanceButton;

    public void SetTournamentBracket(List<MatchData> matchList) { //assumes teams are in order
        sixteenTeamSetup.SetActive(false);
        eightTeamSetup.SetActive(false);
        fourTeamSetup.SetActive(false);

        if(matchList.Count == 8) {
            Debug.Log("Setting for 16 teams");
            currentSetup = sixteenTeamSetup;
        } else if(matchList.Count == 4) {
            Debug.Log("Setting for 8 teams");
            currentSetup = eightTeamSetup;
        } else if(matchList.Count == 2) {
            Debug.Log("Setting for 4 teams");
            currentSetup = fourTeamSetup;
        } else {
            Debug.Log("Inappropriate number of tournament teams at " + (matchList.Count * 2));
        }

        currentSetup.SetActive(true);

        finalRoundMatchupHolder = currentSetup.transform.GetChild(currentSetup.transform.childCount - 1);
        semiFinalRoundMatchupHolder = currentSetup.transform.GetChild(currentSetup.transform.childCount - 2);
        if(currentSetup.transform.childCount > 3) {
            quarterFinalRoundMatchupHolder = currentSetup.transform.GetChild(currentSetup.transform.childCount - 3);
        }
        if(currentSetup.transform.childCount > 4) {
            eighthFinalRoundMatchupHolder = currentSetup.transform.GetChild(currentSetup.transform.childCount - 4);
        }

        SetNextRound(matchList);
    }

    public void SetNextRound(List<MatchData> matchList) {
        advanceButton.EnableButton();

        if(matchList.Count == 8) {
            for(int i = 0; i < eighthFinalRoundMatchupHolder.childCount; i++) {
                eighthFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>().SetMatchupBox(matchList[i]);
            }
        } else if(matchList.Count == 4) {
            for(int i = 0; i < quarterFinalRoundMatchupHolder.childCount; i++) {
                quarterFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>().SetMatchupBox(matchList[i]);
            }
        } else if(matchList.Count == 2) {
            for(int i = 0; i < semiFinalRoundMatchupHolder.childCount; i++) {
                semiFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>().SetMatchupBox(matchList[i]);
            }
        } else if(matchList.Count == 1) {
            finalRoundMatchupHolder.GetChild(0).GetComponent<MatchupBox>().SetMatchupBox(matchList[0]);
        } else {
            Debug.Log("Inappropriate number of tournament teams at " + (matchList.Count * 2));
        }
    }

    public void UpdateTournamentBracket() {
        if(eighthFinalRoundMatchupHolder != null) {
            for(int i = 0; i < eighthFinalRoundMatchupHolder.childCount; i++) {
                MatchupBox emb = eighthFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>();
                emb.UpdateMatchupBox();
            }
        } 
        
        if(quarterFinalRoundMatchupHolder != null) {
            for(int i = 0; i < quarterFinalRoundMatchupHolder.childCount; i++) {
                MatchupBox qmb = quarterFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>();
                qmb.UpdateMatchupBox();
            }
        }

        for(int i = 0; i < semiFinalRoundMatchupHolder.childCount; i++) {
            MatchupBox smb = semiFinalRoundMatchupHolder.GetChild(i).GetComponent<MatchupBox>();
            smb.UpdateMatchupBox();
        }

        MatchupBox fmb = finalRoundMatchupHolder.GetChild(0).GetComponent<MatchupBox>();
        fmb.UpdateMatchupBox();

        //If we get to this point then the tournament is over
    }

    public void AdvanceRound() {
        advanceButton.DisableButton();
    }

}
