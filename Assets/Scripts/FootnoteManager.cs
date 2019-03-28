using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FootnoteManager : MonoBehaviour {

	private MatchController matchController;

	public GameObject footnotePanel;
	public TextMeshProUGUI nameText;
	public GameObject statTextHolder;
	private List<GameObject> statTextList = new List<GameObject>();

	private float originalFontSize;
	private float largeFontSize;

	private Color restStatTextColor;
	
	private Athlete footnoteAthlete;

	void Start() {
		matchController = FindObjectOfType<MatchController>();

		for(int i = 0; i < statTextHolder.transform.childCount; i++) {
			statTextList.Add(statTextHolder.transform.GetChild(i).gameObject);
		}

		originalFontSize = statTextList[0].transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize;
		largeFontSize = originalFontSize * 1.3f;

		footnotePanel.SetActive(false);
	}

	public void Display(Athlete athlete) {
		Team team = athlete.GetTeam();

		footnoteAthlete = athlete;

		footnotePanel.SetActive(true);

		footnotePanel.GetComponent<Image>().color = team.primaryColor;
		nameText.text = athlete.name;

		restStatTextColor = Color.white;

		if(statTextList != null) {
			for(int i = 0; i < statTextList.Count; i++) {
				Stat chosenStat;
				if(matchController.GetMatchStarted()) {
					chosenStat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).statList[i];
				} else { //If the match hasn't started, display their career stats
					chosenStat = athlete.statList[i];
				}

				TextMeshProUGUI topText = statTextList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
				topText.text = chosenStat.GetCount().ToString();

				TextMeshProUGUI bottomText = statTextList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>();
				bottomText.text = chosenStat.GetStatName();

				if(chosenStat.GetCount() > 0) {
					topText.color = Color.white;
				} else {
					topText.color = Color.grey;
				}
			}
		}
	}

	public void Hide() {
		footnoteAthlete = null;

		footnotePanel.SetActive(false);
	}

    public void UpdateIncreasedStat(StatType statIncreased, Athlete athlete) {
        //Display(footnoteAthlete);
		Team team = athlete.GetTeam();

		for(int i = 0; i < statTextList.Count; i++) {
			Stat stat = team.GetCurrentMatchData().GetTeamMatchData(team).GetAthleteMatchData(athlete).statList[i];
			if(stat.GetStatType() == statIncreased) {
				statTextList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = stat.GetCount().ToString();
				statTextList[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = stat.GetStatName();
				
				StartCoroutine(AnimateStatIncrease(statTextList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>(), athlete.GetTeam().primaryColor));

				break;
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
