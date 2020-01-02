using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MatchupBox : MonoBehaviour {

    private MatchData match;
    private Team topTeam;
    private Team bottomTeam;
   
    public Image topNameBox;
    public TextMeshProUGUI topNameText;
    public Image bottomNameBox;
    public TextMeshProUGUI bottomNameText;

    public Image topScoreBox;
    public TextMeshProUGUI topScoreText;
    public Image bottomScoreBox;
    public TextMeshProUGUI bottomScoreText;

    public Image borderImage;

    public void SetMatchupBox(MatchData matchData) {
        match = matchData;
        topTeam = match.homeTeamData.team;
        bottomTeam = match.awayTeamData.team;

        topNameBox.color = topTeam.primaryColor;
        topScoreBox.color = topTeam.GetDarkTint();

        bottomNameBox.color = bottomTeam.primaryColor;
        bottomScoreBox.color = bottomTeam.GetDarkTint();

        topNameText.text = topTeam.nameLocation;
        topNameText.color = topTeam.secondaryColor;
        topScoreText.text = "";

        bottomNameText.text = bottomTeam.nameLocation;
        bottomNameText.color = bottomTeam.secondaryColor;
        bottomScoreText.text = "";

        if(!matchData.homeTeamData.team.computerControlled || !matchData.awayTeamData.team.computerControlled) {
            borderImage.gameObject.SetActive(true);
        } else {
            borderImage.gameObject.SetActive(false);
        }
    }

    public void UpdateMatchupBox() {
        if(match == null) {
            topScoreText.text = "";
            bottomScoreText.text = "";
            topNameText.text = "";
            bottomNameText.text = "";
        } else {
            if(match.GetWinner() != null) {
                topScoreText.text = match.homeTeamData.GetScore().ToString();
                bottomScoreText.text = match.awayTeamData.GetScore().ToString();
            }
        }
    }

    public bool IsMatchSet() {
        if(match != null) {
            return true;
        } else {
            return false;
        }
    }
}
