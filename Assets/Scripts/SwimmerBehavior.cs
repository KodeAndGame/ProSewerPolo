using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwimmerState {
	Neutral,		//Default state
	ShootRecovery,	//Can't catch
	BigCatching		//Catch size enlarged -- to aid passing
}

public class SwimmerBehavior : MonoBehaviour {
	
	#region Static
	public static  List<SwimmerBehavior> allSwimmers;
	#endregion
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	#endregion
	
	#region Public Members
	//Shooting variables
	public float _minShootPower2 = 2300f;
	public float MaxShootPower = 4600f;
	public float MaxShootTime = 2f;
	
	//Speed variables
	public float BaseSpeed = 1000f;			//Ball-less movement speed
	public float HoldingSpeed = 800f;		//Movement speed with the ball
	public float TurboSpeedIncrease = 500f;
	
	//Turbo variables
	public int TurboMaxAmount = 100;
	
	//Catching variables
	public float PassingZoneSize = 2f;
	public float DefaultZoneSize = 1f;
	public float ShootRecoveryStateTime = .5f;	//Time to catch again after shooting
	public float CatchableTime = 2f;				//Time teammate/you have an enlarged catch zone
	
	//Input names
	public string HorizontalAxisName;
	public string VerticalAxisName;
	public string ShootAxisName;
	public string TurboAxisName;
	
	//Object references
	public SwimmerBehavior Teammate;
	public BallBehavior BallScript;
	
	//TODO: These should probably be moved away from the regular public members.
	// I don't intend for these to be modified via GUI.
	public float CurrentSpeed = 1000f;
	public int TurboAmount = 100;
	
	//public GUIBehavior PowerMeter;
	#endregion
	
	#region Protected Members
	protected float ShotTimer;
	protected float CatchableTimer;
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
		SetCatchZoneSize (DefaultZoneSize);
		_ballObject = BallScript.gameObject;
		
		if(allSwimmers == null) {
			allSwimmers = new List<SwimmerBehavior> (); 
		}
		allSwimmers.Add (this);
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
			if(!BallScript.IsHeldByPlayer && _state != SwimmerState.ShootRecovery) {
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
		Teammate.SetCatchZoneSize(PassingZoneSize);
		SetSpeed(BaseSpeed);
	}
	
	public void HandleBallPickup () {				
		//Caught the ball, so change catch size for all
		for(var x = 0; x < allSwimmers.Count; x++) {
			allSwimmers[x].Reset();
		}
		
		gameObject.layer = PlayerHoldingBallLayer;
		SetSpeed (HoldingSpeed);
	}
	
	public void SetCatchZoneSize (float newSize) {
		var catcher = GetComponent<SphereCollider> ();
		catcher.radius = newSize;
	}
	
	public void SetSpeed (float newSpeed) {
		CurrentSpeed = newSpeed;
	}
	
	public void PrepareToCatch () {
		SetState(SwimmerState.BigCatching);
	}
	
	public void Reset () {
		SetState(SwimmerState.Neutral);
		SetCatchZoneSize( DefaultZoneSize );
		_stateTimer = 0f;
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
		
		//if(PreviouslyShooting && CurrentlyShooting){//increment shotBar
			//if(BallScript.IsHeldByPlayer && (_ballObject.transform.parent.parent == transform || _isTouchingBall)) {//make sure a player has the ball
				//if(PowerMeter.ShotPower < MaxShootTime * 60)//for 60fps
					//++ PowerMeter.ShotPower;
			//}
		//}
		
		if(PreviouslyShooting && CurrentlyShooting)//increment shotBar
			{return;}
		
		if(PreviouslyShooting && CurrentlyShooting == false){//SHOOT HER!
			if(BallScript.IsHeldByPlayer && (_ballObject.transform.parent.parent == transform || _isTouchingBall)) {//make sure a player has the ball
				
				ShotTimer = Time.time - ShotTimer;//time since button was pressed
				if(ShotTimer  > 2)
					ShotTimer = 2;
				
				var shotPower = ShotTimer / MaxShootTime;//% to modify shot speeed 
				var additionalPowerPool = MaxShootPower - _minShootPower2;//amount shot can be modified
				var additionalPower = additionalPowerPool * shotPower;//power to add
				var calculatedPower = additionalPower + _minShootPower2;//total power
				
				BallScript.Shoot (_heading * calculatedPower);
				SetState (SwimmerState.ShootRecovery);
				Teammate.PrepareToCatch();
				//PowerMeter.ShotPower = 0;
				
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
			if (_state == SwimmerState.ShootRecovery)
				SetState (SwimmerState.BigCatching);
			else
				SetState (SwimmerState.Neutral);
		}
	}
	#endregion
	
	void SetState (SwimmerState state) {
		ReleaseState (_state);
		
		_state = state;
		switch (state) {
		case SwimmerState.ShootRecovery:
			SetCatchZoneSize(0f);
			_stateTimer = ShootRecoveryStateTime;
			break;
		case SwimmerState.BigCatching:
			SetCatchZoneSize(PassingZoneSize);
			_stateTimer = CatchableTime;
			break;
		default:
			_stateTimer = 0f;
			break;
		}
	}
	
	void ReleaseState (SwimmerState state) {
		SetCatchZoneSize( DefaultZoneSize );
		
		_stateTimer = 0f;
	}
}