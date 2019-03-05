using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour {

	private GoalController parentGoal;

	void Start() {
		parentGoal = transform.parent.GetComponent<GoalController>();
	}

	private void OnTriggerEnter2D(Collider2D collie) {
		Debug.Log("Entered Trigger");
		
		if(collie.gameObject.CompareTag("Ball")) {
			BallController ball = collie.gameObject.GetComponent<BallController>();
			
			StartCoroutine(WaitForImpact(ball));
		}
	}

	private IEnumerator WaitForImpact(BallController ball) {

		yield return new WaitForSeconds(0.04f);

		parentGoal.BallEnteredTrigger(ball);
	}
}
