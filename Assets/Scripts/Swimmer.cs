using UnityEngine;
using System.Collections;

public class Swimmer : MonoBehaviour {
	
	#region Constants
	private const int PlayerIdentifierLength = 2;
	private const int LengthOfNameWithoutSwimmerId = 9;
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
	
	#region Public Members
	public float BaseSpeed = 1000f;
	public float BaseShootPower = 2300f;
	#endregion
	
	#region Private Members
	private string playerId;
	private string swimmerId;
	private Vector3 heading = new Vector3(1f, 0f, 0f);
	private GameObject ball;
	private bool isTouchingBall = false;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		ball = GameObject.Find ("Ball");
		playerId = name.Substring(0, PlayerIdentifierLength);
		swimmerId = name.Substring (PlayerIdentifierLength, name.Length - LengthOfNameWithoutSwimmerId);
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement();
		UpdateShoot ();
	}
	
	//Called when collision occurs
	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Ball") {
			
			isTouchingBall = true;
			
			if(collision.transform.parent == null) {
				gameObject.layer = PlayerHoldingBallLayer;
				var ballAnchor = transform.Find("BallAnchor");
				collision.rigidbody.velocity = Vector3.zero;
				collision.rigidbody.isKinematic = true;
				collision.transform.parent = ballAnchor;
				collision.transform.localPosition = new Vector3(1f, 0f, 0f);
				collision.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
	}
	
	//Called when collision case ceases
	void OnCollisionExit(Collision collision) {
		if (collision.gameObject.tag == "Ball") {
				isTouchingBall = false;
		}
	}
	#endregion
	
	void UpdateMovement () {
		//Get initial data
		var id  = playerId + swimmerId;
		
		//Update velocity
		var userHeading = new Vector3 (Input.GetAxis (id + "Horizontal"), 0f, Input.GetAxis  (id + "Vertical"));
		rigidbody.velocity = userHeading * BaseSpeed * Time.deltaTime;
		
		//Update direction swimmer is facing in	(don't update if neither axis is active)
		if(Input.GetAxis (id + "Horizontal") != 0f || Input.GetAxis(id + "Vertical") != 0f) {
			var ballAnchor = transform.Find("BallAnchor");
			if(ballAnchor != null) {
				heading = userHeading.normalized;
				var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (-Input.GetAxis (id + "Vertical"), Input.GetAxis (id + "Horizontal"));
				var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
				ballAnchor.rotation = newRotation;
			}
		}
	}
	
	void UpdateShoot () {
		if(ball.transform.parent != null && ball.transform.parent.parent != null) {
			var shootAxisName = playerId + "Shoot";
			if(Input.GetAxis (shootAxisName) > 0f && (ball.transform.parent.parent == transform || isTouchingBall)) {
				ball.transform.parent = null;
				ball.rigidbody.isKinematic = false;
				ball.rigidbody.detectCollisions = true;
				var force = heading * BaseShootPower;
				ball.rigidbody.AddForce (force);
				gameObject.layer = PlayerLayer;
			}
		}
	}
}
