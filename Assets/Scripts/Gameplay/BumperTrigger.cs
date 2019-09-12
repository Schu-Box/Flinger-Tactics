using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperTrigger : MonoBehaviour {

	private Bumper parentBumper;

	void Start() {
		parentBumper = GetComponentInParent<Bumper>();
	}

	private void OnTriggerEnter2D(Collider2D collie) {
		if(collie.gameObject.CompareTag("Athlete")) {
			collie.gameObject.GetComponent<AthleteController>().EnteredBumper(parentBumper);
		} else if(collie.gameObject.CompareTag("Ball")) { //Ball entered bumper zone
			collie.gameObject.GetComponent<BallController>().SetLastBumper(parentBumper);
		}
	}

	private void OnTriggerExit2D(Collider2D collie) {
		if(collie.gameObject.CompareTag("Athlete")) {
			collie.gameObject.GetComponent<AthleteController>().ExitedBumper(parentBumper);
		}
	}

	
}
