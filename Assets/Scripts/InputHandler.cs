using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
	
	public float BaseSpeed = 50.0f;
	
	private GameObject P1LeftSwimmer;
	private GameObject P1RightSwimmer;
	private GameObject P2LeftSwimmer;
	private GameObject P2RightSwimmer;
	
	// Use this for initialization
	void Start () {
		P1LeftSwimmer = GameObject.Find("P1LeftSwimmer");
		P1RightSwimmer = GameObject.Find("P1RightSwimmer");
		P2LeftSwimmer = GameObject.Find("P2LeftSwimmer");
		P2RightSwimmer = GameObject.Find("P2RightSwimmer");
		
		if(P1LeftSwimmer == null) {Debug.Log("P1 Left does not exist in scene");}
		if(P1RightSwimmer == null) {Debug.Log("P1 Right does not exist in scene");}
		if(P2LeftSwimmer == null) {Debug.Log("P2 Left does not exist in scene");}
		if(P2RightSwimmer == null) {Debug.Log("P2 Right does not exist in scene");}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateAllMovement ();
	}
	
	void UpdateAllMovement () {
		UpdateMovement (P1LeftSwimmer, "P1", "Left");
		UpdateMovement (P1RightSwimmer, "P1", "Right");
		UpdateMovement (P2LeftSwimmer, "P2", "Left");
		UpdateMovement (P2RightSwimmer, "P2", "Right");
	}
	
	void UpdateMovement (GameObject swimmer, string playerIdentifier, string swimmerIdentifier) {
		if(swimmer == null) {
			return;
		}
		
		var id  = playerIdentifier + swimmerIdentifier;
		var vector = new Vector3 (
						Input.GetAxis (id + "Horizontal"),
						0f,
						Input.GetAxis  (id + "Vertical"));
		swimmer.rigidbody.velocity = vector * BaseSpeed * Time.deltaTime;
	}
}
