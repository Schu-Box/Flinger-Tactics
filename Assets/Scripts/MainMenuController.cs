using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuController : MonoBehaviour {

	public GameObject titleScreen;
	public GameObject careerScreen;

	private void Start() {
		titleScreen.SetActive(true);
		careerScreen.SetActive(false);
	}

	public void SelectCareerMode() {
		titleScreen.SetActive(false);
		careerScreen.SetActive(true);
	}

	public void SelectPlayNowMode() {
		StartNewGame();
	}

	public void StartNewGame() {
		Debug.Log("Starting New Game");
		SceneManager.LoadScene(1);
	}

	public void BeginCareer() {
		
	}
}
