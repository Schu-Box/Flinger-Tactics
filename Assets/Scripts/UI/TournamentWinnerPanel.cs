using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TournamentWinnerPanel : MonoBehaviour {
    public GameObject showcaseAthleteUI;
	public Transform showcaseAthleteHolder;
	public TextMeshProUGUI teamNameText;

    private List<ShowcaseAthleteUI> showcaseAthleteList = new List<ShowcaseAthleteUI>();

    public void SetPanel(Team team) {
        teamNameText.text = team.name;
		teamNameText.color = team.primaryColor;

        Athlete mvp = team.GetCurrentMatchData().GetMVP();

		for(int i = showcaseAthleteHolder.childCount - 1; i > -1;  i--) {
			Destroy(showcaseAthleteHolder.GetChild(i).gameObject);
		}

		showcaseAthleteList = new List<ShowcaseAthleteUI>();
		for(int i = 0; i < team.athletes.Count; i++) {

			ShowcaseAthleteUI show = Instantiate(showcaseAthleteUI, Vector3.zero, Quaternion.identity, showcaseAthleteHolder).GetComponent<ShowcaseAthleteUI>();
			show.SetAthlete(team.athletes[i], team.careerTeamMatchData.GetAthleteMatchData(team.athletes[i]));
			if(team.athletes[i] == mvp) {
				show.haloText.gameObject.SetActive(true);
				show.GetComponent<AthleteImage>().SetMVP();
			} else {
				show.haloText.gameObject.SetActive(false);
			}
			
			showcaseAthleteList.Add(show);
		}
    }
}
