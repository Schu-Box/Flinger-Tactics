using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPostMatchPanel : MonoBehaviour {

	public TextMeshProUGUI teamNameText;

	public Vector3 homePosition;
	public Vector3 awayPosition;

	public List<ShowcaseAthleteUI> showcaseAthleteList = new List<ShowcaseAthleteUI>();
    public Button arrowButton_back;
	public Vector3 buttonPos_home;
	public Vector3 buttonPos_away;


	public void SetTeamPostMatchPanel(bool homeSide, Team team) {
		if(homeSide) {
			gameObject.transform.localPosition = homePosition;
			arrowButton_back.transform.rotation = Quaternion.Euler(0, 0, 180);
			arrowButton_back.transform.localPosition = buttonPos_home;
		} else {
			gameObject.transform.localPosition = awayPosition;
			arrowButton_back.transform.rotation = Quaternion.Euler(0, 0, 0);
			arrowButton_back.transform.localPosition = buttonPos_away;
		}
		arrowButton_back.GetComponent<Image>().color = team.primaryColor;

		teamNameText.text = team.name;
		teamNameText.color = team.primaryColor;

		Athlete mvp = team.GetCurrentMatchData().GetMVP();

		for(int i = 0; i < showcaseAthleteList.Count; i++) {
			showcaseAthleteList[i].SetAthlete(team.athletes[i]);
			if(team.athletes[i] == mvp) {
				showcaseAthleteList[i].GetComponent<AthleteImage>().SetMVP();
			}
		}

		/*
		for(int i = statLabelHolder.transform.childCount - 1; i > -1; i--) {
			Destroy(statLabelHolder.transform.GetChild(i).gameObject);
		}
		for(int i = 0; i < team.athletes[0].statList.Count; i++) {
			GameObject newLabel = Instantiate(statBoxPrefab, Vector3.zero, Quaternion.identity, statLabelHolder.transform);

			newLabel.GetComponentInChildren<TextMeshProUGUI>().text = team.athletes[0].statList[i].GetStatName();
		}

		for(int i = athleteStatHolder.transform.childCount - 1; i > -1; i--) {
			Destroy(athleteStatHolder.transform.GetChild(i).gameObject);
		}
		for(int i = 0; i < team.athletes.Count; i++) {
			GameObject newHolder = Instantiate(athleteStatHolderPrefab, Vector3.zero, Quaternion.identity, athleteStatHolder.transform);
			for(int j = 0; j < team.athletes[i].statList.Count; j++) {
				GameObject newStatNum = Instantiate(statBoxPrefab, Vector3.zero, Quaternion.identity, newHolder.transform);
				newStatNum.GetComponentInChildren<TextMeshProUGUI>().text = team.athletes[i].statList[j].GetCount().ToString();
			}
		}

		for(int i = athleteImageHolder.transform.childCount - 1; i > -1; i--) {
			Destroy(athleteImageHolder.transform.GetChild(i).gameObject);
		}
		for(int i = 0; i < team.athletes.Count; i++) {
			GameObject newImage = Instantiate(athleteUIPrefab, Vector3.zero, Quaternion.identity, athleteImageHolder.transform);
			newImage.GetComponent<AthleteImage>().SetImages(team.athletes[i]);
		}
		*/
	}
}
