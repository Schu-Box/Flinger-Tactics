using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {

	private float bumpForce = 0.2f;

	public Vector2 bumpDirection = new Vector2(0, -1);

	void OnCollisionEnter2D(Collision2D collision) {
		Rigidbody2D otherRB = collision.gameObject.GetComponent<Rigidbody2D>();
	
		otherRB.AddForce(bumpDirection * bumpForce, ForceMode2D.Impulse);
	}
}
