﻿using System.Collections;
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
	private float trauma = 0f;
    private Coroutine screenShake;

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

	 public void AddTrauma(float traumaAdded) {
        trauma += traumaAdded;
        trauma = Mathf.Clamp01(trauma); //Clamps the trauma value between 0 and 1

        if(screenShake == null) {
            screenShake = StartCoroutine(ShakeScreen());
        }
    }

    public IEnumerator ShakeScreen() {
        //Debug.Log("Shaking Screen");

        Vector3 originalPosition = Camera.main.transform.localPosition;

        float decayRate = 0.02f;

        while(trauma > 0) {
            float shake = Mathf.Pow(trauma, 2);

            float maxAngleOffset = 3.5f;
            float maxOffset = 0.2f;

            float angle = maxAngleOffset * shake * ((Random.value * 2) - 1f);
            float x = maxOffset * shake * ((Random.value * 2) - 1f);
            float y = maxOffset * shake * ((Random.value * 2) - 1f);

            transform.localPosition = new Vector3(x, y, 0) + originalPosition;
            transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, angle);

            trauma -= decayRate;

            yield return new WaitForFixedUpdate();
        }
        //Debug.Log("Shake Complete");

        transform.localPosition = originalPosition;

        screenShake = null;
    }
}