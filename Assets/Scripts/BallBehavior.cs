using UnityEngine;
using System.Collections;

public class BallBehavior : MonoBehaviour {
	
	#region Constants
	private const string ValidPlayZoneTag = "Respawn";
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
	
	private RespawnBehavior respawn;	
	
	#region Unity Event Handlers
	void Start () {
		respawn = GetComponent("RespawnBehavior") as RespawnBehavior;
	}
	void OnTriggerExit (Collider collider) {
        if(collider.tag == ValidPlayZoneTag && !collider.GetComponent<BoxCollider>().bounds.Contains(transform.position)) {
            Reset (false);
        }
    }
	#endregion
	
	#region Public Functions and Properties
	public void Reset (bool isNeutral = true) {
		//Handle Ball changes
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		var newPosition = respawn.Respawn ();
		transform.position = GetNewPosition(isNeutral);
		rigidbody.velocity = Vector3.zero;
		
		ReleaseBallFromHolder();
		
		for(var x = 0; x < SwimmerBehavior.allSwimmers.Count; x++) {
			SwimmerBehavior.allSwimmers[x].Reset();
		}
	}

	public void Shoot (Vector3 force) {	
		//Handle Ball changes
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		rigidbody.AddForce(force);
		
		//Handle Ball parent changes
		ReleaseBallFromHolder();
	}
	
	public void Pickup (SwimmerBehavior swimmerScript) {
		var swimmer  = swimmerScript.gameObject;
		var ballAnchor = swimmer.transform.Find ("BallAnchor");
		transform.parent = ballAnchor;
		rigidbody.isKinematic = true;
		rigidbody.detectCollisions = false;
		
		transform.localPosition = new Vector3(1f, 0f, 0f);
		transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		
		swimmerScript.HandleBallPickup ();
	}
	
	public bool IsHeldByPlayer {
		get {
			return (transform.parent != null &&
					transform.parent.parent != null &&
					transform.parent.parent.gameObject.tag == "Player");
		}
	}
	#endregion
	
	#region Helpers
	void ReleaseBallFromHolder () {
		//Handle Ball holder changes
		if(IsHeldByPlayer) {
			var swimmerScript = transform.parent.parent.GetComponent("SwimmerBehavior") as SwimmerBehavior;
			swimmerScript.HandleBallRelease();
		}
		transform.parent = null;
	}
	#endregion
	
	private Vector3 GetNewPosition(bool isNeutral = true) {
		var type = RespawnType.Randomize;
		
		if (!isNeutral) {
			if(SwimmerBehavior.LastPossession == PlayerType.PlayerOne) {
				type = RespawnType.RandomizeRightOnly;
			}
			else if(SwimmerBehavior.LastPossession == PlayerType.PlayerOne) {
				type = RespawnType.RandomizeLeftOnly;
			}
		}
		
		var newPosition = respawn.Respawn(type);
		return new Vector3 (newPosition.x, 0f, newPosition.y);
	}
}