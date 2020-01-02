using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailTip : MonoBehaviour {

	private AthleteController athleteController;
	private SpriteMask spriteMask;
	private SpriteRenderer spriteRenderer;

	private float spriteMaskStartY;
	private float restPoint = 0f;
	private float minStretchPoint = 0f;
	private float maxStretchPoint = 0f;

	private float stretch = 5.5f; //subjective value - determines the rate of tail extension

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
		float step = (dirMag - athlete.minPull) / (athlete.maxPull - athlete.minPull);

		//I shouldn't be setting maxStretch point here every time it's adjusted but SetTailTip() leads to errors
		minStretchPoint = athlete.minPull * stretch;
		maxStretchPoint = athlete.maxPull * stretch;
		
		Vector3 newPosition = Vector3.zero;
		Vector3 newScale = new Vector3(1, spriteMaskStartY, 1);

		if(dirMag > athlete.minPull) {
			newPosition.y = Mathf.Lerp(minStretchPoint, maxStretchPoint, step);
			newScale.y = Mathf.Lerp(minStretchPoint + spriteMaskStartY, maxStretchPoint + spriteMaskStartY, step);
		} else {
			newPosition.y = restPoint;
		}

		transform.localPosition = newPosition;
		spriteMask.transform.localScale = newScale;
	}
}
