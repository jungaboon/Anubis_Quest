using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementHandler : MonoBehaviour
{
	[SerializeField] private PlayerInputHandler input;
	[SerializeField] private CharacterController ccontroller;
	[SerializeField] private Transform camera;
	[Space]
	[SerializeField] private Vector2 moveSpeed;
	[SerializeField] private float turnSpeed = 1f;
	[SerializeField] private float jumpSpeed = 7f;
	[SerializeField] private float gravity = 10f;
	[SerializeField] private LayerMask groundMask;
	private float currentMoveSpeed;
	private Vector3 finalMoveDir;
	private Vector3 moveDir;
	private bool grounded;
	private bool previouslyGrounded;
	
	public float velocity {get; private set;}
	private float turnSmoothVelocity;
	
	public delegate void OnLand();
	public OnLand onLand;
	
	public delegate void OnJump();
	public OnJump onJump;
	
	protected void OnEnable() {
		input.onJump += Jump;
	}
	protected void OnDisable() {
		input.onJump -= Jump;
	}
	
	protected void Update() {
		Vector2 moveInput = input.GetMoveInput();
		velocity = moveInput.sqrMagnitude;
		currentMoveSpeed = input.Run()? moveSpeed.y : moveSpeed.x;
		GroundCheck();
		
		moveDir = Vector3.zero;
		if(velocity > 0.1f)
		{
			float targetAngle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + camera.eulerAngles.y;
			float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeed);
			transform.rotation = Quaternion.Euler(0f, angle, 0f);
			
			moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
			moveDir *= currentMoveSpeed;
		}
		
		finalMoveDir.x = moveDir.x;
		finalMoveDir.z = moveDir.z;
		ccontroller.Move(finalMoveDir * Time.deltaTime);
		
		previouslyGrounded = grounded;
	}
	
	private void GroundCheck()
	{
		grounded = Physics.CheckSphere(transform.position, 0.1f, groundMask);
		if(!grounded)
		{
			finalMoveDir.y -= gravity * Time.deltaTime;
		}
		if(grounded && !previouslyGrounded) 
		{
			finalMoveDir.y = 0f;
			onLand?.Invoke();
		}
	}
	
	private void Jump()
	{
		if(!grounded) return;
		finalMoveDir.y = jumpSpeed;
		onJump?.Invoke();
	}
	

}
