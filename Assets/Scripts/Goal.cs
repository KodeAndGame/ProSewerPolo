using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour {
	
	public Ball ballObject;
	
	void OnTriggerEnter (Collider collider) {
		if (collider.tag != "Ball")
			return; //don't do anything if it's not the ball

		// Increment variable to keep track of score
		if (this.tag == "BlueGoal")
			{}//increment BlueScore
		
			
		else if (this.tag == "RedGoal")
			{}//increment RedScore
		
		//reset ball
		ballObject.Reset ();
	}
}

//function Start () {

//}

//function Update () {

//}