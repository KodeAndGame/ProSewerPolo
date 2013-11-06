using UnityEngine;
using System.Collections;

public class Swimmer : MonoBehaviour {
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	private const float PossessSize = 0.5f;
	private const float LackingSize = 1f;
	#endregion
	
	#region Public Members
	public float BaseSpeed = 1000f;
	public float BaseShootPower = 2300f;
	public float CatchZoneSize = 1f;
	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string ShootAxisName;
	public Swimmer teammate;
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
		UpdateCatchSize ();
	}
	
	//Called when something enters the catch zone
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Ball") {
			
			isTouchingBall = true;			
			
			if(other.transform.parent == null || other.transform.parent.tag != "Player") {
				
				//Caught the ball, so reduce catch size for team
				CatchZoneSize = teammate.CatchZoneSize = PossessSize;
				
				gameObject.layer = PlayerHoldingBallLayer;
				var ballAnchor = transform.Find ("BallAnchor");
				other.transform.parent = ballAnchor;
				other.rigidbody.isKinematic = true;
 				other.transform.localPosition = new Vector3(1f, 0f, 0f);
 				other.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
			}
		}
	}
	
	//Called when trigger case ceases
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Ball") {
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
		
		//Update direction swimmer is facing (only if either axis is active)
		if(horizontalInput != 0f || verticalInput != 0f) {
			heading = userHeading.normalized;
			var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (horizontalInput, verticalInput);
			var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
			transform.rotation = newRotation;
		}
	}
	
	//If something changed the catch size of the swimmer, adjust the collider to match
	void UpdateCatchSize () {
		var catcher = gameObject.GetComponent<SphereCollider> ();
		
		//Adjusts gradually towards CatchZoneSize, asymptotically
		catcher.radius +=(CatchZoneSize - catcher.radius) * 2f * Time.deltaTime;
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
                
                //Lost the ball, so enlarge the team's catch radius
                CatchZoneSize = teammate.CatchZoneSize = LackingSize;
            }
        }
    }
}