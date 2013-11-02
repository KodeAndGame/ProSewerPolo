using UnityEngine;
using System.Collections;

public class DebugScripts : MonoBehaviour {
	
	private GameObject ball;
	
	// Use this for initialization
	void Start () {
		ball = InitializeGameObject("Ball");
	}
	
	// Update is called once per frame
	void Update () {
		enabled = Debug.isDebugBuild;
		
		if(Input.GetKeyDown(KeyCode.R)) {
			ResetBall();
		}
	}
	
	void ResetBall () {
		ball.transform.position = Vector3.zero;
		ball.rigidbody.velocity = Vector3.zero;
		ball.transform.parent = null;
		ball.rigidbody.isKinematic = false;
		ball.rigidbody.detectCollisions = true;
	}
	
	GameObject InitializeGameObject (string name) {
		var obj = GameObject.Find (name);
		if(obj == null) {
			Debug.Log (name + " GameObject not found in scene");
		}
		return obj;
	}
}
