using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AthleteData", menuName = "Athlete Data", order = 1)]
public class AthleteData : ScriptableObject {

	public List<string> nameList = new List<string>();

	public List<Color> skinColorList = new List<Color>();

	public string typeString;

	public float bumperModifier = 1f;

	public Sprite bodySprite;
	public Sprite faceBaseSprite;
	public Sprite legSprite;
	public Sprite athleteJersey;

	public float frontLegRest = 0.8f;
	public float frontLegMin = 0.9f;
	public float frontLegMax = 1.2f;

	public float backLegRest = 0.8f;
	public float backLegMin = 0.9f;
	public float backLegMax = 1.2f;

	public void AddNames(List<string> names) {
		for(int i = 0; i < names.Count; i++) {
			nameList.Add(names[i]);
		}
	}

	public void AddSkinColors(List<Color> colors) {
		for(int i = 0; i < colors.Count; i++) {
			skinColorList.Add(colors[i]);
		}
	}
}
