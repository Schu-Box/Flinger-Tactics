using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamPostMatchPanel : MonoBehaviour {

	public GameObject athleteStatHolderPrefab;
	public GameObject statBoxPrefab;
	public GameObject athleteUIPrefab;

	public TextMeshProUGUI teamNameText;

	public GameObject athleteImageHolder;
	public GameObject statLabelHolder;
	public GameObject athleteStatHolder;

	public Vector3 homePosition;
	public Vector3 awayPosition;


	public void SetTeamPostMatchPanel(bool homeSide, Team team) {
		if(homeSide) {
			gameObject.transform.localPosition = homePosition;
		} else {
			gameObject.transform.localPosition = awayPosition;
		}

		teamNameText.text = team.name;
		teamNameText.color = team.primaryColor;

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
	}
}
