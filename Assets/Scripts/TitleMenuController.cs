using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenuController : MonoBehaviour {

	public GameObject titleScreen;
	public GameObject careerScreen;

	public Team practiceTeam_Home;
	public Team practiceTeam_Away;

	private MatchController matchController;
	private RuleSet ruleSet = new RuleSet();

	private void Start() {
		PlayerPrefs.SetString("mode", "");

		matchController = FindObjectOfType<MatchController>();

		titleScreen.SetActive(true);
		careerScreen.SetActive(false);

		matchController.SetupCourt(ruleSet);

		practiceTeam_Home.SetNewRoster(ruleSet.GetRule("athleteOnRosterCount").value);
		practiceTeam_Away.SetNewRoster(ruleSet.GetRule("athleteOnRosterCount").value);

		matchController.SetUnbounded(true);

		matchController.SetSide(true, practiceTeam_Home);
		matchController.SetSide(false, practiceTeam_Away);


		StartCoroutine(IntroAnimation());
	}

	public IEnumerator IntroAnimation() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float timer = 0f;
		float duration = 0.5f;
		while(timer < duration) {
			timer++;

			yield return waiter;
		}

		matchController.StartMatch();
	}

	public void SelectCareerMode() {
		titleScreen.SetActive(false);
		careerScreen.SetActive(true);
	}

	public void SelectPlayNowMode() {
		PlayerPrefs.SetString("mode", "playNow");
		StartNewGame();
	}

	public void SelectGauntletMode() {
		PlayerPrefs.SetString("mode", "gauntlet");
		StartNewGame();
	}

	public void StartNewGame() {
		Debug.Log("Entering Play Scene");
		SceneManager.LoadScene(1);
	}

	public void BeginCareer() {
		
	}
}
