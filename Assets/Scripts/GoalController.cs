using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {

	public Team goalAttacker;

	public BlockerController blocker;

	private GameController gameController;
	private Collider2D trigger;

	private bool ballScoredHere;

	void Start() {
		gameController = FindObjectOfType<GameController>();
		trigger = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(other.gameObject.CompareTag("Ball")) { //Ball entered goal zone
			BallController ball = other.GetComponent<BallController>();

			if(ball.GetScoredByTeam() == null) {
				ballScoredHere = true;
				gameController.ScoreGoal(goalAttacker, ball);
			}

		} else if(other.gameObject.CompareTag("Athlete")) { //Athlete entered goal zone

		}
	}

	public bool GetBallEntered() {
		return ballScoredHere;
	}

	public void SetBallEntered(bool entered) {
		ballScoredHere = entered;
	}

	public void SetTriggerState(bool active) {
		trigger.enabled = active;
	}
}
