﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SwimmerState {
	Neutral,		//Default state
	ShootRecovery,	//Can't catch
	BigCatching		//Catch size enlarged -- to aid passing
}

public enum PlayerType {
	None,
	PlayerOne,
	PlayerTwo
}

public enum SwimmerSide {
	Left,
	Right
}

public class SwimmerBehavior : MonoBehaviour {
	
	#region Static
	public static  List<SwimmerBehavior> allSwimmers;
	public static PlayerType LastPossession = PlayerType.None;
	#endregion
	
	#region Protected
	protected Animator animator;
	#endregion
	
	#region Constants
	private const int PlayerLayer = 9;
	private const int PlayerHoldingBallLayer = 8;
	private const float HeadingMultiplier = 0.025f;
	#endregion
	
	#region Public Members
	public PlayerType Player;
	public SwimmerSide SwimmerSide;
	
	//Shooting variables
	public float MinShootPower = 2300f;
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
	public float CurrentSpeed = 10;
	public int TurboAmount = 100;
	
	//public GUIBehavior PowerMeter;
	#endregion
	
	#region Protected Members
	protected float ShotTimer;
	protected float CatchableTimer;
	#endregion
	
	#region Private Members
	private Vector3 heading = new Vector3(0f, 0f, 0f);	
	private Vector3 targetHeading = new Vector3(0f, 0f, 0f);
	private Vector3 headingDelta = new Vector3(0f, 0f, 0f);
	private float animDirection;
	private float animSpeed;
	
	int directionHash, speedHash, doFlipHash;
	
	private bool isTouchingBall = false;
	private bool PreviouslyShooting = false , CurrentlyShooting = false;
	private bool PreviouslyTurbo = false , CurrentlyTurbo = false;
	private GameObject ballObject;
	private SwimmerState state;
	private float stateTimer;
	private int TurboRechargeCounter;
	#endregion
	
	#region Unity Hooks
	// Use this for initialization
	void Start () {
		AssertValidAxisNames ();
		SetCatchZoneSize (DefaultZoneSize);
		ballObject = BallScript.gameObject;
		
		if(allSwimmers == null) {
			allSwimmers = new List<SwimmerBehavior> (); 
		}
		allSwimmers.Add (this);
		
		animator = GetComponentInChildren<Animator>();
		
		directionHash = Animator.StringToHash("Direction");
		speedHash = Animator.StringToHash("Speed");
	    doFlipHash = Animator.StringToHash("DoFlip");
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
			isTouchingBall = true;			
			if(!BallScript.IsHeldByPlayer && state != SwimmerState.ShootRecovery) {
				BallScript.Pickup(this);
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
	
	#region Public Function
	public void HandleBallRelease () {
		
		gameObject.layer = PlayerLayer;
		Teammate.SetCatchZoneSize(PassingZoneSize);
		SetSpeed(BaseSpeed);
		SetState (SwimmerState.ShootRecovery);
		
		var capsule = GetComponent<CapsuleCollider>();
		capsule.radius = .5f;
		var center = capsule.center;
		center.z = 0f;
		capsule.center = center;
	}
	
	public void HandleBallPickup () {				
		//Caught the ball, so change catch size for all
		for(var x = 0; x < allSwimmers.Count; x++) {
			allSwimmers[x].Reset();
		}
		
		gameObject.layer = PlayerHoldingBallLayer;
		SetSpeed (HoldingSpeed);
		
		var capsule = GetComponent<CapsuleCollider>();
		capsule.radius = 1f;
		var center = capsule.center;
		center.z = .4f;
		capsule.center = center;
		
		LastPossession = Player;
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
		stateTimer = 0f;
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
		targetHeading = userHeading;
		
		headingDelta = targetHeading - heading;
		heading = heading + (headingDelta * HeadingMultiplier);
		
		rigidbody.velocity = heading * CurrentSpeed;
		
		
		
		
		
		// aniation controller parameters
		animDirection = Vector3.Cross(heading, targetHeading).y;
		animSpeed = rigidbody.velocity.magnitude;
		
		
		
		
		if (Vector3.Dot(heading, targetHeading) < -0.3f ) {
			//_animator.SetBool(doFlipHash, true);
			
		}
		else {
			animator.SetBool(doFlipHash, false);
			//Update direction swimmer is facing (only if either axis is active)
			if(userHeading != Vector3.zero) {
				//_heading = userHeading.normalized;
				var newRotationAroundY = Mathf.Rad2Deg * Mathf.Atan2 (heading.x, heading.z);
				var newRotation = Quaternion.Euler(new Vector3(0, newRotationAroundY, 0));
				transform.rotation = newRotation;
			}
		}
		
		animator.SetFloat(directionHash, animDirection);
		animator.SetFloat(speedHash, animSpeed);
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
			if(BallScript.IsHeldByPlayer && (ballObject.transform.parent.parent == transform || isTouchingBall)) {//make sure a player has the ball
				
				ShotTimer = Time.time - ShotTimer;//time since button was pressed
				if(ShotTimer  > MaxShootTime)
					ShotTimer = MaxShootTime;
				
				var shotPower = ShotTimer / MaxShootTime;//% to modify shot speeed 
				var additionalPowerPool = MaxShootPower - MinShootPower;//amount shot can be modified
				var additionalPower = additionalPowerPool * shotPower;//power to add
				var calculatedPower = additionalPower + MinShootPower;//total power
				
				BallScript.Shoot (heading.normalized * calculatedPower);
				SetState (SwimmerState.ShootRecovery);
				Teammate.PrepareToCatch();
				//PowerMeter.ShotPower = 0;
				
				return;
			}
		}
			
		if(PreviouslyShooting == false && CurrentlyShooting){//start the timer
			ShotTimer = Time.time;
			return;
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
		switch (state) {
		case SwimmerState.Neutral:
			return;
		default:
			stateTimer -= Time.deltaTime;
			break;			
		}
		
		if(stateTimer <= 0f) {
			if (state == SwimmerState.ShootRecovery)
				SetState (SwimmerState.BigCatching);
			else
				SetState (SwimmerState.Neutral);
		}
	}
	#endregion
	
	void SetState (SwimmerState newState) {
		ReleaseState (state);
		
		state = newState;
		switch (state) {
		case SwimmerState.ShootRecovery:
			SetCatchZoneSize(0f);
			stateTimer = ShootRecoveryStateTime;
			break;
		case SwimmerState.BigCatching:
			SetCatchZoneSize(PassingZoneSize);
			stateTimer = CatchableTime;
			break;
		default:
			stateTimer = 0f;
			break;
		}
	}
	
	void ReleaseState (SwimmerState oldState) {
		SetCatchZoneSize( DefaultZoneSize );
		SetSpeed (BaseSpeed);
		stateTimer = 0f;
	}
}