using UnityEngine;
using System.Collections;

public class GoalBehavior : MonoBehaviour {
	
	public BallBehavior ballObject;
	
	void OnTriggerEnter (Collider collider) {
		if (collider.tag != "Ball")
			return; //don't do anything if it's not the ball

		// Increment variable to keep track of score
		if (this.tag == "BlueGoal")
			{}//increment RedScore
		else if (this.tag == "RedGoal")
			{}//increment BlueScore
		
		//reset ball
		ballObject.Reset ();
	}
}