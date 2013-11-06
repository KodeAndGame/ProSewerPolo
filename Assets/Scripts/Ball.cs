using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	private const string ValidPlayZoneTag = "Respawn";
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
		
	void OnTriggerExit (Collider collider) {
		if(collider.tag == ValidPlayZoneTag) {
			Reset ();
		}
	}
	
	public void Reset () {
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		transform.position = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		transform.parent = null;
	}
	
}