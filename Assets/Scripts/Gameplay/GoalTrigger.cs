using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour {

	private GoalController parentGoal;

	private MatchController matchController;

	void Start() {
		parentGoal = transform.parent.GetComponent<GoalController>();

		matchController = FindObjectOfType<MatchController>();
	}

	private void OnTriggerEnter2D(Collider2D collie) {
		if(collie.gameObject.CompareTag("Ball")) {
			BallController ball = collie.gameObject.GetComponent<BallController>();
			
			StartCoroutine(WaitForBallImpact(ball));
		} else if(collie.gameObject.CompareTag("Athlete")) {
			AthleteController ac = collie.gameObject.GetComponent<AthleteController>();

			if(ac.GetAthlete().GetTeam() == parentGoal.GetGoalAttacker()) { //If this athlete is an attacker of this goal
				StartCoroutine(WaitForAthleteImpact(ac));
			}
		}
	}

	private IEnumerator WaitForBallImpact(BallController ball) {
		yield return new WaitForSeconds(0.03f);

		parentGoal.BallEnteredTrigger(ball);
	}

	private IEnumerator WaitForAthleteImpact(AthleteController ac) {
		yield return new WaitForSeconds(0.03f);

		parentGoal.AthleteEnteredTrigger(ac);
	}
}
