using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using DG.Tweening;
using MoreMountains.Feedbacks;

public class BeastController : MonoBehaviour
{
	private enum PlayerStates
	{
		Idle,
		Walking,
		Running,
		Wallrunning,
		Airborne,
		Stunned,
		AirDash
	}
	
	[SerializeField] private PlayerStates state;
	[SerializeField] private PlayerInputHandler input;
	[SerializeField] private CharacterController cc;
	[SerializeField] private Transform mainCam;
	[SerializeField] private Animator animator;
	[Header("Move Parameters")]
	[SerializeField] private float currentSpeed;
	[SerializeField] private float prevSpeed;
	[SerializeField] private float normalSpeed;
	[SerializeField] private float runSpeed;
	[SerializeField] private float initialAirSpeed;
	[SerializeField] private float airSpeed;
	[SerializeField] private float wallRunSpeed;
	[Space]
	[SerializeField] private Vector2 jumpHeightRange;
	[SerializeField] private float currentJumpHeight;
	[SerializeField] private float jumpChargeSpeed;
	[SerializeField] private float jumpChargeTime;
	[SerializeField] private float gravity;
	[SerializeField] private float airDashSpeed;
	[SerializeField] private float inputVelocity;
	[SerializeField] private float jumpMaxMargin;
	[SerializeField] private float currentJumpMargin;
	[SerializeField] private float airControl;
	[Space]
	[SerializeField] private float turnSpeed;
	[SerializeField] private float idleTurnSpeed;
	[SerializeField] private float runTurnSpeed;
	[SerializeField] private float airTurnSpeed;
	[Space]
	[SerializeField] private bool useGravity;
	[SerializeField] private bool isGrounded;
	[SerializeField] private bool prevGrounded;
	[SerializeField] private bool chargingJump;
	[SerializeField] private bool recentlyPressedJump;
	[SerializeField] private LayerMask groundMask;
	[Space]
	[SerializeField] private ParticleSystem chargeParticles;
	[SerializeField] private UnityEvent onJump;
	[SerializeField] private UnityEvent onChargeStart;
	[SerializeField] private UnityEvent onLand;
	[SerializeField] private UnityEvent onStartRun;
	
	private Vector2 moveInput;
	private Vector2 desiredDir;
	private Vector3 prevMoveDir;
	private Vector3 savedMoveDir;
	private Vector3 moveDir;
	private Vector3 finalMoveDir;
	
	private Coroutine jumpChargeCoroutine;
	private Coroutine airDashCoroutine;
	
	private float turnSmoothVelocity;
	private float targetAngle;
	
	private int _velocity = Animator.StringToHash("velocity");
	private int _running = Animator.StringToHash("running");
	private int _grounded = Animator.StringToHash("grounded");
	
	protected void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, moveDir);
		
		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, finalMoveDir);
		
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, 0.25f);
	}
	
	protected void OnEnable() {
		input.onJump += ChargeJumpStart;
		input.onJumpRelease += ChargeJumpEnd;
		input.onRunStart += AirDash;
	}
	protected void OnDisable() {
		input.onJump -= ChargeJumpStart;
		input.onJumpRelease -= ChargeJumpEnd;
		input.onRunStart -= AirDash;
	}
	
	protected void Update() {
		GroundCheck();
		moveInput = input.GetMoveInput();
		inputVelocity = moveInput.sqrMagnitude;
		animator.SetFloat(_velocity, inputVelocity, 0.1f, Time.deltaTime);

		moveDir = Vector3.zero;
		if(inputVelocity > 0.1f)
		{
			targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + mainCam.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
			moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
			savedMoveDir = moveDir;
		}

		switch(state)
		{
		case PlayerStates.Airborne:
			turnSpeed = airTurnSpeed;
			prevSpeed = Mathf.Lerp(prevSpeed, airSpeed, 0.25f * Time.deltaTime);
			moveDir = prevMoveDir * prevSpeed;
			finalMoveDir.y -= gravity * Time.deltaTime;
			if(isGrounded) state = PlayerStates.Idle;
			break;
		case PlayerStates.Idle:
			currentSpeed = 0f;
			turnSpeed = idleTurnSpeed;
			if(inputVelocity > 0.1f) state = PlayerStates.Walking;
			if(!isGrounded) 
			{
				prevMoveDir = moveDir;
				prevSpeed = currentSpeed;
				state = PlayerStates.Airborne;
			}
			break;
		case PlayerStates.Running:
			currentSpeed = Mathf.Lerp(currentSpeed, runSpeed, Time.deltaTime);
			turnSpeed = runTurnSpeed;
			moveDir *= currentSpeed;
			if(!input.Run()) state = PlayerStates.Walking;
			if(!isGrounded) 
			{
				prevMoveDir = moveDir / currentSpeed;
				prevSpeed = currentSpeed;
				state = PlayerStates.Airborne;
			}
			animator.SetBool(_running, true);
			break;
		case PlayerStates.Stunned:
			break;
		case PlayerStates.Walking:
			currentSpeed = normalSpeed;
			turnSpeed = idleTurnSpeed;
			moveDir *= currentSpeed;
			if(inputVelocity < 0.1f) state = PlayerStates.Idle;
			if(input.Run()) 
			{
				state = PlayerStates.Running;
				onStartRun?.Invoke();
			}
			if(!isGrounded) 
			{
				prevMoveDir = moveDir;
				prevSpeed = currentSpeed;
				state = PlayerStates.Airborne;
			}
			animator.SetBool(_running, false);
			break;
		case PlayerStates.Wallrunning:
			break;
		case PlayerStates.AirDash:
			break;
		}
		
		finalMoveDir.x = moveDir.x;
		finalMoveDir.z = moveDir.z;
		
		// FOR JUMPING
		if(chargingJump && currentJumpHeight < jumpHeightRange.y) currentJumpHeight += jumpChargeSpeed * Time.deltaTime;
		if(recentlyPressedJump && currentJumpMargin < jumpMaxMargin) currentJumpMargin += Time.deltaTime;
		else recentlyPressedJump = false;
		Jump();
		
		// FINAL MOVE
		cc.Move(finalMoveDir * Time.deltaTime);
	}
	
	private void GroundCheck()
	{
		isGrounded = Physics.CheckSphere(transform.position, 0.25f, groundMask, QueryTriggerInteraction.Ignore);
		animator.SetBool(_grounded, isGrounded);
		
		if(isGrounded && !prevGrounded) onLand?.Invoke();
		
		prevGrounded = isGrounded;
	}
	
	private void ChargeJumpStart()
	{
		currentJumpHeight = jumpHeightRange.x;
		chargingJump = true;
		if(jumpChargeCoroutine != null) StopCoroutine(jumpChargeCoroutine);
		jumpChargeCoroutine = StartCoroutine(ChargeJump());
	}
	private void ChargeJumpEnd()
	{
		chargingJump = false;
		recentlyPressedJump = true;
		currentJumpMargin = 0f;
		chargeParticles.Stop();
	}
	
	private IEnumerator ChargeJump()
	{
		float time = 0f;
		chargeParticles.Play();
		while(time < jumpChargeTime)
		{
			currentJumpHeight = Mathf.Lerp(jumpHeightRange.x, jumpHeightRange.y, time/jumpChargeTime);
			chargeParticles.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, time/jumpChargeTime);
			time += Time.deltaTime;
			yield return null;
		}
	}
	
	private void Jump()
	{
		if(isGrounded && recentlyPressedJump)
		{
			finalMoveDir.y = Mathf.Sqrt(currentJumpHeight * 2f * gravity);
			recentlyPressedJump = false;
			onJump?.Invoke();
		}
	}
	
	private void AirDash()
	{
		if(isGrounded) return;
		if(airDashCoroutine != null) StopCoroutine(airDashCoroutine);
		airDashCoroutine = StartCoroutine(_Dash());
		
		IEnumerator _Dash()
		{
			animator.SetTrigger("dash");
			state = PlayerStates.AirDash;
			Vector3 initMoveInput = new Vector3(savedMoveDir.x, 0f, savedMoveDir.z);
			float t = 0f;
			while(t < 0.25f)
			{
				cc.Move(initMoveInput.normalized * airDashSpeed * Time.deltaTime);
				t += Time.deltaTime;
				yield return null;
			}
			state = PlayerStates.Airborne;
		}
	}
}
