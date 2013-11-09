using UnityEngine;
using System.Collections;

public class GoalBehavior : MonoBehaviour {
	
	public BallBehavior ballObject;
	public GUIBehavior scoreObject;
	
	void OnTriggerEnter (Collider collider) {
		if (collider.tag != "Ball")
			return; //don't do anything if it's not the ball

		// Increment variable to keep track of score
		if (this.tag == "BlueGoal")
			++ scoreObject.redScore;//increment RedScore
		
		else if (this.tag == "RedGoal")
			++ scoreObject.blueScore;//increment BlueScore
		
		//reset ball
		ballObject.Reset ();
	}
}