using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuoteManager : MonoBehaviour {

	/*
	public List<string> preMatchTeammateQuotes = new List<string>();
	public List<string> preMatchOpponentQuotes = new List<string>();

	public List<string> preFlingTeammateQuotes = new List<string>();
	public List<string> preFlingOpponentQuotes = new List<string>();

	public List<string> goalQuotes = new List<string>();
	public List<string> ownGoalQuotes = new List<string>();


	public List<string> wonQuotes = new List<string>();
	public List<string> lostQuotes = new List<string>();
	*/

	MatchController matchController;

	void Start() {
		matchController = FindObjectOfType<MatchController>();

		/*
		SetPreMatchQuotes();
		SetPreFlingQuotes();
		SetGoalQuotes();
		SetPostMatchQuotes();
		*/
	}

	/*
	public void SetPreMatchQuotes() {
		preMatchTeammateQuotes.AddRange(new List<string> {
			"Game time!", "Let's play!", "I'm ready!", "We're ready, coach!", "Let's do this!", "Ready to play, coach!",
			

			//On the block
			"Let's go, boss.", "You ready, coach?",
			
			"It's game time babbbyyy!", 

			//On the block
			"I'm the best flinger in the land.", "Hit begin so we can start, coach!", "Can we start already?",
			"Time is money and today is pay day!", "Let's get this cheddar.", "We're getting paid for this, right?",
			"Another day, another golden.", "Haha we're playing these guys?", "Who are we playing today?",
			"We have to play them!?", "OOOGGGAAGAAA!", "I'm bout to do it to em.", "Time to go fling mode",
			"I'm a little nervous today.", "Let's hope this goes well.",
			
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
			
			"It's fling time!", "Let's do this!", "Yes! My time to shine.", "Let's get it!", "I can do this!", 
			"I gotchu, coach!", "You can trust me!", "Fling me!", "Fling time!", "Yipee! Fling time!", 
			"Coach I can do this!", "My turn to fling!", "...",

			
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

	public void SetGoalQuotes() {
		goalQuotes.AddRange(new List<string> {
			"Yesss!", "Haha I scored!", "Yes! I scored!", "Woah I did it!", "I scored!!!", "We did it, coach!",
			"Wahaa! Let's do that again!", "Goooaaaalllll!!", "I knew I could score!", "I knew I could do it!",
			"I did a goal!",

			"That's what I do!", "That's what I do babbbyyyy!", "Did you see that!?", " imdabes ", "Gimme dat!",
			"It's too easy baby!", "GG 2 EZ", "That's all day!"
		});

		ownGoalQuotes.AddRange(new List<string> {
			"Whoopsies!", "Ohhh noooo!", "Oh... My bad.", "Uh oh. Not my best.", "I swear that's an accident!",
			"No no no no!", "Oh that's the wrong goal, huh?", "I meant to score on the other goal...",
			"Ugh it's not my day...",
			
			"Not my fault!", "Arrrrgghhh why'd you make me do that?!", "Wait is that the wrong goal?"
		});
	}

	public void SetPostMatchQuotes() {
		wonQuotes.AddRange(new List<string> {
			"Victory!", "Haha we won!", "Victory is so sweet!", "I love winning!", "Great work, team!", 
			"We did it everyone!", "Yaaayyyy!!!", "Hard work really does pay off!",

			"Haha! You're losers we're winners!",
			
			"Another day another dub!", "That's what we do, baby!", "We did it, coach! We won!"
		});

		lostQuotes.AddRange(new List<string> {
			"Ugh.....", "I hate losing...", "How'd we lose?", "Dang I thought we had this one...", "We lost...",
			"Let's just go home...", "Can we just go home?", "We need to be better.", "Time to hit the gym.",

			"Next time we'll have them!",
			
			"We losted? Again?",

			"Ugh they got so lucky!", "You're lucky I'm sick!", "We'll try for real next time!"
		});
	}
	*/

	/*
	public string GetQuote(Athlete athlete, string id) {
		List<string> possibleQuotes = new List<string>();

		switch(id) {
			case "goal":
				possibleQuotes = goalQuotes;
				break;
			case "ownGoal":
				possibleQuotes = ownGoalQuotes;
				break;
			case "preMatch":
				if(!athlete.GetTeam().computerControlled) {
					possibleQuotes.AddRange(preMatchTeammateQuotes);
				} else {
					possibleQuotes.AddRange(preMatchOpponentQuotes);
				}
				break;
			case "preFling":
				if(athlete.GetTeam() == matchController.GetTurnTeam()) {
					possibleQuotes.AddRange(preFlingTeammateQuotes);
				} else {
					possibleQuotes.AddRange(preFlingOpponentQuotes);
				}
				break;
			case "postMatch":
				if(matchController.GetMatchData().winner == athlete.GetTeam()) {
					possibleQuotes.AddRange(wonQuotes);
				} else {
					possibleQuotes.AddRange(lostQuotes);
				}
				break;
			default:
				possibleQuotes = new List<string> { "<QUOTE HERE>" };
				break;
		}

		return GetRandomQuote(possibleQuotes);
	}
	
	public string GetRandomQuote(List<string> quotes) {
		return quotes[Random.Range(0, quotes.Count)];
	}
	*/
}
