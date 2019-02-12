using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Team {
    public string name;
	public string fieldString;
    public Color color;

    public List<Athlete> athletes = new List<Athlete>();

    //In Game
    public int score;
    public bool wonTheGame = false;

	public void SetNewRoster(int num) {
		for(int i = 0; i < num; i++) {
			athletes.Add(new Athlete());
			athletes[i].SetTeam(this);
		}
	}

}

public enum AthleteType {
	Standard, Longchuck, Grodwag
}

public class Athlete {

	public AthleteData standardAthleteData;
	
	//Where do I get this from???

	public string name;
	public AthleteType athleteType;
	public Color skinColor;
	public Color tongueColor;

	private Sprite bodySprite;
	private Sprite faceSprite;
	private List<Sprite> legSpriteList = new List<Sprite>();
	private Sprite tailBodySprite;
	private Sprite tailTipSprite;

	public float flingForce = 700f;

	public float minPull = 0.3f;
	public  float maxPull = 1.2f;

	private Team team;

	private bool hasPoleTail = true;

	public List<Stat> statList = new List<Stat>();

	public Athlete() {

		standardAthleteData = Resources.Load<AthleteData>("StandardAthleteData");

		List<string> nameList = standardAthleteData.nameList;
		name = nameList[Random.Range(0, nameList.Count)];

		List<Color> colorList = standardAthleteData.skinColorList;
		skinColor = colorList[Random.Range(0, colorList.Count)];

		tongueColor = colorList[Random.Range(0, colorList.Count)];

		float rando = Random.value;
		if (rando <= 0.2) {
			athleteType = AthleteType.Grodwag;

			hasPoleTail = false;

			minPull = 0.3f;
			maxPull = 0.5f;

			flingForce = 6000f;
		} else if (rando <= 0.5) {
			athleteType = AthleteType.Longchuck;

			flingForce = 800f;
		} else {
			athleteType = AthleteType.Standard;

			flingForce = 700f;
		}

		//Stats
		statList.Add(new Stat("Goals"));
		statList.Add(new Stat("Assists"));
		statList.Add(new Stat("Touches"));
		statList.Add(new Stat("Bumps"));
		statList.Add(new Stat("Bounces"));
		statList.Add(new Stat("Breaks"));
	}

	public void SetTeam(Team t) {
		team = t;
	}

	public Team GetTeam() {
		return team;
	}

	public void SetBodyPartSprite(string bodyPart, Sprite bodyPartSprite) {
		switch(bodyPart) {
			case "body":
				bodySprite = bodyPartSprite;
				break;
			case "face":
				faceSprite = bodyPartSprite;
				break;
			case "tailBody":
				tailBodySprite = bodyPartSprite;
				break;
			case "tailTip":
				tailTipSprite = bodyPartSprite;
				break;
			default:
				Debug.Log("Error: That body part doesn't exist or do nothing");
				break;
		}
	}

	public void SetMultipleBodyPartSprites(string bodyPart, List<Sprite> sprites) {
		switch(bodyPart) {
			case "legs":
				legSpriteList = new List<Sprite>();
				for(int i = 0; i < sprites.Count; i++) {
					legSpriteList.Add(sprites[i]);
				}
				break;
			default:
				Debug.Log("Error: Those multiple body parts don't do anything");
				break;
		}
	}

	public Sprite GetBodyPartSprite(string bodyPart) {
		switch(bodyPart) {
			case "body":
				return bodySprite;
			case "face":
				return faceSprite;
			case "tailBody":
				return tailBodySprite;
			case "tailTip":
				return tailTipSprite;
			default:
				Debug.Log("That body part is non-existant chump");
				return null;
		}
	}

	public List<Sprite> GetMultipleBodyPartSprites(string bodyParts) {
		switch(bodyParts) {
			case "legs":
				return legSpriteList;
			default:
				Debug.Log("Those body parts do no exist bruh");
				return null;
		}
	}

	public bool HasPoleTail() {
		return hasPoleTail;
	}

	public void IncreaseStat(string statName) {
		Stat stat = null;
		for(int i = 0; i < statList.Count; i++) {
			if(statList[i].GetStatName().ToLower() == statName.ToLower()) {
				stat = statList[i];
				break;
			}
		}

		if(stat != null) {
			stat.IncreaseValue();
		} else {
			Debug.Log("That stat doesn't exist bruh");
		}
	}
}

public class Stat {
	string stat;
	int value;

	public Stat(string statName) {
		stat = statName;
		value = 0;
	}

	public string GetStatName() {
		return stat;
	}

	public int GetValue() {
		return value;
	}

	public void IncreaseValue() {
		value++;
	}
}
