using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
	
	public float BaseSpeed = 700f;
	public float BaseKickPower = 2000f;
	
	private GameObject p1LeftSwimmer;
	private GameObject p1RightSwimmer;
	private GameObject p2LeftSwimmer;
	private GameObject p2RightSwimmer;
	private GameObject ball;
	private Vector3 direction = new Vector3(1f, 0f, 0f);
	
	// Use this for initialization
	void Start () {
		InitializeSwimmers();
	}
	
	void InitializeSwimmers() {
		p1LeftSwimmer = InitializeSwimmer ("P1" , "Left");
		p1RightSwimmer = InitializeSwimmer ("P1" , "Right");
		p2LeftSwimmer = InitializeSwimmer ("P2" , "Left");
		p2RightSwimmer = InitializeSwimmer ("P2" , "Right");
		ball = InitializeGameObject("Ball");
	}
	
	GameObject InitializeSwimmer (string playerIdentifier, string swimmerIdentifier) {
		var id = playerIdentifier + swimmerIdentifier;
		var swimmer = GameObject.Find (id + "Swimmer");
		
		if(swimmer == null) {
			Debug.Log (id + " swimmer does not exist in scene");
		}
		
		return swimmer;
	}
	
	GameObject InitializeGameObject (string name) {
		var obj = GameObject.Find (name);
		if(obj == null) {
			Debug.Log (name + " GameObject not found in scene");
		}
		return obj;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (direction);
		UpdateAllMovement ();	
		HandleShootBall ();
	}
	
	void HandleShootBall() {
		if(Input.GetAxis ("Shoot") <= 0f || ball.transform.parent == null) {
			return;
		}
		
		ball.transform.parent = null;
		ball.rigidbody.isKinematic = false;
		ball.rigidbody.detectCollisions = true;
		var force = direction * BaseKickPower;
		ball.rigidbody.AddForce (force);
	}
	
	void UpdateAllMovement () {
		UpdateMovement (p1LeftSwimmer, "P1", "Left");
		UpdateMovement (p1RightSwimmer, "P1", "Right");
		UpdateMovement (p2LeftSwimmer, "P2", "Left");
		UpdateMovement (p2RightSwimmer, "P2", "Right");
	}
	
	void UpdateMovement (GameObject swimmer, string playerIdentifier, string swimmerIdentifier) {
		if(swimmer == null) {
			return;
		}
		
		var id  = playerIdentifier + swimmerIdentifier;
		if(Input.GetAxis (id + "Horizontal") != 0f || Input.GetAxis(id + "Vertical") != 0f) {
			//Update velocity
			var directionalVector = new Vector3 (Input.GetAxis (id + "Horizontal"), 0f, Input.GetAxis  (id + "Vertical"));
			swimmer.rigidbody.velocity = directionalVector * BaseSpeed * Time.deltaTime;
			
			//Update rotation
			var ballAnchor = swimmer.transform.Find("BallAnchor");
			if(ballAnchor != null) {
				direction = directionalVector;
				var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (-Input.GetAxis (id + "Vertical"), Input.GetAxis (id + "Horizontal"));
				var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
				ballAnchor.rotation = newRotation;
			}
		}
	}
}
