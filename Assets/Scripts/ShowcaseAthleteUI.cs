using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ShowcaseAthleteUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	//Name Plate Panel
	public GameObject namePlate;
	public TextMeshProUGUI nameText;

	//Highlights Panel
	public GameObject highlightsPanel;
	public TextMeshProUGUI stat1Num;
	public TextMeshProUGUI stat1Label;
	public TextMeshProUGUI stat2Num;
	public TextMeshProUGUI stat2Label;

	//Stat Panel
	public GameObject statPanel;

	private Vector3 highlightStartPos;
	private Vector3 highlightEndPos;
	private Vector3 statStartPos;
	private Vector3 statEndPos;

	private Athlete showcasedAthlete;

	public void SetAthlete(Athlete athlete) {
		showcasedAthlete = athlete;
		Team team = athlete.GetTeam();

		namePlate.GetComponent<Image>().color = team.primaryColor;
		highlightsPanel.GetComponent<Image>().color = team.GetLightTint();
		statPanel.GetComponent<Image>().color = team.primaryColor;

		nameText.text = athlete.name;

		GetComponent<AthleteImage>().SetImages(athlete);
		
		Stat firstStat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).GetNthBestStat(1);
		Stat secondStat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).GetNthBestStat(2);

		if(firstStat != null && firstStat.GetCount() > 0) {
			stat1Num.text = firstStat.GetCount().ToString();
			stat1Label.text = firstStat.GetStatName();
		} else {
			stat1Num.text = "";
			stat1Label.text = "";
		}

		if(secondStat != null && secondStat.GetCount() > 0) {
			stat2Num.text = secondStat.GetCount().ToString();
			stat2Label.text = secondStat.GetStatName();
		} else {
			stat2Num.text = "";
			stat2Label.text = "";
		}

		if(highlightsPanel.activeSelf) {
			highlightStartPos = highlightsPanel.transform.localPosition;
			statStartPos = statPanel.transform.localPosition;

			highlightEndPos = highlightStartPos;
			highlightEndPos.y = 295;

			statEndPos = statStartPos;
			statEndPos.y = -53;

			AthleteMatchData athleteMatchData = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete);
			for(int i = 0; i < athleteMatchData.statList.Count; i++) {
				statPanel.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = athleteMatchData.statList[i].GetCount().ToString();
				statPanel.transform.GetChild(i).GetChild(1).GetComponent<TextMeshProUGUI>().text = athleteMatchData.statList[i].GetStatName();
			}
		} //else the panel is fake
	}

	public void OnPointerEnter(PointerEventData eventData) {
		/*
		Debug.Log("Entered " + showcasedAthlete.name);
        ScrollUp();
		*/
    }

	public void OnPointerExit(PointerEventData eventData) {
        //ScrollDown();
    }

	public void ScrollUp() {
		StartCoroutine(MoveObjectFromTo(highlightsPanel, highlightStartPos, highlightEndPos, 0.3f));
		StartCoroutine(MoveObjectFromTo(statPanel, statStartPos, statEndPos, 0.3f));
	}

	public void ScrollDown() {
		StartCoroutine(MoveObjectFromTo(highlightsPanel, highlightEndPos, highlightStartPos, 0.3f));
		StartCoroutine(MoveObjectFromTo(statPanel, statEndPos, statStartPos, 0.3f));
	}

	public IEnumerator MoveObjectFromTo(GameObject obj, Vector3 start, Vector3 end, float duration) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();

		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			obj.transform.localPosition = Vector3.Lerp(start, end, timer/duration);

			yield return waiter;
		}

		obj.transform.localPosition = end;
	}
}
