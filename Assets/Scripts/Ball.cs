using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	
	#region Constants
	private const string ValidPlayZoneTag = "Respawn";
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
	
	#region Public Functions and Properties
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
	
	public void Pickup (Swimmer swimmerScript) {
	
		var swimmer  = swimmerScript.gameObject;
		var ballAnchor = swimmer.transform.Find ("BallAnchor");
		transform.parent = ballAnchor;
		rigidbody.isKinematic = true;
		transform.localPosition = new Vector3(1f, 0f, 0f);
		transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		
		swimmerScript.HandleBallPickup ();
	}
	
	public bool IsHeldByPlayer {
		get {
			return (transform.parent != null && transform.parent.parent != null && transform.parent.parent.gameObject.tag == "Player");
		}
	}
	#endregion
	
	#region Helpers
	void ReleaseBallFromHolder () {
		//Handle Ball holder changes
		if(IsHeldByPlayer) {
			var swimmerScript = transform.parent.parent.GetComponent("Swimmer") as Swimmer;
			swimmerScript.HandleBallRelease();
		}
		transform.parent = null;
	}
	#endregion
	
}