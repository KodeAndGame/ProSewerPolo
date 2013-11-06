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
	public GameObject Ball;
	#endregion
	
	#region Private Members
	private Vector3 _heading = new Vector3(1f, 0f, 0f);	
	private bool _isTouchingBall = false;
	private Ball _ballScript;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		AssertValidAxisNames ();
		SetCatchZoneSize (LackingCatchZoneSize);
		_ballScript = Ball.GetComponent ("Ball") as Ball;
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
			
			if(other.transform.parent == null || other.transform.parent.tag != "Player") {
				
				//Caught the ball, so change catch size for team
				SetCatchZoneSize(PossessCatchZoneSize);
				
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
			_isTouchingBall = false;
		}
	}
	#endregion
	
	#region Public Function
	public void HandleBallRelease () {
		gameObject.layer = PlayerLayer;
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
        if(Ball.transform.parent != null && Ball.transform.parent.parent != null) {
            if(Input.GetAxis (ShootAxisName) > 0f && (Ball.transform.parent.parent == transform || _isTouchingBall)) {
				_ballScript.Shoot (_heading * BaseShootPower);
                
                //Lost the ball, so change the team's catch radius
				SetCatchZoneSize(LackingCatchZoneSize);
            }
        }
    }
	#endregion
}