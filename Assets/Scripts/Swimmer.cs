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
	public float PossessCatchZoneSize = 2f;
	public float LackingCatchZoneSize = .75f;
	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string ShootAxisName;
	public Swimmer Teammate;
	public Ball BallScript;
	#endregion
	
	#region Private Members
	private Vector3 _heading = new Vector3(1f, 0f, 0f);	
	private bool _isTouchingBall = false;
	private GameObject _ballObject;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		AssertValidAxisNames ();
		SetCatchZoneSize (LackingCatchZoneSize);
		_ballObject = BallScript.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateMovement ();
		UpdateShoot ();
	}
	
	//Called when something enters the catch zone
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Ball") {			
			_isTouchingBall = true;			
			if(!BallScript.IsHeldByPlayer) {
				BallScript.Pickup(this);
			}
		}
	}
	
	//Called when trigger case ceases
	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Ball") {
			_isTouchingBall = false;
		}
	}
	#endregion
	
	#region Public Function
	public void HandleBallRelease () {
		gameObject.layer = PlayerLayer;
		SetCatchZoneSize(LackingCatchZoneSize);
	}
	public void HandleBallPickup () {				
		//Caught the ball, so change catch size for team
		SetCatchZoneSize(PossessCatchZoneSize);
		gameObject.layer = PlayerHoldingBallLayer;
	}
	
	public void SetCatchZoneSize (float catchZoneSize) {
		var catcher = gameObject.GetComponent<SphereCollider> ();		
		catcher.radius  = catchZoneSize;
		
		catcher = Teammate.gameObject.GetComponent<SphereCollider> ();
		catcher.radius = catchZoneSize;
	}
	#endregion
	
	#region Startup Helpers
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
	#endregion
	
	#region Update Helpers
	void UpdateMovement () {
		var horizontalInput = Input.GetAxis (HorizontalAxisName);
		var verticalInput = Input.GetAxis (VerticalAxisName);
		
		//Update velocity
		var userHeading = new Vector3 (horizontalInput, 0f, verticalInput);
		rigidbody.velocity = userHeading * BaseSpeed * Time.deltaTime;
		
		//Update direction swimmer is facing (only if either axis is active)
		if(horizontalInput != 0f || verticalInput != 0f) {
			_heading = userHeading.normalized;
			var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (horizontalInput, verticalInput);
			var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
			transform.rotation = newRotation;
		}
	}

	void UpdateShoot () {
        if(BallScript.IsHeldByPlayer && Input.GetAxis (ShootAxisName) > 0f && (_ballObject.transform.parent.parent == transform || _isTouchingBall)) {
			BallScript.Shoot (_heading * BaseShootPower);
        }
    }
	#endregion
}