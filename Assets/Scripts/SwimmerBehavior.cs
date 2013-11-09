using UnityEngine;
using System.Collections;

public enum SwimmerState {
	Neutral,
	ShootRecovery
}

public class SwimmerBehavior : MonoBehaviour {
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	private const float ShootRecoveryStateTime = .5f;
	#endregion
	
	#region Public Members
	public float MinShootPower = 2300f;
	public float MaxShootPower = 4600f;
	public float MaxShootTime = 2f;
	public float BaseSpeed = 1000f;			//No ball movement speed
	public float HoldingSpeed = 800f;		//Movement speed when possessing the ball
	public float CurrentSpeed = 1000f;
	public float TurboSpeedIncrease = 500f;
	public float PossessCatchZoneSize = 2f;
	public float LackingCatchZoneSize = 1f;
	public float TurboRechargeRate = 1f;//rate at which turbo recharges per frame.
	public int TurboMaxAmount = 100;
	public int TurboAmount = 100;
	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string ShootAxisName;
	public string TurboAxisName;
	public SwimmerBehavior Teammate;
	public BallBehavior BallScript;
	#endregion
	
	#region Protected Members
	protected float ShotTimer;
	#endregion
	
	#region Private Members
	private Vector3 _heading = new Vector3(1f, 0f, 0f);	
	private bool _isTouchingBall = false;
	private bool PreviouslyShooting = false , CurrentlyShooting = false;
	private bool PreviouslyTurbo = false , CurrentlyTurbo = false;
	private GameObject _ballObject;
	private SwimmerState _state;
	private float _stateTimer;
	private int TurboRechargeCounter;
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
		UpdateTurbo ();
		UpdateState ();
	}
	
	//Called when something enters the catch zone
	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Ball") {			
			_isTouchingBall = true;			
			if(!BallScript.IsHeldByPlayer && _state == SwimmerState.Neutral) {
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
		SetSpeed(BaseSpeed);
	}
	
	public void HandleBallPickup () {				
		//Caught the ball, so change catch size for team
		SetCatchZoneSize(PossessCatchZoneSize);
		gameObject.layer = PlayerHoldingBallLayer;
		SetSpeed (HoldingSpeed);
	}
	
	public void SetCatchZoneSize (float catchZoneSize) {
		var catcher = gameObject.GetComponent<SphereCollider> ();		
		catcher.radius  = catchZoneSize;
		
		catcher = Teammate.gameObject.GetComponent<SphereCollider> ();
		catcher.radius = catchZoneSize;
	}
	
	public void SetSpeed (float newSpeed) {
		CurrentSpeed = newSpeed;
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
		rigidbody.velocity = userHeading * CurrentSpeed * Time.deltaTime;
		
		//Update direction swimmer is facing (only if either axis is active)
		if(userHeading != Vector3.zero) {
			_heading = userHeading.normalized;
			var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (horizontalInput, verticalInput);
			var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
			transform.rotation = newRotation;
		}
	}

	void UpdateShoot () {
		PreviouslyShooting = CurrentlyShooting;
		CurrentlyShooting = (Input.GetAxis(ShootAxisName) > 0f);
		
		if(PreviouslyShooting == false && CurrentlyShooting == false)//nothing needs to be done
			return;
		
		if(PreviouslyShooting && CurrentlyShooting)//increment shotBar
			{return;}
		
		if(PreviouslyShooting && CurrentlyShooting == false){//SHOOT HER!
			if(BallScript.IsHeldByPlayer && (_ballObject.transform.parent.parent == transform || _isTouchingBall)) {//make sure a player has the ball
				
				ShotTimer = Time.time - ShotTimer;//time since button was pressed
				if(ShotTimer  > 2)
					ShotTimer = 2;
				
				var shotPower = ShotTimer / MaxShootTime;//% to modify shot speeed 
				var additionalPowerPool = MaxShootPower - MinShootPower;//amount shot can be modified
				var additionalPower = additionalPowerPool * shotPower;//power to add
				var calculatedPower = additionalPower + MinShootPower;//total power
				BallScript.Shoot (_heading * calculatedPower);
				SetState (SwimmerState.ShootRecovery);
				return;
			}
		}
			
		if(PreviouslyShooting == false && CurrentlyShooting){//start the timer
			if(BallScript.IsHeldByPlayer && (_ballObject.transform.parent.parent == transform || _isTouchingBall)) {//make sure a player has the ball
				ShotTimer = Time.time;
				return;
			}
		}
    }
	
	void UpdateTurbo(){
		PreviouslyTurbo = CurrentlyTurbo;
		CurrentlyTurbo = (Input.GetAxis(TurboAxisName) > 0f);
		
		if(PreviouslyTurbo == false && CurrentlyTurbo == false){//recharge meter
			if(TurboRechargeCounter == 10){
				if(TurboAmount < TurboMaxAmount)
					++TurboAmount;
				TurboRechargeCounter = 0;
			}
			else
				++ TurboRechargeCounter;
			return;
		}
		
		if(TurboAmount != 0 && PreviouslyTurbo && CurrentlyTurbo){//Decrement turbo meter
			--TurboAmount;
			return;
		}
		
		if(TurboAmount == 0 || PreviouslyTurbo && CurrentlyTurbo == false){//decrease speed
			CurrentSpeed = BaseSpeed;
			return;
		}
		
		if(TurboAmount != 0 && PreviouslyTurbo == false && CurrentlyTurbo){//increase speed.
			CurrentSpeed = BaseSpeed + TurboSpeedIncrease;
			return;
		}
	}
	
	void UpdateState () {
		switch (_state) {
		case SwimmerState.Neutral:
			return;
		default:
			_stateTimer -= Time.deltaTime;
			break;			
		}
		
		if(_stateTimer <= 0f) {
			SetState (SwimmerState.Neutral);
		}
	}
	#endregion
	
	void SetState (SwimmerState state) {
		ReleaseState (_state);
		
		_state = state;
		switch (state) {
		case SwimmerState.ShootRecovery:
			var catcher = gameObject.GetComponent<SphereCollider> ();		
			catcher.radius  = 0;
			_stateTimer = ShootRecoveryStateTime;
			break;
		case SwimmerState.Neutral:
		default:
			_stateTimer = 0f;
			break;
		}
	}
	
	void ReleaseState (SwimmerState state) {
		var catcher = gameObject.GetComponent<SphereCollider> ();		
		catcher.radius  = LackingCatchZoneSize;
		_stateTimer = 0f;
	}
}