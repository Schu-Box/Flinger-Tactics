using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AthletePostMatchStatBox : MonoBehaviour {

	public TextMeshProUGUI nameText;
	public TextMeshProUGUI goalsText;
	public TextMeshProUGUI assistsText;
	public TextMeshProUGUI touchesText;
	public TextMeshProUGUI bumpsText;
	
	public void SetAthleteStats(Athlete athlete) {
		nameText.text = athlete.name;
		/*
		goalsText.text = athlete.goals + " Goals";
		assistsText.text = athlete.bumpersDestroyed + " Bumpers Destroyed";
		touchesText.text = athlete.touches + " Touches";
		bumpsText.text = athlete.bumps + " Bumps";
		*/
	}
}
