using UnityEngine;
using System.Collections;

public class BallHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Player" && transform.parent == null) {		
			var ballAnchor = collision.gameObject.transform.Find("BallAnchor");
			if(ballAnchor == null) {
				Debug.Log ("No Ball Anchor exists on " + collision.gameObject.name);
				return;
			}
			rigidbody.detectCollisions = false;
			//rigidbody.velocity = Vector3.zero;
			
			transform.parent = ballAnchor;
			transform.localPosition = new Vector3(1f, 0f, 0f);
			transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		}
	}
}
