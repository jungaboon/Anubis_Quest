using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInputHandler : MonoBehaviour
{
	[SerializeField] private PlayerInput input;
	
	protected InputAction move;
	protected InputAction run;
	protected InputAction attack;
	protected InputAction jump;
	
	protected void OnEnable() {
		input = new PlayerInput();
		
		move = input.Player.Move;
		move.Enable();
		
		run = input.Player.Run;
		run.Enable();
		run.started +=_=>onRunStart?.Invoke();
		run.canceled +=_=>onRunRelease?.Invoke();
		
		attack = input.Player.Fire;
		attack.Enable();
		attack.performed +=_=>onAttack?.Invoke();
		
		jump = input.Player.Jump;
		jump.Enable();
		jump.started +=_=>onJump?.Invoke();
		jump.canceled +=_=>onJumpRelease?.Invoke();
	}
	
	protected void OnDisable() {
		move.Disable();
		attack.Disable();
		jump.Disable();
		run.Disable();
		
	}
	
	public Vector2 GetMoveInput()
	{
		return move.ReadValue<Vector2>();
	}
	
	public bool Run()
	{
		return run.IsPressed();
	}
	
	public delegate void OnAttack();
	public OnAttack onAttack;
	public delegate void OnJump();
	public OnJump onJump;
	public OnJump onJumpRelease;
	public OnJump onRunStart;
	public OnJump onRunRelease;
}
