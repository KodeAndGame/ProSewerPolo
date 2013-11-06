using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	private const string ValidPlayZoneTag = "Respawn";
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
	
	#region Unity Event Handlers
	void OnTriggerExit (Collider collider) {
		if(collider.tag == ValidPlayZoneTag) {
			Reset ();
		}
	}
	#endregion
	
	#region Public Functions
	public void Reset () {
		//Handle Ball changes
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		transform.position = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		
		ReleaseBallFromHolder();
	}
	
	public void Shoot (Vector3 force) {	
		
		//Handle Ball changes
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		rigidbody.AddForce(force);
		
		//Handle Ball parent changes
		ReleaseBallFromHolder();	
	}
	#endregion
	
	#region Helpers
	void ReleaseBallFromHolder () {
		//Handle Ball holder changes
		if(transform.parent != null && transform.parent.parent != null && transform.parent.parent.tag == "Player") {
			var swimmerScript = transform.parent.parent.GetComponent("Swimmer") as Swimmer;
			swimmerScript.HandleBallRelease();
		}
		transform.parent = null;
	}
	#endregion
	
}