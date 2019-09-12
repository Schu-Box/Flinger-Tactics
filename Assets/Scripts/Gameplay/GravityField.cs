using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour {

	private CircleCollider2D colliderTrigger;
	private SpriteRenderer spriteRenderer;

	private Vector3 fullScale;
	private float fullRadius;

	private float attractionStrength = 10f;

	private List<GameObject> objectsInGravity = new List<GameObject>();
	private List<Coroutine> gravityCoroutines = new List<Coroutine>();

	private bool gravityFieldEnabled = false;

	void Start() {
		colliderTrigger = GetComponent<CircleCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();

		fullScale = transform.localScale;
		fullRadius = colliderTrigger.radius;

		spriteRenderer.enabled = false;
		colliderTrigger.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.CompareTag("Ball")) {
			Debug.Log("It the ball");
			objectsInGravity.Add(other.gameObject);	
			StartGravitation(other.gameObject);
		}
	}

	private void OnTriggerExit2D(Collider2D other) {
		if(other.CompareTag("Ball")) {
			StopGravitation(other.gameObject);
		}
	}

	public void EnableGravityField() {
		if(!gravityFieldEnabled) {
			spriteRenderer.enabled = true;
			
			gravityFieldEnabled = true;

			StartCoroutine(GrowGravityField());
		}
	}

	public void ActivateGravityField() {
		colliderTrigger.enabled = true;
	}

	public IEnumerator GrowGravityField() {
		transform.localScale = Vector3.zero;
		colliderTrigger.radius = 0f;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.5f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localScale = Vector3.Lerp(Vector3.zero, fullScale, timer/duration);
			colliderTrigger.radius = Mathf.Lerp(0, fullRadius, timer/duration);

			yield return waiter;
		}
	}

	public void DisableGravityField() {
		if(gravityFieldEnabled) {
			gravityFieldEnabled = false;
			StopAllGravitation();

			StartCoroutine(ShrinkGravityField());
		}
	}

	public IEnumerator ShrinkGravityField() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.2f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			transform.localScale = Vector3.Lerp(fullScale, Vector3.zero, timer/duration);
			colliderTrigger.radius = Mathf.Lerp(fullRadius, 0, timer/duration);

			yield return waiter;
		}

		spriteRenderer.enabled = false;
		colliderTrigger.enabled = false;
	}

	private void StartGravitation(GameObject gravitationalObject) { //gravObj must have a rigidbody2D
		gravityCoroutines.Add(StartCoroutine(Gravitate(gravitationalObject)));
	}

	private IEnumerator Gravitate(GameObject gravitationalObject) {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(true) {
			Vector2 offset = gravitationalObject.transform.position - transform.position;

			float magSqr = offset.sqrMagnitude;

			if(magSqr > 0) {
				GetComponentInParent<Rigidbody2D>().AddForce(attractionStrength * offset.normalized / magSqr);
			}

			yield return waiter;
		}
	}

	public void StopAllGravitation() {
		for(int i = objectsInGravity.Count - 1; i >= 0; i--) {
			StopGravitation(objectsInGravity[i]);
		}
	}

	public void StopGravitation(GameObject gravitationalObject) {
		int index = -1;
		for(int i = 0; i < objectsInGravity.Count; i++) {
			if(objectsInGravity[i] == gravitationalObject) {
				index = i;
				break;
			}
		}

		if(index != -1) {
			StopCoroutine(gravityCoroutines[index]);
			gravityCoroutines.RemoveAt(index);
			objectsInGravity.RemoveAt(index);
		} else {
			Debug.Log("That gravitational object is not currently gravitating");
		}
	}

	public void PossessBall(BallController ball) {
		StopGravitation(ball.gameObject);

		ball.StopMovement();

		DisableGravityField();
	}

	public bool IsGravityFieldEnabled() {
		return gravityFieldEnabled;
	}
}
