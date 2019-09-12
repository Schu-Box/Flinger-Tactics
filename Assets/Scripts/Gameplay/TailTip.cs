using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTip : MonoBehaviour {

	private AthleteController athleteController;
	private SpriteMask spriteMask;
	private SpriteRenderer spriteRenderer;

	private float spriteMaskStartY;
	private float restPoint = 0f;
	private float maxStretchPoint = 0f;

	private float stretch = 7f;

	public void SetTailTip() {
		athleteController = GetComponentInParent<AthleteController>();

		spriteMask = transform.parent.GetComponentInChildren<SpriteMask>();
		spriteMaskStartY = spriteMask.transform.localScale.y;

		spriteRenderer = GetComponent<SpriteRenderer>();

		restPoint = transform.localPosition.y;
	}

	public void OnMouseEnter() {
		athleteController.MouseEnter();
	}

	public void OnMouseExit() {
		athleteController.MouseExit();
	}

	private void OnMouseDown() {
		athleteController.MouseClick();
	}

	private void OnMouseDrag() {
		athleteController.MouseDrag();
	}

	private void OnMouseUp() {
		athleteController.Unclicked();
	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}

	public void SetMaterial(Shader material) {
		spriteRenderer.material.shader = material;
	}

	public void AdjustTailPosition(float dirMag) {
		Athlete athlete = athleteController.GetAthlete();
		float step = (dirMag - athlete.minPull) / (athlete.maxPull);

		maxStretchPoint = athlete.maxPull * stretch;

		Vector3 newPosition = Vector3.zero;
		newPosition.y = Mathf.Lerp(restPoint, maxStretchPoint, step);
		transform.localPosition = newPosition;

		Vector3 newScale = new Vector3(1, spriteMaskStartY, 1);
		newScale.y = Mathf.Lerp(spriteMaskStartY, maxStretchPoint + (athlete.minPull * stretch), step);
		spriteMask.transform.localScale = newScale;
	}
}
