using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {
	void OnBecameInvisible () {
		Reset ();
	}
	void Reset () {
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