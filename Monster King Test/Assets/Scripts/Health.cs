using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public class Health : MonoBehaviour, IDamage
{
	[SerializeField] protected Animator animator;
	[SerializeField] protected int maxHealth;
	protected int currentHealth;
	protected bool alive;
	protected bool canTakeDamage;
	protected Coroutine damageCoroutine;
	
	[SerializeField] protected Image healthFill;
	[SerializeField] protected Image greyFill;

	[SerializeField] protected UnityEvent onHit;
	[SerializeField] protected UnityEvent onDie;
	
	public float HealthPercent()
	{
		return (float)currentHealth/(float)maxHealth;
	}
	
	protected void Start() {
		currentHealth = maxHealth;
		alive = true;
		canTakeDamage = true;
	}
	
	public void AddHealth(int f)
	{
		currentHealth += f;
		SetHealth(HealthPercent());
	}
	
	public void Damage(int amount)
	{
		if(!alive) return;
		if(!canTakeDamage) return;
		onHit?.Invoke();
		currentHealth-=amount;
		
		SetHealth(HealthPercent());
		if(currentHealth <= 0) Die();
	}
	
	protected void SetHealth(float percent)
	{
		Sequence s = DOTween.Sequence();
		s.Append(healthFill.DOFillAmount(percent, 0.35f).SetEase(Ease.OutSine));
		s.Append(greyFill.DOFillAmount(percent, 0.5f).SetEase(Ease.OutSine));
		animator.SetFloat("health", percent);
	}
	
	protected virtual void Die()
	{
		alive = false;
		onDie?.Invoke();
	}
	
	public void AddInvincibility(float f)
	{
		if(damageCoroutine != null) StopCoroutine(damageCoroutine);
		damageCoroutine = StartCoroutine(_I());
		
		IEnumerator _I()
		{
			canTakeDamage = false;
			yield return new WaitForSeconds(f);
			canTakeDamage = true;
		}
	}
}
