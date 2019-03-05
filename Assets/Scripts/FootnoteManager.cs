using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FootnoteManager : MonoBehaviour {

	private MatchController matchController;

	[Header("Home")]
	public GameObject footnote_home;
	public Image namePanel_home;
	public TextMeshProUGUI number_home;
	public TextMeshProUGUI name_home;
	public GameObject statTextHolder_home;
	private List<GameObject> statTextList_home = new List<GameObject>();
	

	[Header("Away")]
	public GameObject footnote_away;
	public Image namePanel_away;
	public TextMeshProUGUI number_away;
	public TextMeshProUGUI name_away;
	public GameObject statTextHolder_away;
	private List<GameObject> statTextList_away = new List<GameObject>();
	

	private float originalFontSize;
	private float largeFontSize;

	private Color restStatTextColor;
	
	private Athlete footnoteAthlete_home;
	private Athlete footnoteAthlete_away;

	void Start() {
		matchController = FindObjectOfType<MatchController>();

		for(int i = 0; i < statTextHolder_home.transform.childCount; i++) {
			statTextList_home.Add(statTextHolder_home.transform.GetChild(i).gameObject);
		}
		for(int i = 0; i < statTextHolder_away.transform.childCount; i++) {
			statTextList_away.Add(statTextHolder_away.transform.GetChild(i).gameObject);
		}

		originalFontSize = statTextList_home[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize;
		largeFontSize = originalFontSize * 1.3f;

		footnote_home.SetActive(false);
		footnote_away.SetActive(false);
	}

	public void Display(Athlete athlete) {
		Team team = athlete.GetTeam();

		List<GameObject> textList = null;

		Athlete a;
		if(matchController.GetAthleteInitiater() != null) {
			a = matchController.GetAthleteInitiater().GetAthlete();
		} else {
			a = null;
		}

		if(team == matchController.GetTeam(true)) {
			if(footnoteAthlete_home == null) { //If the home slot is available
				footnoteAthlete_home = athlete;
				textList = statTextList_home;

				footnote_home.SetActive(true);

				namePanel_home.GetComponent<Image>().color = team.primaryColor;
				number_home.text = athlete.jerseyNumber.ToString();
				name_home.text = athlete.name;
			} else {
				if(footnoteAthlete_home != athlete) {
					footnoteAthlete_away = athlete;
					textList = statTextList_away;

					footnote_away.SetActive(true);

					namePanel_away.GetComponent<Image>().color = team.primaryColor;
					number_away.text = athlete.jerseyNumber.ToString();
					name_away.text = athlete.name;
				}
			}
		} else {
			if(footnoteAthlete_away == null) { //If the away slot is available
				footnoteAthlete_away = athlete;
				textList = statTextList_away;

				footnote_away.SetActive(true);

				namePanel_away.GetComponent<Image>().color = team.primaryColor;
				number_away.text = athlete.jerseyNumber.ToString();
				name_away.text = athlete.name;
			} else {
				if(footnoteAthlete_away != athlete) {
					footnoteAthlete_home = athlete;
					textList = statTextList_home;

					footnote_home.SetActive(true);

					namePanel_home.GetComponent<Image>().color = team.primaryColor;
					number_home.text = athlete.jerseyNumber.ToString();
					name_home.text = athlete.name;
				}
			}
		}

		restStatTextColor = Color.white;

		if(textList != null) {
			for(int i = 0; i < textList.Count; i++) {
				Stat chosenStat;
				if(matchController.GetMatchStarted()) {
					chosenStat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).statList[i];
				} else { //If the match hasn't started, display their career stats
					chosenStat = athlete.statList[i];
				}

				TextMeshProUGUI topText = textList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
				topText.text = chosenStat.GetCount().ToString();

				TextMeshProUGUI bottomText = textList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
				bottomText.text = chosenStat.GetStatName();

				if(chosenStat.GetCount() > 0) {
					topText.color = Color.white;
				} else {
					topText.color = Color.grey;
				}
			}
		}
	}

	public void HideBoth() {
		if(footnoteAthlete_home != null) {
			Hide(footnoteAthlete_home);
		}
		if(footnoteAthlete_away != null) {
			Hide(footnoteAthlete_away);
		}
	}

	public void Hide(Athlete athlete) {
		Athlete a;
		if(matchController.GetAthleteInitiater() != null) {
			a = matchController.GetAthleteInitiater().GetAthlete();
		} else {
			a = null;
		}

		if(athlete == footnoteAthlete_home) {
			if(a != athlete) { //If this athlete is not the initator, hide it
				footnote_home.SetActive(false);
				footnoteAthlete_home = null;
			}
		} else {
			if(a != athlete) {
				footnote_away.SetActive(false);
				footnoteAthlete_away = null;
			}
		}
	}

    public void UpdateIncreasedStat(StatType statIncreased, Athlete athlete) {
        //Display(footnoteAthlete);
		Team team = athlete.GetTeam();

		if(athlete == footnoteAthlete_home) {
			for(int i = 0; i < statTextList_home.Count; i++) {
				Stat stat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).statList[i];
				if(stat.GetStatType() == statIncreased) {
					statTextList_home[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stat.GetCount().ToString();
					statTextList_home[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = stat.GetStatName();
					
					StartCoroutine(AnimateStatIncrease(statTextList_home[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>(), footnoteAthlete_home.GetTeam().primaryColor));

					break;
				}
			}
		} else { //Assumes it's the away athlete
			for(int i = 0; i < statTextList_away.Count; i++) {
				Stat stat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).statList[i];
				if(stat.GetStatType() == statIncreased) {
					statTextList_away[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stat.GetCount().ToString();
					statTextList_away[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = stat.GetStatName();
					
					StartCoroutine(AnimateStatIncrease(statTextList_away[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>(), footnoteAthlete_away.GetTeam().primaryColor));

					break;
				}
			}
		}
	}

	public IEnumerator AnimateStatIncrease(TextMeshProUGUI txt, Color flashColor) {

		txt.color = flashColor;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.3f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			txt.fontSize = Mathf.Lerp(originalFontSize, largeFontSize, timer/duration);

			txt.transform.parent.GetComponent<Image>().color = Color.Lerp(Color.grey, flashColor, timer/duration);
			
			yield return waiter;
		}

		duration = 0.7f;
		timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;
			float step = timer/duration;

			txt.fontSize = Mathf.Lerp(largeFontSize, originalFontSize, step);

			//txt.color = Color.Lerp(flashColor, restStatTextColor, step);

			txt.transform.parent.GetComponent<Image>().color = Color.Lerp(flashColor, Color.grey, timer/duration);

			yield return waiter;
		}

		//txt.color = restStatTextColor;
	}
}
