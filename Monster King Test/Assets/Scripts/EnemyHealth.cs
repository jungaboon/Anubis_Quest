using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour, IDamage, IAttack
{
	protected ObjectPooler objectPooler;
	
	[SerializeField] protected Animator animator;
	[SerializeField] protected Rigidbody rb;
	[SerializeField] protected NavMeshAgent agent;
	[SerializeField] protected BlazeAI blazeAI;
	[SerializeField] protected AudioSource audioSource;
	[SerializeField] protected SCR_Sounds sounds;
	[SerializeField] protected LayerMask attackMask;
	[SerializeField] protected LayerMask groundMask;
	[SerializeField] protected Transform torso;
	[Space]
	[SerializeField] protected float launchForce;
	[SerializeField] protected Vector2 gravityVector;
	[SerializeField] protected float gravChangeSpeed;
	[SerializeField] protected float currentGravVector;
	[Space]
	[SerializeField] protected float maxHealth;
	[SerializeField] protected float currentHealth;
	[SerializeField] protected UnityEvent onTakeDamage;
	[SerializeField] protected UnityEvent onTakeHeavyDamage;
	[SerializeField] protected UnityEvent onLaunch;
	[SerializeField] protected UnityEvent onLand;
	
	protected bool alive;
	protected bool airborne;
	protected bool grounded;
	protected bool prevGrounded;
	protected bool useGravity;
	
	protected Coroutine gravCoroutine;
	protected Coroutine reactivateAICoroutine;
	
	protected void Awake() {
		currentHealth = maxHealth;
		alive = true;
	}
	
	protected void Start()
	{
		objectPooler = ObjectPooler.Instance;
	}
	
	protected void Update()
	{
		grounded = Physics.CheckSphere(transform.position, 0.1f, groundMask, QueryTriggerInteraction.Ignore);
		
		if(grounded && !prevGrounded)
		{
			Land();
		}
		
		if(!grounded && !rb.isKinematic)
		{
			currentGravVector = Mathf.MoveTowards(currentGravVector, gravityVector.y, gravChangeSpeed * Time.deltaTime);
		}
		
		prevGrounded = grounded;
	}
	
	protected void FixedUpdate()
	{
		if(!rb.isKinematic && useGravity) 
		{
			rb.AddForce(Vector3.down * currentGravVector, ForceMode.Acceleration);
		}
	}
	
	public void Damage(int amount)
	{
		if(audioSource) audioSource.PlayOneShot(sounds.hurtSounds.sound[Random.Range(0, sounds.hurtSounds.sound.Length)], Random.Range(0.8f,1f));
		animator.SetFloat("hitType", Mathf.Round(Random.Range(0f,1f)));
		
		switch(amount)
		{
		case 1:
			onTakeDamage?.Invoke();
			break;
		case 2:
			onTakeHeavyDamage?.Invoke();
			if(blazeAI) blazeAI.Hit();
			break;
		case 3:
			onTakeHeavyDamage?.Invoke();
			Launch();
			break;
		}
		
		if(!prevGrounded) DelayGravity();
		currentHealth -= amount;
		if(maxHealth > 0 && currentHealth <= 0f && alive) Die();
	}
	
	protected void DelayGravity()
	{
		if(gravCoroutine != null) StopCoroutine(gravCoroutine);
		gravCoroutine = StartCoroutine(_DelayGravity());
		
		IEnumerator _DelayGravity()
		{
			useGravity = false;
			rb.velocity = Vector3.zero;
			yield return new WaitForSeconds(0.35f);
			useGravity = true;
		}
	}
	
	public void Launch()
	{
		if(!grounded) return;
		if(reactivateAICoroutine != null) StopCoroutine(reactivateAICoroutine);
		animator.ResetTrigger("land");
		airborne = true;
		// blazeAI.enabled = false;
		agent.enabled = false;
		animator.applyRootMotion = false;
		rb.isKinematic = false;
		useGravity = true;
		rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);
		currentGravVector = gravityVector.x;
		
		onLaunch?.Invoke();
	}
	
	private void Land()
	{
		airborne = false;
		animator.applyRootMotion = true;
		animator.SetTrigger("land");
		onLand?.Invoke();
		if(reactivateAICoroutine != null) StopCoroutine(reactivateAICoroutine);
		reactivateAICoroutine = StartCoroutine(_Reactivate());
		
		IEnumerator _Reactivate()
		{
			yield return new WaitForSeconds(2f);
			agent.enabled = true;
			blazeAI.enabled = true;
			rb.isKinematic = true;
			useGravity = false;
		}
	}
	
	public void Die()
	{
		if(!alive) return;
		alive = false;
		if(blazeAI) blazeAI.Death();

		StartCoroutine(FadeOut());
	}
	
	private IEnumerator FadeOut()
	{
		yield return new WaitForSeconds(3f);
		objectPooler.SpawnFromPool("enemyDeath", torso.position, Quaternion.identity);
		Destroy(gameObject);
	}
	
	public void Attack(SCR_Attack attack)
	{
		Collider[] hitColl = new Collider[10];
		int numColl = Physics.OverlapSphereNonAlloc(transform.position, attack.range, hitColl, attackMask);
		for (int i = 0; i < numColl; i++) {
			float angle = Vector3.Angle(transform.forward, (hitColl[i].transform.position - transform.position).normalized);
			if(angle <= attack.angle && hitColl[i].TryGetComponent(out IDamage iDamage))
			{
				iDamage.Damage(attack.damage);
				if(audioSource)
				{
					audioSource.PlayOneShot(attack.attackSound, Random.Range(0.65f,0.75f));
					audioSource.PlayOneShot(sounds.attackSounds.sound[Random.Range(0,sounds.attackSounds.sound.Length)], Random.Range(0.5f,0.75f));
				}
			}
		}
	}
}
