using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenuController : MonoBehaviour {

	public GameObject titleScreen;
	public GameObject careerScreen;

	public int numPracticeTeamAthletes = 1;

	public Team practiceTeam_Home;
	public Team practiceTeam_Away;

	private MatchController matchController;

	private void Start() {
		matchController = FindObjectOfType<MatchController>();

		titleScreen.SetActive(true);
		careerScreen.SetActive(false);

		matchController.SetupCourt();

		practiceTeam_Home.SetNewRoster(numPracticeTeamAthletes);
		practiceTeam_Away.SetNewRoster(numPracticeTeamAthletes);

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
		StartNewGame();
	}

	public void StartNewGame() {
		Debug.Log("Entering Play Now");
		SceneManager.LoadScene(1);
	}

	public void BeginCareer() {
		
	}
}
