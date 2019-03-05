using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTip : MonoBehaviour {

	private AthleteController athleteController;
	private SpriteMask spriteMask;
	private SpriteRenderer spriteRenderer;

	private float restPoint = 0f;
	private float maxStretchPoint = 0f;

	private float stretch = 7f;

	public void SetTailTip() {
		athleteController = GetComponentInParent<AthleteController>();

		spriteMask = transform.parent.GetComponentInChildren<SpriteMask>();

		spriteRenderer = GetComponent<SpriteRenderer>();

		restPoint = transform.localPosition.y;
	}

	public void OnMouseEnter() {
		athleteController.MouseEnterTail();
	}

	public void OnMouseExit() {
		athleteController.MouseExitTail();
	}

	/*
	private void OnMouseDown() {
		athleteController.Clicked();
	}
	*/

	private void OnMouseDrag() {
		athleteController.TailAdjusted();
	}

	private void OnMouseUp() {
		athleteController.Unclicked();
	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}

	public void AdjustTailPosition(float dirMag) {
		Athlete athlete = athleteController.GetAthlete();
		float step = (dirMag - athlete.minPull) / (athlete.maxPull);

		maxStretchPoint = athlete.maxPull * stretch;

		Vector3 newPosition = Vector3.zero;
		newPosition.y = Mathf.Lerp(restPoint, maxStretchPoint, step);
		transform.localPosition = newPosition;

		Vector3 newScale = new Vector3(1, 3, 1);
		newScale.y = Mathf.Lerp(3, maxStretchPoint + (athlete.minPull * stretch), step);
		spriteMask.transform.localScale = newScale;
	}
}
