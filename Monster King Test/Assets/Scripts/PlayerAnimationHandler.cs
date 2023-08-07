using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandler : MonoBehaviour
{
	[SerializeField] private PlayerMovementHandler moveHandler;
	[SerializeField] private PlayerInputHandler inputHandler;
	[SerializeField] private Animator animator;
	
	private int _velocity = Animator.StringToHash("velocity");
	private int _attack = Animator.StringToHash("attack");
	private int _run = Animator.StringToHash("run");
	private int _jump = Animator.StringToHash("jump");
	
	protected void OnEnable() {
		inputHandler.onAttack += PlayAttack;
		moveHandler.onJump += PlayJump;
		moveHandler.onLand += EndJump;
	}
	protected void OnDisable() {
		inputHandler.onAttack -= PlayAttack;
		moveHandler.onJump -= PlayJump;
		moveHandler.onLand -= EndJump;
		
	}
	
	protected void Update() {
		animator.SetFloat(_velocity, moveHandler.velocity, 0.1f, Time.deltaTime);
		animator.SetBool(_run, inputHandler.Run());
	}
	
	private void PlayJump()
	{
		animator.SetBool(_jump, true);
	}
	
	private void EndJump()
	{
		animator.SetBool(_jump, false);
		
	}
	
	private void PlayAttack()
	{
		animator.SetTrigger(_attack);
	}
}
