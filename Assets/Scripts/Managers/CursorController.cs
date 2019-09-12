using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour {

	public Texture2D baseSprite;
	public Texture2D clickSprite;
	public Texture2D openHandSprite;
	public Texture2D closedHandSprite;
	

	private Vector2 hotspot;

	private Vector3 clickSpot;

	private bool hovering = false;
	private bool clicked = false;
	private bool dragging = false;

	private Coroutine releaseCoroutine;
	

	void Start() {
		hotspot = new Vector2(5, 8);
	}

	void Update () {
		//if cursor down
		if(!clicked) {
			if(Input.GetMouseButtonDown(0)) {
				Click();
			}
		} else {
			if(Input.GetMouseButtonUp(0)) {
				Unclick();
			} else {
				//Cursor.
			}
		}
	}

	public void Click() {
		clicked = true;
		clickSpot = Input.mousePosition;

		if(releaseCoroutine != null) {
			StopReleaseCoroutine();
		}

		if(!dragging) {
			Cursor.SetCursor(clickSprite, hotspot, CursorMode.Auto);
		}
	}

	public void Unclick() {
		clicked = false;

		if(releaseCoroutine == null) {
			if(hovering) {
				Cursor.SetCursor(openHandSprite, hotspot, CursorMode.Auto);
			} else {
				Cursor.SetCursor(baseSprite, hotspot, CursorMode.Auto);
			}
		}
	}

	public void SetHover(bool hover) {
		hovering = hover;

		if(hover) {
			if(!clicked && !dragging) {
				if(releaseCoroutine != null) {
					StopReleaseCoroutine();
				}
				
				Cursor.SetCursor(openHandSprite, hotspot, CursorMode.Auto);
			}
		} else {
			if(!clicked && !dragging) {
				Cursor.SetCursor(baseSprite, hotspot, CursorMode.Auto);
			}
		}
	}

	public void SetDragging(bool drag) {
		dragging = drag;

		if(dragging) {
			Cursor.SetCursor(closedHandSprite, hotspot, CursorMode.Auto);
		} else {
			ReleaseFromDrag();
		}
	}

	public void ReleaseFromDrag() {
		releaseCoroutine = StartCoroutine(WaitThenDetermineState());

		Cursor.SetCursor(openHandSprite, hotspot, CursorMode.Auto);
	}

	public IEnumerator WaitThenDetermineState() {
		yield return new WaitForSeconds(0.5f);

		if(hovering) {
			Cursor.SetCursor(openHandSprite, hotspot, CursorMode.Auto);
		} else {
			Cursor.SetCursor(baseSprite, hotspot, CursorMode.Auto);
		}

		releaseCoroutine = null;
	}

	public void StopReleaseCoroutine() {
		StopCoroutine(releaseCoroutine);

		releaseCoroutine = null;
	}
}
