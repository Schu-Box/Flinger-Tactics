using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubstitutePlatform : MonoBehaviour {

	private MatchController matchController;

	private Vector3 subPlatformFullSize;
	private Vector3 subPlatformRestSize;

	private Vector3 subChairOrigin;
	private Vector3 subChairRest;
	private Vector3 subChairEnd;

	private Vector3 subPlatformUpperBound;
	private Vector3 subPlatformBackBound;

	private float maxSubYStretch;
	private float maxSubXStretch;

	private Transform startBar;
	private Transform attachmentBar;
	
	void Start() {
		matchController = FindObjectOfType<MatchController>();

		startBar = transform.GetChild(2);
		attachmentBar = transform.GetChild(3);
		
		subChairOrigin = transform.GetChild(4).position;
		subChairEnd = transform.GetChild(5).position;

		subChairRest = startBar.position;

		subPlatformFullSize = startBar.localScale;
		subPlatformRestSize = subPlatformFullSize;
		subPlatformRestSize.x = subPlatformFullSize.x;
		subPlatformRestSize.y = 0;

		subPlatformUpperBound = transform.GetChild(0).position;
		maxSubYStretch = subPlatformUpperBound.y;

		subPlatformBackBound = transform.GetChild(1).position;
		maxSubXStretch = subPlatformBackBound.x;

		startBar.localScale = subPlatformRestSize;

		DisplayAttachmentBar(false);
	}

	public void SetTeam(Team team) {
		startBar.GetComponent<SpriteRenderer>().color = team.secondaryColor;
		attachmentBar.GetComponent<SpriteRenderer>().color = team.secondaryColor;
	}

	public IEnumerator AnimateSubPlatformOpening() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.4f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			startBar.localScale = Vector3.Lerp(subPlatformRestSize, subPlatformFullSize, timer/duration);

			yield return waiter;
		}

		startBar.localScale = subPlatformFullSize;
	}

	public IEnumerator AnimateSubPlatformClosing() {
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		float duration = 0.2f;
		float timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			startBar.localScale = Vector3.Lerp(subPlatformFullSize, subPlatformRestSize, timer/duration);

			yield return waiter;
		}

		startBar.localScale = subPlatformRestSize;

		matchController.ClearSubChairsActive();

		DisplayAttachmentBar(false);
	}

	public Vector3 GetSubChairRest() {
		return subChairRest;
	}

	public Vector3 GetSubChairOrigin() {
		return subChairOrigin;
	}

	public Vector3 GetSubChairEnd() {
		return subChairEnd;
	}


	public Vector2 GetMaxStretch() {
		return new Vector2(maxSubXStretch, maxSubYStretch);
	}

	public void UpdateAttachmentBar(Vector3 subChairPosition) {
		DisplayAttachmentBar(true);

		Vector3 newPosition = attachmentBar.transform.position;
		newPosition.y = subChairPosition.y;
		attachmentBar.transform.position = newPosition;
	}

	public void DisplayAttachmentBar(bool setActive) {
		attachmentBar.gameObject.SetActive(setActive);
	}
}
