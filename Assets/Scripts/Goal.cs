using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	
	public Ball ballObject;
	public Score scoreObject;
	
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