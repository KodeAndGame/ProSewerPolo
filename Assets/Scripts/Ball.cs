using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	private const string ValidPlayZoneTag = "Respawn";
		
	void OnTriggerExit (Collider collider) {
		if(collider.tag == ValidPlayZoneTag) {
			Reset ();
		}
	}
	
	void Reset () {
		rigidbody.isKinematic = false;
		rigidbody.detectCollisions = true;
		transform.position = Vector3.zero;
		rigidbody.velocity = Vector3.zero;
		transform.parent = null;
	}
}