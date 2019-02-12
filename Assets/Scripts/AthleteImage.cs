using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AthleteImage : MonoBehaviour {
	public TextMeshProUGUI text;
	public Image body;
	public Image face;
	public GameObject legHolder;
	public Image tail;

	public void SetImages(Athlete athlete) {
		text.text = athlete.name;

		body.sprite = athlete.GetBodyPartSprite("body");
		body.color = athlete.GetTeam().color;

		face.sprite = athlete.GetBodyPartSprite("face");
		face.color = athlete.skinColor;

		for(int i = legHolder.transform.childCount - 1; i > -1; i--) {
			Destroy(legHolder.transform.GetChild(i).gameObject);
		}

		List<Sprite> legSpriteList = athlete.GetMultipleBodyPartSprites("legs");
		for(int i = 0; i < legSpriteList.Count; i++) {

			GameObject newLeg = Instantiate(new GameObject(), legHolder.transform.position, Quaternion.identity, legHolder.transform);
			newLeg.AddComponent<Image>();
			Image leg = newLeg.GetComponent<Image>();

			//Should probably make this adaptable
			newLeg.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 150);

			if(i % 2 == 1) { //If i is odd
				newLeg.transform.localEulerAngles = new Vector3(0, 180, 0);
			} else {
				newLeg.transform.localEulerAngles = new Vector3(0, 0, 0);
			}

			leg.sprite = legSpriteList[i];
			leg.color = athlete.skinColor;
		}

		tail.sprite = athlete.GetBodyPartSprite("tailTip");
		tail.color = athlete.GetTeam().color;
	}
}
