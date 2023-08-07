using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using DG.Tweening;

public class AbilityDodge : MonoBehaviour
{
	protected PlayerInput input;
	protected InputAction dodge;
	[Header("Air Dash")]
	[SerializeField] protected float airDashSpeed = 3f;
	[SerializeField] protected int maxAirDash = 2;
	protected int airDashesAvailable;

	[SerializeField] protected StandardControllerThirdPerson tps;
	[SerializeField] protected Animator animator;
	[SerializeField] protected UnityEvent onDodge;
	[SerializeField] protected UnityEvent onAirDash;
	
	protected void OnEnable() {
		input = new PlayerInput();
		dodge = input.Player.Dodge;
		dodge.Enable();
		dodge.performed +=_=> Dodge();
	}
	protected void OnDisable() {
		dodge.Disable();
	}
	protected void Dodge()
	{
		if (tps.Grounded() || airDashesAvailable > 0)
		{
			animator.SetTrigger("dodge");

		}
		onDodge?.Invoke();
	}
	public void SetMaxAirDash()
    {
		airDashesAvailable = maxAirDash;
    }
	public void AirDodge()
	{
		if (airDashesAvailable <= 0) return;
		tps.DelayGravity(0.25f);
		if (tps.GetInputVelocity > 0.1f)
		{
			transform.DOMove(transform.position + tps.GetMoveVector * airDashSpeed, 0.2f).SetEase(Ease.OutSine);
		}
		else
		{
			transform.DOMove(transform.position + transform.forward * airDashSpeed, 0.2f).SetEase(Ease.OutSine);
		}
		airDashesAvailable--;
		onAirDash?.Invoke();
	}
}
