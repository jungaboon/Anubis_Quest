using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerAttackHandler : MonoBehaviour, IAttack
{
	[SerializeField] protected StandardControllerThirdPerson tps;
	[SerializeField] protected LayerMask attackMask;
	[SerializeField] protected Animator animator;
	
	protected PlayerInput input;
	protected InputAction attack;
	protected InputAction heavyAttack;
	protected InputAction moving;
	protected InputAction charge;

	[Header("Attack Buffer")]
	[SerializeField] protected bool currentlyTriggered;
	[SerializeField] protected bool triggersReset;
	[SerializeField] protected bool canCombo;
	[SerializeField] protected bool pressedAttack;
	[SerializeField] protected float attackResetTimer;
	protected float attackTime;

	[Header("Special Attack")]
	[SerializeField] protected bool canSpecial;
	[SerializeField] protected Transform firepoint;
	[SerializeField] protected PlayerProjectile pProjectile;
	
	[Header("Attack Asistance")]
	[SerializeField] protected float assistRadius;
	[SerializeField] protected float assistTurnDuration;
	[SerializeField] protected float assistAngle;
	protected Coroutine attackTurnCoroutine;
	protected Transform currentAssistTarget;
	protected bool currentlyMoving;
	
	[Header("Events")]
	[SerializeField] protected UnityEvent onAttack;
	[SerializeField] protected UnityEvent onChargeAttack;
	[SerializeField] protected UnityEvent onAirChargeAttack;
	
	[Header("Sounds")]
	[SerializeField] protected AudioSource audioSource;
	[SerializeField] protected SCR_Sounds sounds;
	
	protected void OnDrawGizmos() {
		if(currentAssistTarget != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(currentAssistTarget.position, 0.5f);
		}
		
		Gizmos.DrawWireSphere(firepoint.position, 1.5f);
	}
	
	protected void OnEnable() {
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		currentlyTriggered = false;
		
		input = new PlayerInput();
		attack = input.Player.Fire;
		attack.Enable();
		attack.started +=_=> pressedAttack = true;
		attack.canceled +=_=> TriggerAttack();
		
		heavyAttack = input.Player.Heavy;
		heavyAttack.Enable();
		heavyAttack.performed +=_=> TriggerHeavyAttack();
		
		charge = input.Player.Charge;
		charge.Enable();
		charge.performed +=_=> TriggerChargeAttack();
		
		moving = input.Player.Move;
		moving.Enable();
		moving.performed +=_=> currentlyMoving = true;
		moving.canceled +=_=> currentlyMoving = false;
		
		canSpecial = true;
		canCombo = true;
	}
	
	protected void OnDisable() {
		attack.Disable();
		heavyAttack.Disable();
		moving.Disable();
	}
	
	protected void Update() {
		animator.SetBool("canCombo", canCombo);
		if(attackTime > 0) attackTime -= Time.deltaTime;
		else
		{
			animator.ResetTrigger("attack");
		}
	}
	
	protected void TriggerAttack()
	{
		if(!pressedAttack) return;
		AttackTurn();
		animator.SetTrigger("attack");
		canCombo = false;
		attackTime = attackResetTimer;
		onAttack?.Invoke();
	}
	
	protected void TriggerHeavyAttack()
	{
		AttackTurn();
		animator.SetTrigger("heavyAttack");
		triggersReset = false;
		onAttack?.Invoke();
	}
	
	protected void TriggerChargeAttack()
	{
		animator.ResetTrigger("heavyAttack");
		animator.ResetTrigger("attack");
		pressedAttack = false;
		
		animator.SetTrigger("chargeAttack");
		AttackTurn();
		triggersReset = false;
		onAttack?.Invoke();
	}
	
	public void ResetAllTriggers()
	{
		animator.ResetTrigger("heavyAttack");
		animator.ResetTrigger("attack");
		animator.ResetTrigger("chargeAttack");
	}
	
	
	public void ChargeAttack(SCR_Attack attack)
	{
		if(tps.Grounded()) onChargeAttack?.Invoke();
		else onAirChargeAttack?.Invoke();
		
		Collider[] hitColl = new Collider[10];
		int numColl = Physics.OverlapSphereNonAlloc(transform.position, attack.range, hitColl, attackMask);
		for (int i = 0; i < numColl; i++) {
			if(hitColl[i].TryGetComponent(out IDamage iDamage))
			{
				iDamage.Damage(attack.damage);
				if(audioSource)
				{
					audioSource.PlayOneShot(attack.attackSound, Random.Range(0.65f,0.75f));
				}
			}
		}
	}
	
	public void Attack(SCR_Attack attack)
	{
		if(audioSource)
		{
			audioSource.PlayOneShot(sounds.attackSounds.sound[Random.Range(0,sounds.attackSounds.sound.Length)], Random.Range(0.5f,0.6f));
		}
		
		Collider[] hitColl = new Collider[10];
		int numColl = Physics.OverlapSphereNonAlloc(firepoint.position, attack.range, hitColl, attackMask);
		for (int i = 0; i < numColl; i++) {
			if(hitColl[i].TryGetComponent(out IDamage iDamage))
			{
				iDamage.Damage(attack.damage);
				if(audioSource)
				{
					audioSource.PlayOneShot(attack.attackSound, Random.Range(0.65f,0.75f));
				}
			}
		}
	}
	
	public void SetCanCombo(int t)
	{
		canCombo = t > 0 ? true : false;
		pressedAttack = false;
	}
	
	public void SpecialAttack(SCR_Attack attack)
	{
		if(!canSpecial) return;
		Collider[] hitColl = new Collider[10];
		int numColl = Physics.OverlapSphereNonAlloc(firepoint.position, attack.range, hitColl, attackMask);
		
		canSpecial = false;
		List<Transform> t = new List<Transform>();
		if(numColl > 0)
		{
			for (int i = 0; i < numColl; i++) {
				if(hitColl[i].TryGetComponent(out IDamage iDamage))
				{
					t.Add(hitColl[i].transform);
				}
			}
		}
		
		pProjectile.Launch(t, attack.damage);
	}
	
	public void SpecialAttackReturn()
	{
		animator.Play("Grab", 2);
		canSpecial = true;
	}
	
	protected void AttackTurn()
	{
		if(attackTurnCoroutine != null) StopCoroutine(attackTurnCoroutine);
		attackTurnCoroutine = StartCoroutine(_AttackTurn());
		
		IEnumerator _AttackTurn()
		{
			if(currentlyMoving) yield break;
			float t = 0f;
			currentAssistTarget = JungaBoonUtils.GetClosestInAngle(transform.position, transform.forward, assistRadius, assistAngle, attackMask);
			if(currentAssistTarget == null) yield break;
			Vector3 adjustedPos = transform.position + transform.forward;
			while(t < assistTurnDuration)
			{
				adjustedPos = Vector3.Lerp(transform.position + transform.forward, new Vector3(currentAssistTarget.position.x, transform.position.y, currentAssistTarget.position.z), t/ assistTurnDuration);
				transform.LookAt(adjustedPos);
				t += Time.deltaTime;
				yield return null;
			}
			currentAssistTarget = null;
		}
	}
}
