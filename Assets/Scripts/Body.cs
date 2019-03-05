using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Body : MonoBehaviour {

	private AthleteController athleteController;
	private Rigidbody2D rb;
	private Collider2D collie;
	private SpriteRenderer spriteRenderer;

	public void SetBody() {
		athleteController = GetComponentInParent<AthleteController>();

		rb = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void OnMouseEnter() {
		athleteController.MouseEnterBody();
	}

	private void OnMouseExit() {
		athleteController.MouseExitBody();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		athleteController.Collided(collision);
	}

	private void OnMouseDown() {

	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}
	
	public void SetSprite(Sprite sprite) {
		spriteRenderer.sprite = sprite;

		if(collie == null) {
			if(athleteController.GetAthlete().athleteType.typeString.ToLower() == "circle") {
				collie = GetComponent<CircleCollider2D>();
			} else {
				Destroy(gameObject.GetComponent<CircleCollider2D>());
				collie = gameObject.AddComponent<PolygonCollider2D>();
			}
		} else {
			Debug.Log("This collider should be null. Why it ain't tho?");
		}
	}

	public void AddForce(Vector2 force) {
		rb.AddForce(force);
	}

	public Vector2 GetVelocity() {
		return rb.velocity;
	}

	public float GetAngularVelocity() {
		return rb.angularVelocity;
	}

	public void StopMovement() {
		rb.velocity = Vector2.zero;
		rb.angularVelocity = 0f;
	}

	public void DisableBody() {
		//rb.bodyType = RigidbodyType2D.Static;
		//collie.enabled = false;
	}

	public void EnableBody() {
		//rb.bodyType = RigidbodyType2D.Dynamic;
		//collie.enabled = true;
	}
}
