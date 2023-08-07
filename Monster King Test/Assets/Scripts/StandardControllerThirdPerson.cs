using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using NaughtyAttributes;

public enum PlayerState
{
	Idle,
	Walking,
	Running,
	Wallrunning,
	Airborne,
	Stopped,
	Dash
}

public class StandardControllerThirdPerson : MonoBehaviour
{
	protected PlayerInput input;
	protected InputAction move;
	protected InputAction jump;
	protected InputAction run;
	protected Transform mainCam;
	
	[SerializeField] protected PlayerState state;
	[SerializeField] protected CharacterController cc;
	[SerializeField] protected Animator animator;
	[Header("Move Parameters")]
	[SerializeField] protected float walkSpeed;
	[SerializeField] protected float runSpeed;
	[SerializeField, Min(1f)] protected float runAcceleration;
	[SerializeField] protected bool useAirMomentum = true;
	[SerializeField, HideIf("useAirMomentum")] protected float airControl = 1f;
	[SerializeField, ShowIf("useAirMomentum")] protected float airSpeed;
	[Tooltip("The smaller the value, the slower the velocity goes to zero. Try values between 0.005f and 0.01f.")]
	[SerializeField, Range(0f,0.1f), ShowIf("useAirMomentum")] protected float airDecayRate;
	[Header("Jump Parameters")]
	[SerializeField] protected float jumpHeight;
	[SerializeField] protected float gravityMultiplier;
	[SerializeField] protected float groundCheckRadius;
	[SerializeField] protected float timeInAir;
	[SerializeField] protected float maxTimeInAir;
	[SerializeField] protected bool canAirJump;
	protected int airJumps;
	[SerializeField, ShowIf("canAirJump")] protected int maxAirJumps;
	[SerializeField, ShowIf("canAirJump")] protected float airJumpVelocity;
	[SerializeField] protected LayerMask groundLayer;
	[Header("Turn Parameters")]
	[SerializeField] protected float turnSpeed;
	[SerializeField] protected float runTurnSpeed;
	[Header("Events")]
	[SerializeField] protected UnityEvent onJump;
	[SerializeField] protected UnityEvent onLeaveGround;
	[SerializeField] protected UnityEvent onLand;
	
	protected bool isGrounded;
	protected bool prevGrounded;
	protected bool running;
	protected bool canControl;
	[SerializeField] protected bool applyGravity;
	
	protected Vector2 moveInput;
	protected Vector3 moveDir;
	protected Vector3 prevMoveDir;
	protected Vector3 targetVector;
	
	protected Coroutine pauseControllerCoroutine;
	protected Coroutine gravityCoroutine;
	
	protected float gravity = 9.8f;
	protected float inputVelocity;
	protected float turnSmoothVelocity;
	protected float targetAngle;
	protected float currentTurnSpeed;
	protected float currentRunSpeed;
	
	[SerializeField] protected float jumpBuffer = 0.2f;
	protected float currentJumpTime = 0f;
	protected bool jumped = false;
	protected bool setJump = false;
	
	protected int _velocity = Animator.StringToHash("velocity");
	protected int _running = Animator.StringToHash("running");
	protected int _grounded = Animator.StringToHash("grounded");
	protected int _x = Animator.StringToHash("x");
	protected int _y = Animator.StringToHash("y");
	protected int _jump = Animator.StringToHash("jump");
	
	public Vector3 GetMoveVector { get => new Vector3(targetVector.x, 0f, targetVector.z); }
	public float GetInputVelocity { get => moveInput.sqrMagnitude; }
	public bool Grounded()
	{
		return isGrounded;
	}
	
	protected void OnEnable() {
		mainCam = Camera.main.transform;
		input = new PlayerInput();
		move = input.Player.Move;
		move.Enable();
		move.performed +=_=> moveInput = move.ReadValue<Vector2>();
		move.canceled +=_=> moveInput = Vector2.zero;
		jump = input.Player.Jump;
		jump.Enable();
		jump.performed +=_=> BufferJump();
		run = input.Player.Run;
		run.Enable();
		run.started +=_=> running = true;
		run.canceled +=_=> running = false;
		
		canControl = true;
		applyGravity = true;
		cc.enableOverlapRecovery = false;
	}
	
	protected void OnDisable() {
		move.Disable();
		jump.Disable();
		run.Disable();
	}
	
	protected void Update() {
		GroundCheck();
		HandleMove();
		HandleAnimations();
	}

	public void DelayGravity(float f)
	{
		if(gravityCoroutine != null) StopCoroutine(gravityCoroutine);
		gravityCoroutine = StartCoroutine(_DelayGravity());
		
		IEnumerator _DelayGravity()
		{
			applyGravity = false;
			moveDir.y = 0f;
			yield return new WaitForSeconds(f);
			applyGravity = true;
		}
	}
	
	protected void GroundCheck()
	{
		isGrounded = Physics.CheckSphere(transform.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);
		
		if(!isGrounded && prevGrounded)
		{
			onLeaveGround?.Invoke();
		}
		
		if(isGrounded)
		{
			if(moveDir.y < 0f) moveDir.y = -5f;
			if(!prevGrounded && timeInAir >= maxTimeInAir)
			{
				onLand?.Invoke();
			}
			timeInAir = 0f;
		}
		
		prevGrounded = isGrounded;
	}
	
	protected void HandleMove()
	{
		inputVelocity = moveInput.sqrMagnitude;
		
		// Do rotations here
		if(inputVelocity > 0.1f)
		{
			targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + mainCam.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, currentTurnSpeed);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
			targetVector = Quaternion.Euler(0f,targetAngle,0f) * Vector3.forward;
		}
		else
		{
			targetVector = Vector3.zero;
		}
		
		
		// Do all the movement calculations here
		Vector3 adjustedVector = new Vector3();
		switch(state)
		{
		case PlayerState.Airborne:
			if(isGrounded) state = PlayerState.Idle;
			if(useAirMomentum)
			{
				prevMoveDir = Vector3.MoveTowards(prevMoveDir, Vector3.zero, airDecayRate);
				adjustedVector = prevMoveDir + targetVector * airSpeed;
				moveDir.x = adjustedVector.x;
				moveDir.z = adjustedVector.z;
			}
			else
			{
				adjustedVector = targetVector * airSpeed * airControl * (applyGravity ? 1f : 0.1f);
				moveDir.x = adjustedVector.x;
				moveDir.z = adjustedVector.z;
			}
			
			if(applyGravity) 
			{
				moveDir.y -= gravity * gravityMultiplier * Time.deltaTime;
			}
			else
			{
				moveDir.y -= gravity * 0.1f * Time.deltaTime;
			}
			
			timeInAir += Time.deltaTime;
			break;
		case PlayerState.Dash:
			break;
		case PlayerState.Idle:
			if(inputVelocity > 0.1) state = PlayerState.Walking;
			if(!isGrounded) 
			{
				prevMoveDir = moveDir;
				state = PlayerState.Airborne;
			}
			currentTurnSpeed = turnSpeed;
			moveDir.x = 0f;
			moveDir.z = 0f;
			break;
		case PlayerState.Running:
			if(inputVelocity < 0.1f) state = PlayerState.Idle;
			if(!running) state = PlayerState.Walking;
			if(!isGrounded) 
			{
				prevMoveDir = targetVector * runSpeed;
				state = PlayerState.Airborne;
			}
			currentTurnSpeed = runTurnSpeed;
			if(!canControl) return;
			if(currentRunSpeed < runSpeed) currentRunSpeed += Time.deltaTime * runAcceleration;
			adjustedVector = targetVector * currentRunSpeed;
			moveDir.x = adjustedVector.x;
			moveDir.z = adjustedVector.z;
			break;
		case PlayerState.Stopped:
			break;
		case PlayerState.Walking:
			if(inputVelocity < 0.1f) state = PlayerState.Idle;
			if(running) 
			{
				currentRunSpeed = walkSpeed;
				state = PlayerState.Running;
			}
			if(!isGrounded) 
			{
				prevMoveDir = targetVector * walkSpeed;
				state = PlayerState.Airborne;
			}
			currentTurnSpeed = turnSpeed;
			if(!canControl) return;
			adjustedVector = targetVector * walkSpeed;
			moveDir.x = adjustedVector.x;
			moveDir.z = adjustedVector.z;
			break;
		case PlayerState.Wallrunning:
			break;
		}
		
		// Apply the actual move to the Character Controller
		if(!animator.applyRootMotion) cc.Move(moveDir * Time.deltaTime);
		
		Debug.DrawRay(transform.position, moveDir, Color.green, 1f);
		
		currentJumpTime += Time.deltaTime;
		if(currentJumpTime < jumpBuffer && setJump)
		{
			Jump();
		}
	}

	
	protected void BufferJump()
	{
		currentJumpTime = 0f;
		setJump = true;
	}
	
	protected void Jump()
	{
		if(isGrounded && setJump)
		{
			onJump?.Invoke();
			moveDir.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
			setJump = false;
			
			if(canAirJump)
			{
				airJumps = maxAirJumps;
			}
		}
		
		if(!isGrounded && setJump && canAirJump && airJumps > 0)
		{
			onJump?.Invoke();
			moveDir.y = Mathf.Sqrt(airJumpVelocity * 2f * gravity);
			setJump = false;
			airJumps--;
		}
	}
	
	public void SetMomentum(Vector3 v)
	{
		moveDir = v;
		prevMoveDir = v;
	}
	
	public void AddMomentum(Vector3 v)
	{
		moveDir += v;
	}
	
	protected void HandleAnimations()
	{
		float yDot = Vector3.Dot(transform.forward, targetVector);
		float xDot = Vector3.Dot(transform.right, targetVector);
		
		animator.SetFloat(_x,xDot, 0.1f, Time.deltaTime);
		animator.SetFloat(_y,yDot, 0.1f, Time.deltaTime);
		animator.SetFloat(_velocity,inputVelocity, 0.05f, Time.deltaTime);
		animator.SetBool(_running, running);
		
		if(!isGrounded)
		{
			if(timeInAir >= maxTimeInAir)
			{
				animator.SetBool(_grounded, false);
			}
		}
		else
		{
			animator.SetBool(_grounded, true);
		}
	}
	public void PauseCharacterController(float duration)
	{
		WaitForSeconds pauseDelay = new WaitForSeconds(duration);
		if(pauseControllerCoroutine != null) StopCoroutine(pauseControllerCoroutine);
		pauseControllerCoroutine = StartCoroutine(_Pause());
		
		IEnumerator _Pause()
		{
			canControl = false;
			yield return pauseDelay;
			canControl = true;
		}
	}
}
