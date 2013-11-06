using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	private const string ValidPlayZoneTag = "Respawn";
		
	void OnTriggerExit (Collider collider) {
		if(collider.tag == ValidPlayZoneTag) {
			Reset ();
		}
	}
	public void Reset () {
		if(gameObject.active) {
			rigidbody.isKinematic = false;
			rigidbody.detectCollisions = true;
			transform.position = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
			if(transform.parent != null && transform.parent.gameObject.active && transform.parent.parent.gameObject.active) {
				transform.parent = null;
			}
		}
	}
}