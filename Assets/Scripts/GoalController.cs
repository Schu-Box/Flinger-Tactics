using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalController : MonoBehaviour {

	private Team goalAttacker;
	private Team teamOwner;
	
	private MatchController matchController;
	private CameraController cameraController;
	private SpriteRenderer spriteRenderer;

	private Bumper bumper;
	private SubstitutePlatform subPlatform;

	void Start() {
		matchController = FindObjectOfType<MatchController>();
		cameraController = FindObjectOfType<CameraController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		bumper = GetComponent<Bumper>();

		subPlatform = transform.GetChild(1).GetComponent<SubstitutePlatform>();

		bumper.SetAsGoal(true);
	}

	public void OpenSubPlatform() {
		StartCoroutine(subPlatform.AnimateSubPlatformOpening());
	}
	

	public void CloseSubPlatform() {
		StartCoroutine(subPlatform.AnimateSubPlatformClosing());

		List<SubstituteChair> subChairList = matchController.GetSubChairsActive();
		for(int i = 0; i < subChairList.Count; i++) {
			subChairList[i].SetInteractable(false);
		}
	}

	public void SetTeamRelations(Team owner, Team attacker) {
		teamOwner = owner;
		goalAttacker = attacker;

		subPlatform.SetTeam(owner);
	}

	public Team GetGoalAttacker() {
		return goalAttacker;
	}

	/*
	public Team GetTeamOwner() {
		return teamOwner;
	}

	public Team GetAttacker() {
		return goalAttacker;
	}
	*/
	
	/*
	private void OnCollisionEnter2D(Collision2D collie) {
		if(collie.gameObject.CompareTag("Ball")) {
			BallController ball = collie.gameObject.GetComponent<BallController>();
			matchController.ScoreGoal(goalAttacker, ball);

			StartCoroutine(GoalFlash());
		}
	}
	*/

	public void BallEnteredTrigger(BallController ball) {
		matchController.ScoreGoal(goalAttacker, ball, transform.position);

		StartCoroutine(GoalFlash());
	}

	
	public void AthleteEnteredTrigger(AthleteController ac) {
		matchController.SubstituteAthleteOut(ac, transform.position);
	}

	public IEnumerator GoalFlash() {

		spriteRenderer.color = goalAttacker.primaryColor;

		yield return new WaitForSeconds(0.4f);

		float duration = 0.5f;
		float timer = 0f;
		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(timer < duration) {
			timer += Time.deltaTime;

			spriteRenderer.color = Color.Lerp(goalAttacker.primaryColor, teamOwner.primaryColor, timer/duration);

			yield return waiter;
		}

		/*
		duration = 0.4f;
		timer = 0f;
		while(timer < duration) {
			timer += Time.deltaTime;

			spriteRenderer.color = Color.Lerp(teamOwner.GetLightTint(), teamOwner.primaryColor, timer/duration);

			yield return waiter;
		}
		*/

		spriteRenderer.color = teamOwner.primaryColor;
	}

	public SubstitutePlatform GetSubPlatform() {
		return subPlatform;
	}
}
