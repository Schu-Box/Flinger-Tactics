using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailBody : MonoBehaviour {

	private SpriteRenderer spriteRenderer;

	public void SetTailBody() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}
}
