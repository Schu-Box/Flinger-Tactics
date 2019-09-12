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
	public Image jersey;
	public TextMeshProUGUI numberText;

	public void SetImages(Athlete athlete) {
		numberText.text = athlete.jerseyNumber.ToString();

		body.sprite = athlete.athleteData.bodySprite;
		body.color = athlete.bodySkinColor;
		tail.color = athlete.bodySkinColor;

		faceBase.sprite = athlete.athleteData.faceBaseSprite;
		faceBase.color = athlete.skinColor;

		face.sprite = athlete.moodFace;

		jersey.sprite = athlete.athleteData.athleteJersey;
		jersey.color = athlete.GetTeam().primaryColor;
		
		float frontOffset = (legList[0].GetComponent<RectTransform>().rect.width - (legList[0].GetComponent<RectTransform>().rect.width * athlete.athleteData.frontLegRest)) / 2;
		float backOffset = (legList[0].GetComponent<RectTransform>().rect.width - (legList[0].GetComponent<RectTransform>().rect.width * athlete.athleteData.backLegRest)) / 2;

		for(int i = 0; i < legList.Count; i++) {
			legList[i].color = athlete.skinColor;

			RectTransform rt = legList[i].GetComponent<RectTransform>();

			if(i >= legList.Count / 2) {
				rt.offsetMin = new Vector2(0, frontOffset);
				rt.offsetMax = new Vector2(0, -frontOffset);
			} else {
				rt.offsetMin = new Vector2(0, backOffset);
				rt.offsetMax = new Vector2(0, -backOffset);
			}
		}

		hat.enabled = false;
	}

	public void SetMVP() {
		hat.enabled = true;
	}
}
