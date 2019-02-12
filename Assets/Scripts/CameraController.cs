using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Vector3 startPosition;
	public Vector3 upPosition;
	public Vector3 upLeftPosition;
	public Vector3 upRightPosition;
	public Vector3 downPosition;

	private GameObject objectToFollow;

	private float fullSize;
	public float zoomSize;

	private Camera cam;

	void Start() {
		cam = GetComponent<Camera>();
		fullSize = cam.orthographicSize;
	}

	void Update() {
		if(objectToFollow != null) {
			Vector3 newPos = objectToFollow.transform.position;
			newPos.z = transform.position.z;
			transform.position = newPos;
		}
	}

	public void FollowObject(GameObject obj) {
			cam.orthographicSize = zoomSize;

			objectToFollow = obj;
	}
}
