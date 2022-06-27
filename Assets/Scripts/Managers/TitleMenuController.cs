using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleMenuController : MonoBehaviour {

	public GameObject titleScreen;
	public GameObject careerScreen;
	public GameObject titleLogo;
	public CustomButton returnButton;

	public GameObject escapePanel;
	public Slider musicVolumeSlider;
	public Slider soundEffectsVolumeSlider;

	public Team practiceTeam_Home;
	public Team practiceTeam_Away;	

	private MatchController matchController;
	private CameraController cameraController;
	private RuleSet ruleSet = new RuleSet();

	private void Start() {
		PlayerPrefs.SetString("mode", "");

		Time.timeScale = 1f;

		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();

		titleScreen.SetActive(true);
		careerScreen.SetActive(false);

		escapePanel.SetActive(false);

		ruleSet.ChangeRule(ruleSet.GetRuleSlot("athleteRosterCount").possibleRules[4]);
		ruleSet.ChangeRule(ruleSet.GetRuleSlot("athleteFieldCount").possibleRules[1]);
		ruleSet.ChangeRule(ruleSet.GetRuleSlot("ballCount").possibleRules[2]);
		ruleSet.ChangeRule(ruleSet.GetRuleSlot("bumperCount").possibleRules[3]);
		ruleSet.GetRule("turnCount").value = 9999;

		matchController.SetupCourt(ruleSet);

		practiceTeam_Home.SetNewRoster(ruleSet.GetRule("athleteRosterCount").value);
		practiceTeam_Away.SetNewRoster(ruleSet.GetRule("athleteRosterCount").value);

		matchController.SetUnbounded(true);

		matchController.SetSide(true, practiceTeam_Home);
		matchController.SetSide(false, practiceTeam_Away);

		//StartCoroutine(IntroAnimation());

		musicVolumeSlider.value = AudioManager.musicVolume;
        soundEffectsVolumeSlider.value = AudioManager.soundEffectsVolume;
	}

	private void Update() {
		if(Input.GetButtonDown("Cancel")) {
            ToggleEscapeMenu();
        } 
	}

	public IEnumerator IntroAnimation() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		/*
		float timer = 0f;
		float duration = 0.5f;
		while(timer < duration) {
			timer++;

			yield return waiter;
		}
		*/

		Vector3 titleStart = titleLogo.transform.localScale;
		Vector3 titleExpand = titleStart * 1.05f;

		float timer = 0f;
		float duration = 0.5f;
		while(true) {
			timer += Time.deltaTime;

			titleLogo.transform.localScale = Vector3.Lerp(titleStart, titleExpand, timer % duration);

			yield return waiter;
		}
	}

	public void SelectPracticeMode() {
		DisplayPracticeMatch();
	}

	public void SelectPlayNowMode() {
		PlayerPrefs.SetString("mode", "playNow");
		StartNewGame();
	}

	public void SelectCompetitiveMode() {
		PlayerPrefs.SetString("mode", "competitive");
		StartNewGame();
	}

	public void SelectGauntletMode() {
		PlayerPrefs.SetString("mode", "gauntlet");
		StartNewGame();
	}

	public void SelectTournamentMode() {
		PlayerPrefs.SetString("mode", "tournament");
		StartNewGame();
	}

	public void StartNewGame() {
		Debug.Log("Entering Play Scene");
		SceneManager.LoadScene(1);
	}

	public void SelectCareerMode() {
		titleScreen.SetActive(false);
		careerScreen.SetActive(true);
	}

	public void BeginCareer() {
		
	}

	public void DisplayPracticeMatch() {
        StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.startPosition));

		matchController.ResetSides();

        matchController.StartMatch();
    }

	public void EnableReturnButton() {
		returnButton.EnableButton();
	}

	public void DisableReturnButton() {
		returnButton.DisableButton();
	}

	public void UndisplayPracticeMatch() {
		StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));

		//matchController.PrematurelyEndTurn();
		matchController.EndMatch();
	}

	public string GetTutorialQuote() {
		List<string> tutorialStrings = new List<string> {
			"Did you get all that? We can repeat everything!", //Only shows up after after first cycle
			"Hey I'm a flinger! Can you fling me?", "That looks like fun! My turn!", 
			"Fling me into their bumpers to break them!", "Oh fling me too! I wanna break some bumpers!", 
			"We can score by hitting a ball into the goal!", "Scoring is fun! That's how we win. I love winning!",
			"Fling me into the goal and we can substitute next turn!", "Substituting is a great idea when we're out of position.", 
			"Did you know every flinger has a special ability?", "Circles can create shockwaves by hitting their own bumpers!", 
			"Boxy flingers can repair their broken bumpers!", "Triangles are big meanies. They knock flingers unconscious..."
		};

		int num = matchController.GetTurn() * 2;
		if(matchController.GetTurnTeam() == matchController.GetTeam(false)) {
			num--;
		}

		num = num % tutorialStrings.Count;

		return tutorialStrings[num];
	}

	public void DisplayEmailList() {
		StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upRightPosition));
	}

	public void UndisplayEmailList() {
		StartCoroutine(MoveObjectToPosition(Camera.main.gameObject, cameraController.upPosition));
	}

	public void ToggleEscapeMenu() {
        if(escapePanel.activeSelf) {
            escapePanel.SetActive(false);
            Time.timeScale = 1f;
        } else {
            escapePanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

	public void QuitGame() {
		Application.Quit();
	}

	//This is copied from canvasManager, bad programmer! -Should move this to camera controller since that's primarily what it's used for
	public IEnumerator MoveObjectToPosition(GameObject obj, Vector3 endPos) {
        Vector3 startPos = obj.transform.position;

        float timer = 0f;
        float duration = 0.3f;
        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        while(timer < duration) {
            timer += Time.deltaTime;

            obj.transform.position = Vector3.Lerp(startPos, endPos, timer/duration);

            yield return waiter;
        }

        obj.transform.position = endPos;
    }
}
