using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AthleteImage : MonoBehaviour {
	public Image body;
	public Image faceBase;
	public Image face;
	public List<Image> legList;
	public Image tail;
	public Image hat;
	public TextMeshProUGUI numberText;

	public void SetImages(Athlete athlete) {
		numberText.text = athlete.jerseyNumber.ToString();

		body.sprite = athlete.athleteType.bodySprite;
		body.color = athlete.GetTeam().primaryColor;

		faceBase.sprite = athlete.athleteType.faceBaseSprite;
		faceBase.color = athlete.skinColor;

		face.sprite = athlete.moodFace;

		for(int i = 0; i < legList.Count; i++) {
			legList[i].color = athlete.skinColor;
		}

		tail.color = athlete.GetTeam().primaryColor;

		hat.enabled = false;
	}

	public void SetMVP() {
		hat.enabled = true;
	}
}
