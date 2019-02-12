using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AthleteData", menuName = "Athlete Data", order = 1)]
public class AthleteData : ScriptableObject {

	public List<string> nameList = new List<string>();

	public List<Color> skinColorList = new List<Color>();
}
