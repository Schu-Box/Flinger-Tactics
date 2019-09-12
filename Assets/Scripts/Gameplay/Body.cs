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

	public void OnMouseEnter() {
		athleteController.MouseEnter();
	}

	public void OnMouseExit() {
		athleteController.MouseExit();
	}

	private void OnMouseDrag() {
		athleteController.MouseDrag();
	}

	private void OnMouseUp() {
		athleteController.Unclicked();
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		athleteController.Collided(collision);
	}


	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}

	public void SetMaterial(Shader material) {
		spriteRenderer.material.shader = material;
	}
	
	public void SetSprite(Sprite sprite) {
		spriteRenderer.sprite = sprite;

		if(athleteController.GetAthlete().athleteData.classString == "Circle") {
			collie = GetComponent<CircleCollider2D>();
		} else {
			Destroy(gameObject.GetComponent<CircleCollider2D>()); //Call this to get rid of the prefab circle
			collie = gameObject.AddComponent<PolygonCollider2D>();
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
		collie.isTrigger = true;
	}

	public void EnableBody() {
		//rb.bodyType = RigidbodyType2D.Dynamic;
		collie.isTrigger = false;
	}

	public void EnableStaticBody() {
		rb.bodyType = RigidbodyType2D.Static;
	}

	public void EnableDynamicBody() {
		rb.bodyType = RigidbodyType2D.Dynamic;
	}
}
