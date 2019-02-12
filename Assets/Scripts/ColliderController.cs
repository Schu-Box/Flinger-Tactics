using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum colliderType {
	Primary,
	Destructer,
	Trigger
};

public class ColliderController : MonoBehaviour {

	public colliderType type;

	private BlockerController blockerController;

	public Vector2 bumpDirection = new Vector2(1, 0);

	void Start() {
		blockerController = GetComponentInParent<BlockerController>();

		if(transform.rotation.eulerAngles.z == 180){
			bumpDirection = -bumpDirection;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		blockerController.ChildCollision(other, this);
	}
}
