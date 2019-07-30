using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteManager : MonoBehaviour {

	public List<string> preMatchTeammateQuotes = new List<string>();
	public List<string> preMatchOpponentQuotes = new List<string>();

	public List<string> preFlingTeammateQuotes = new List<string>();
	public List<string> preFlingOpponentQuotes = new List<string>();

	MatchController matchController;

	void Start() {
		matchController = FindObjectOfType<MatchController>();

		SetPreMatchQuotes();
		SetPreFlingQuotes();
	}

	public void SetPreMatchQuotes() {
		preMatchTeammateQuotes.AddRange(new List<string> {
			/* Keep these */
			"Game time!", "Let's play!", "I'm ready!", "We're ready, coach!", "Let's do this!", "Ready to play, coach!",
			

			//On the block
			"Let's go, boss.", "You ready, coach?",
			
			/* Keep these, personality-specific */
			"It's game time babbbyyy!", 

			//On the block
			"I'm the best flinger in the land.", "Hit begin so we can start, coach!", "Can we start already?",
			"Time is money and today is pay day!", "Let's get this cheddar.", "We're getting paid for this, right?",
			"Another day, another golden.", "Haha we're playing these guys?", "Who are we playing today?",
			"We have to play them!?", "OOOGGGAAGAAA!", "I'm bout to do it to em.", "Time to go fling mode",
			"I'm a little nervous today.", "Let's hope this goes well.",
			
			/* Situational/Status-Based */
			"Coach I think I'm sick today."
		});

		preMatchOpponentQuotes.AddRange(new List<string> {
			"We have nothing to discuss.", "Go check your side.", "We're not friends.", "I'm not your friend.",
			"You scared yet?", "Back up off me!", "You're bout to lose.", "You're gonna lose.", "You aren't winning this!",
			"Prepare for defeat!",

			"Please don't bother me.",

			"Hahaha you're the coach?"
		});
	}

	public void SetPreFlingQuotes() {
		preFlingTeammateQuotes.AddRange(new List<string> {
			/* Keepers */
			"It's fling time!", "Let's do this!", "Yes! My time to shine.", "Let's get it!", "I can do this!", 
			"I gotchu, coach!", "You can trust me!", "Fling me!", "Fling time!", "Yipee! Fling time!", 
			"Coach I can do this!", "My turn to fling!", "...",

			/* Personality-Specific */
			"Just fling me already!", "I'm bout to fling on em.", "Lemme fling!", "Fling fling!", "Flingy time!",
			"Aim for that stoopy-head over there!", "I can bump that chump!", "Let me at em!", " *growls extra ferociously* ",
			"I wanna score!", "I'm gonna score!", "Fling city, baby!", "Yup!", "Weeehaaa!", "Ya ya hey!"
		});

		preFlingOpponentQuotes.AddRange(new List<string> {
			"We're not friends.", "I'm not your friend.", "You're gonna mess up!", "Shut up.", "Don't talk to me.",
			"Go away.", "Don't mess up!", 
			
			"Hey we're not on the same team, sorry.", 

			"Don't mess up, chump!", "You don't even know me!", "Bet you wish I was on your squad.", "You can't fling me!",
			"We're better flingers.", "Stop looking at my stats!", "What's the score?"
		});

	}
	
	public string GetRandomQuote(List<string> quotes) {
		return quotes[Random.Range(0, quotes.Count)];
	}

	public string GetPreMatchQuote(Athlete athlete) {
		List<string> possibleQuotes = new List<string>();

		if(!athlete.GetTeam().computerControlled) {
			possibleQuotes.AddRange(preMatchTeammateQuotes);
		} else {
			possibleQuotes.AddRange(preMatchOpponentQuotes);
		}

		return GetRandomQuote(possibleQuotes);
	}

	public string GetPreFlingQuote(Athlete athlete) {
		List<string> possibleQuotes = new List<string>();

		if(athlete.GetTeam() == matchController.GetTurnTeam()) {
			possibleQuotes.AddRange(preFlingTeammateQuotes);
		} else {
			possibleQuotes.AddRange(preFlingOpponentQuotes);
		}

		return GetRandomQuote(possibleQuotes);
	}
}
