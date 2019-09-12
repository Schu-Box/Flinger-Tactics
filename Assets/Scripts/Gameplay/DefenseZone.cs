using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseZone : MonoBehaviour {

	private Team team;

	public void SetTeam(Team t) {
		team = t;
	}

	private void OnTriggerExit2D(Collider2D c) {
		if(c.gameObject.tag == "Ball") {
			BallController ball = c.gameObject.GetComponent<BallController>();
			if(ball.GetScoredByTeam() == null) {
				AthleteController lastToucher = ball.GetLastToucher();
				if(lastToucher != null) {
					if(lastToucher.GetAthlete().GetTeam() == team) {
						lastToucher.IncreaseStat(StatType.Clears);
					}
				}
			}
		}
	}
}
