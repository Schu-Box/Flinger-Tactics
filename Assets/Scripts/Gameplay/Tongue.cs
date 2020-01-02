using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongue : MonoBehaviour {

	private AthleteController athlete;

	private SpriteRenderer spriteRenderer;
	private CircleCollider2D trigger;
	private float tongueRestLength;
	private float tongueTriggerRestLength;

	private bool tongueOut = false;

	

	public void SetTongue() {
		athlete = transform.GetComponentInParent<AthleteController>();

		spriteRenderer = GetComponent<SpriteRenderer>();
		trigger = GetComponent<CircleCollider2D>();

		tongueRestLength = spriteRenderer.size.y;
		tongueTriggerRestLength = trigger.offset.y;

		HideTongue();
		DisableTrigger();

		//Disable the tongue
		gameObject.SetActive(false);
	}

	private void OnTriggerEnter2D(Collider2D other) {
		/*
		if(!other.isTrigger) {
			athlete.TongueGrabbed(other.gameObject);
		}
		*/
	}

	public void HideTongue() {
		tongueOut = false;
		spriteRenderer.enabled = false;
	}

	public void RevealTongue() {
		tongueOut = true;
		spriteRenderer.enabled = true;
	}

	public bool GetTongueOut() {
		return tongueOut;
	}

	public void DisableTrigger() {
		trigger.enabled = false;
	}

	public void EnableTrigger() {
		trigger.enabled = true;
	}

	public IEnumerator ExtendTongue() {
		EnableTrigger();

		float tongueStretch = 0f;
		float speed = 200f;

		float timer = 0f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(true) {
			//Remove
			timer++;

			tongueStretch += Time.deltaTime * speed;

			spriteRenderer.size = new Vector2(spriteRenderer.size.x, tongueRestLength + tongueStretch);
			trigger.offset = new Vector2(trigger.offset.x, tongueTriggerRestLength + tongueStretch);

			yield return waiter;
		}
	}

	public IEnumerator RetractTongue() {
		trigger.offset = new Vector2(trigger.offset.x, tongueTriggerRestLength);
		DisableTrigger();
		
		float tongueStart = spriteRenderer.size.y;

		float duration = 0.5f;
		float timer = 0f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer++;
			float step = timer / duration;

			spriteRenderer.size = new Vector2(spriteRenderer.size.x, Mathf.Lerp(tongueStart, tongueRestLength, step));

			yield return waiter;
		}

		HideTongue();
	}

	public void SetTongueColor(Color color) {
		spriteRenderer.color = color;
	}
}
