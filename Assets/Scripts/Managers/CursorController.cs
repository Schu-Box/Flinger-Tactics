using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour {

	public Sprite baseSprite;
	public Sprite clickSprite;
	public Sprite openHandSprite;
	public Sprite closedHandSprite;
	

	//private Vector2 hotspot;

	private bool hovering = false;
	private bool clicked = false;
	private bool dragging = false;

	private Coroutine releaseCoroutine;

	private RectTransform rectTransform;
	private Image image;
	

	void Start() {
		//hotspot = new Vector2(5, 8);
		UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware); 
    	UnityEngine.Cursor.visible = false;

    	rectTransform = GetComponent<RectTransform>();
		image = GetComponentInChildren<Image>();

		SetCursor(baseSprite);
	}

	void LateUpdate () {
		Vector3 screenPoint = Input.mousePosition;
		screenPoint.z = 10f;
		rectTransform.position = Camera.main.ScreenToWorldPoint(screenPoint);

		if(!clicked) {
			if(Input.GetMouseButtonDown(0)) {
				Click();
			}
		} else {
			if(Input.GetMouseButtonUp(0)) {
				Unclick();
			}
		}
	}

	void OnApplicationFocus(bool hasFocus) {
		if(hasFocus) {
			UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			UnityEngine.Cursor.visible = false;
		}
	}

	public void Click() {
		clicked = true;

		if(releaseCoroutine != null) {
			StopReleaseCoroutine();
		}

		if(!dragging) {
			SetCursor(clickSprite);
		}
	}

	public void Unclick() {
		clicked = false;

		if(releaseCoroutine == null) {
			if(hovering) {
				SetCursor(openHandSprite);
			} else {
				SetCursor(baseSprite);
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
				
				SetCursor(openHandSprite);
			}
		} else {
			if(!clicked && !dragging) {
				SetCursor(baseSprite);
			}
		}
	}

	public void SetDragging(bool drag) {
		dragging = drag;

		if(dragging) {
			SetCursor(closedHandSprite);
		} else {
			ReleaseFromDrag();
		}
	}

	public void ReleaseFromDrag() {
		releaseCoroutine = StartCoroutine(WaitThenDetermineState());

		SetCursor(openHandSprite);
	}

	public IEnumerator WaitThenDetermineState() {
		yield return new WaitForSeconds(0.5f);

		if(hovering) {
			SetCursor(openHandSprite);
		} else {
			SetCursor(baseSprite);
		}

		releaseCoroutine = null;
	}

	public void StopReleaseCoroutine() {
		StopCoroutine(releaseCoroutine);

		releaseCoroutine = null;
	}

	public void SetCursor(Sprite sprite) {
		//Cursor.SetCursor(sprite, hotspot, CursorMode.ForceSoftware);
		image.sprite = sprite;
	}
}
