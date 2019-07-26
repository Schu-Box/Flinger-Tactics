using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VictoryPanel : MonoBehaviour {

	private Image panelBorder;
	private Image panel;
	private Image decorative;

	private TextMeshProUGUI teamText;
	private TextMeshProUGUI resultText;

	public Vector3 startOffPosition;
	public Vector3 onPosition;
	//public Vector3 endOffPosition;

	void Awake() {
		panelBorder = GetComponent<Image>();
		panel = transform.GetChild(0).GetComponent<Image>();
		decorative = transform.GetChild(1).GetComponent<Image>();

		teamText = panel.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
		resultText = panel.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
	}

	public void SetVictoryTeam(Team team) {
		gameObject.SetActive(true);

		panel.color = team.GetLightTint();

		panelBorder.color = team.primaryColor;
		decorative.color = team.secondaryColor;

		teamText.color = team.secondaryColor;
		teamText.text = team.name;

		StartCoroutine(MoveVictoryPanelOn());
	}

	private IEnumerator MoveVictoryPanelOn() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.5f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localPosition = Vector3.Lerp(startOffPosition, onPosition, timer/duration);

			yield return waiter;
		}
	}
}
