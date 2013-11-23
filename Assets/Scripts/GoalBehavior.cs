using UnityEngine;
using System.Collections;

public class GoalBehavior : MonoBehaviour {
	
	public BallBehavior ballObject;
	public GUIBehavior scoreObject;
	public AudioClip AirHorn;
	
	void OnTriggerEnter (Collider collider) {
		if (collider.tag != "Ball")
			return; //don't do anything if it's not the ball

		// Increment variable to keep track of score
		scoreObject.AddPointToScore(this.tag == "RedGoal");
		
		AudioSource.PlayClipAtPoint(AirHorn, Camera.main.transform.position);
		
		//reset ball
		ballObject.Reset ();//reset ball
	}
}