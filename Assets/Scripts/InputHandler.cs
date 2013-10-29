using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {
	
	public float BaseSpeed = 700.0f;
	
	private GameObject P1LeftSwimmer;
	private GameObject P1RightSwimmer;
	private GameObject P2LeftSwimmer;
	private GameObject P2RightSwimmer;
	
	// Use this for initialization
	void Start () {
		InitializeSwimmers();
	}
	
	void InitializeSwimmers() {
		P1LeftSwimmer = InitializeSwimmer ("P1" , "Left");
		P1RightSwimmer = InitializeSwimmer ("P1" , "Right");
		P2LeftSwimmer = InitializeSwimmer ("P2" , "Left");
		P2RightSwimmer = InitializeSwimmer ("P2" , "Right");
	}
	
	GameObject InitializeSwimmer (string playerIdentifier, string swimmerIdentifier) {
		var id = playerIdentifier + swimmerIdentifier;
		var swimmer = GameObject.Find (id + "Swimmer");
		
		if(swimmer == null) {
			Debug.Log (id + " swimmer does not exist in scene");
		}
		
		return swimmer;
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
