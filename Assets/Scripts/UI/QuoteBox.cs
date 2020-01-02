using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuoteBox : MonoBehaviour {

	private RectTransform rt;

	private Image image;
	private TextMeshProUGUI text;

	private Vector2 startOffsetMin = new Vector2(400f, 0);
	private Vector2 startOffsetMax = new Vector2(-400f, 0);
	private float baseOffset = 80f;

	private float xOffset = 2f;

	private Vector2 latestFullOffset = Vector2.zero;

	private float charIntervalIncrease = 15f;

	private AthleteController latestAthleteController;
	private string latestQuote;
	private Vector3 distanceFromAC;

	private Coroutine currentCoroutine = null;

	bool goingRight = true;
	public void SetQuoteBox(AthleteController ac, string quote) {
		image = transform.GetChild(0).GetComponent<Image>();
		rt = image.transform.GetComponent<RectTransform>();

		text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
		text.text = "";

		Vector3 offset = new Vector3(0, 0.4f, 0);
		if(ac.transform.rotation.eulerAngles.z < 180) {
			goingRight = true;

			image.transform.localEulerAngles = new Vector3(0, 0, 0);
			text.transform.localEulerAngles = new Vector3(0, 0, 0);

			offset.x = xOffset;
		} else {
			goingRight = false;

			image.transform.localEulerAngles = new Vector3(0, 180, 0);
			text.transform.localEulerAngles = new Vector3(0, 180, 0);

			offset.x = -xOffset;
		}
		transform.position = ac.transform.position + offset;

		image.color = ac.GetAthlete().GetTeam().GetDarkTint();

		latestQuote = quote;
		latestAthleteController = ac;
		distanceFromAC = transform.position - ac.transform.position;

		latestAthleteController.SetCurrentQuoteBox(this);

		currentCoroutine = StartCoroutine(OpenQuote());
	}

	public IEnumerator OpenQuote() {
		gameObject.SetActive(true);

		latestAthleteController.SetSpeaking(true);

		if(goingRight) {
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = startOffsetMax;

			latestFullOffset.x = startOffsetMax.x + baseOffset + (latestQuote.Length * charIntervalIncrease);
		} else {
			rt.offsetMin = startOffsetMin;
			rt.offsetMax = Vector2.zero;

			latestFullOffset.x = startOffsetMin.x - baseOffset - (latestQuote.Length * charIntervalIncrease);
		}

		WaitForEndOfFrame waiter = new WaitForEndOfFrame();
		float duration = 0.1f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.unscaledDeltaTime;

			if(goingRight) {
				rt.offsetMax = Vector2.Lerp(startOffsetMax, latestFullOffset, timer/duration);
			} else {
				rt.offsetMin = Vector2.Lerp(startOffsetMin, latestFullOffset, timer/duration);
			}

			yield return waiter;
		}

		if(goingRight) {
			rt.offsetMax = latestFullOffset;
		} else {
			rt.offsetMin = latestFullOffset;
		}

		text.text = latestQuote;

		currentCoroutine = StartCoroutine(SustainAndClose());
	}

	public IEnumerator SustainAndClose() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 2f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			Vector3 newPosition = latestAthleteController.transform.position + distanceFromAC;
			transform.position = newPosition;

			yield return waiter;
		}

		currentCoroutine = StartCoroutine(CloseQuote());
	}

	public void PrematurelyClose() {
		if(currentCoroutine != null) {
			StopCoroutine(currentCoroutine);

			currentCoroutine = StartCoroutine(CloseQuote());
		}
	}

	public IEnumerator CloseQuote() {

		if(latestAthleteController != null) {
			latestAthleteController.closingQuote = true;
		}

		text.text = "";

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.05f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			if(goingRight) {
				rt.offsetMax = Vector2.Lerp(latestFullOffset, startOffsetMax, timer/duration);
			} else {
				rt.offsetMin = Vector2.Lerp(latestFullOffset, startOffsetMin, timer/duration);
			}

			yield return waiter;
		}

		if(goingRight) {
			rt.offsetMax = startOffsetMax;
		} else {
			rt.offsetMin = startOffsetMin;
		}

		if(latestAthleteController != null) {
			latestAthleteController.SetSpeaking(false);
			latestAthleteController.GetFace().DetermineFaceState();
			latestAthleteController.closingQuote = false;
		}

		currentCoroutine = null;

		Destroy(gameObject);
	}
}
