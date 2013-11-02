using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Swimmers : MonoBehaviour {
	
	#region Public Members
	public float BaseSpeed = 1000f;
	public float BaseKickPower = 2300f;
	#endregion	
	
	#region Private Members
	delegate void SwimmerFunction(int index);
	private string [] swimmerPrefixes = {"P1Left", "P1Right", "P2Left", "P2Right"};
	private string [] swimmerNames = {"P1LeftSwimmer", "P1RightSwimmer", "P2LeftSwimmer", "P2RightSwimmer"};
	private GameObject [] swimmers = new GameObject[4];
	private Vector3 [] swimmerHeadings = new Vector3[4];
	private GameObject ball;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		//Initialize critical GameObjects
		ForEachSwimmer(InitializeSwimmer);
		ball = InitializeGameObject("Ball");
		
		//Initialize supporting data
		ForEachSwimmer(InitializeCurrentDirection);		
	}
	
	// Update is called once per frame
	void Update () {
		ForEachSwimmer (UpdateMovement);
		PollForReset ();
	}
	#endregion
	
	#region Initialization Functions
	void InitializeSwimmer (int i) {
		var swimmer = InitializeGameObject(swimmerNames[i]);
		swimmers[i] = swimmer;
	}
	void InitializeCurrentDirection (int i) {
		var heading = swimmers[i].transform.position - ball.transform.position;
		swimmerHeadings[i] = heading.normalized;
	}
	#endregion
	
	#region Update Functions
	void PollForReset() {
		if(Input.GetKeyDown(KeyCode.R)) {
			ball.transform.position = new Vector3(0f, 0f, 0f);
			ball.transform.parent = null;
			ball.rigidbody.isKinematic = false;
			ball.rigidbody.detectCollisions = true;
			ball.rigidbody.velocity = Vector3.zero;
		}
	}	
	void UpdateMovement (int i) {
		//Get initial data
		var id  = swimmerPrefixes[i];
		var swimmer = swimmers[i];
		
		if(swimmer == null) {
			return;
		}		
		
		//Update velocity
		var userHeading = new Vector3 (Input.GetAxis (id + "Horizontal"), 0f, Input.GetAxis  (id + "Vertical"));
		swimmer.rigidbody.velocity = userHeading * BaseSpeed * Time.deltaTime;
		
		//Update direction swimmer is facing in	(don't update if neither axis is active)
		if(Input.GetAxis (id + "Horizontal") != 0f || Input.GetAxis(id + "Vertical") != 0f) {
			var ballAnchor = swimmer.transform.Find("BallAnchor");
			if(ballAnchor != null) {
				swimmerHeadings[i] = userHeading.normalized;
				var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (-Input.GetAxis (id + "Vertical"), Input.GetAxis (id + "Horizontal"));
				var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
				ballAnchor.rotation = newRotation;
			}
		}
	}
	#endregion
	
	#region Swimmer Helper Functions
	void ForEachSwimmer(SwimmerFunction swimmerFunc) {
		for(int i = 0; i < swimmerNames.Length; i++) {
			swimmerFunc(i);
		}
	}
	#endregion
	
	//TODO: Move these
	#region Generic Helper Functions
	GameObject InitializeGameObject (string name) {
		var obj = GameObject.Find (name);
		if(obj == null) {
			Debug.Log (name + " GameObject not found in scene");
		}
		return obj;
	}
	#endregion
}
