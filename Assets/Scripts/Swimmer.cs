using UnityEngine;
using System.Collections;

public class Swimmer : MonoBehaviour {
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
	
	#region Public Members
	public float BaseSpeed = 1000f;
	public float BaseShootPower = 2300f;
	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string ShootAxisName;
	#endregion
	
	#region Private Members
	private Vector3 heading = new Vector3(1f, 0f, 0f);
	private GameObject ball;
	private bool isTouchingBall = false;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		AssertValidAxisNames ();
		ball = GameObject.Find ("Ball");
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
	
	void AssertValidAxisNames() {
		if(string.IsNullOrEmpty(HorizontalAxisName)) {
			Debug.LogError("Horizontal Axis Name is missing");
		}
		if(string.IsNullOrEmpty(VerticalAxisName)) {
			Debug.LogError("Vertical Axis Name is missing");
		}
		if(string.IsNullOrEmpty(ShootAxisName)) {
			Debug.LogError("Shoot Axis Name is missing");
		}
	}
	void UpdateMovement () {
		var horizontalInput = Input.GetAxis (HorizontalAxisName);
		var verticalInput = Input.GetAxis (VerticalAxisName);
		
		//Update velocity
		var userHeading = new Vector3 (horizontalInput, 0f, verticalInput);
		rigidbody.velocity = userHeading * BaseSpeed * Time.deltaTime;
		
		//Update direction swimmer is facing in	(don't update if neither axis is active)
		if(horizontalInput != 0f || verticalInput != 0f) {
			var ballAnchor = transform.Find("BallAnchor");
			if(ballAnchor != null) {
				heading = userHeading.normalized;
				var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (-verticalInput, horizontalInput);
				var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
				ballAnchor.rotation = newRotation;
			}
		}
	}
	void UpdateShoot () {
		if(ball.transform.parent != null && ball.transform.parent.parent != null) {
			if(Input.GetAxis (ShootAxisName) > 0f && (ball.transform.parent.parent == transform || isTouchingBall)) {
				var ballHolder = ball.transform.parent.parent;
				ballHolder.gameObject.layer = PlayerLayer;
				ball.transform.parent = null;
				ball.rigidbody.isKinematic = false;
				ball.rigidbody.detectCollisions = true;
				var force = heading * BaseShootPower;
				ball.rigidbody.AddForce (force);
			}
		}
	}
}
