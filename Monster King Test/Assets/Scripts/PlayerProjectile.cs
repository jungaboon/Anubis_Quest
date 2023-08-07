using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerProjectile : MonoBehaviour
{
	protected enum ProjectileState
	{
		SEEKING,
		RETURNING
	}
	
	[SerializeField] protected ProjectileState state;
	[SerializeField] protected float speed;
	[SerializeField] protected float returnDistance;
	[SerializeField] protected Transform origin;
	[SerializeField] protected MeshRenderer originalRenderer;
	[SerializeField] protected PlayerAttackHandler pl;
	[SerializeField] protected List<Transform> currentTargets = new List<Transform>();
	
	[Space]
	[SerializeField] protected UnityEvent onLaunch;
	[SerializeField] protected UnityEvent onHit;
	[SerializeField] protected UnityEvent onEquip;
	
	protected Coroutine seekCoroutine;
	protected int damageAmount = 1;
	
	protected void Start() {
		gameObject.SetActive(false);
	}
	
	public void Launch(List<Transform> t, int d)
	{
		gameObject.SetActive(true);
		originalRenderer.enabled = false;
		onLaunch?.Invoke();
		
		damageAmount = d;
		currentTargets.Clear();
		currentTargets = t;
		state = ProjectileState.SEEKING;
		if(seekCoroutine != null) StopCoroutine(seekCoroutine);
		seekCoroutine = StartCoroutine(_Launch());
		
		IEnumerator _Launch()
		{
			transform.SetParent(null);
			float time = 0f;
			
			while(gameObject.activeSelf)
			{
				switch(state)
				{
				case ProjectileState.SEEKING:
					if(currentTargets.Count > 0)
					{
						transform.LookAt(currentTargets[0].position + Vector3.up * 1.25f, Vector3.down);
						transform.position = Vector3.MoveTowards(transform.position, currentTargets[0].position + Vector3.up * 1.35f, speed * Time.deltaTime);
					}
					else state = ProjectileState.RETURNING;
					break;
				
				case ProjectileState.RETURNING:
					transform.position = Vector3.MoveTowards(transform.position, origin.position, speed * Time.deltaTime);
					
					float dist = Vector3.Distance(transform.position, origin.position);
					if(dist <= returnDistance)
					{
						onEquip?.Invoke();
						pl.SpecialAttackReturn();
						if(seekCoroutine != null) StopCoroutine(seekCoroutine);
						originalRenderer.enabled = true;
						transform.SetParent(origin);
						transform.localPosition = Vector3.zero;
						gameObject.SetActive(false);
					}
					break;
				}
				
				yield return null;
			}
			
		}
	}
	
	protected void OnTriggerEnter(Collider other) {
		switch (other.gameObject.layer)
		{
		case 7:
			if(other.TryGetComponent(out IDamage iDamage) && currentTargets.Contains(other.transform))
			{
				onHit?.Invoke();
				iDamage.Damage(damageAmount);
				
				int ind = currentTargets.IndexOf(other.transform);
				currentTargets.Remove(currentTargets[ind]);
			}
			break;
		}
		
	}
}
