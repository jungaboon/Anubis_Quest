using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class LockedObject : MonoBehaviour
{
	[SerializeField] protected ConsumableType keyRequired;
	[SerializeField] protected Transform unlockTarget;
	[SerializeField] protected bool unlockSlide;
	[SerializeField] protected Vector3 unlockTransition;
	[SerializeField] protected Ease easeType;
	[SerializeField] protected float unlockSpeed = 1f;
	[Space]
	[SerializeField] protected UnityEvent onTryLock;
	[SerializeField] protected UnityEvent onUnlock;
	protected bool activated;
	
	protected void Awake()
	{
		activated = false;
	}
	
	
	protected void OnTriggerEnter(Collider other) {
		if(activated) return;
		if(other.TryGetComponent(out ConsumableHandler ch))
		{
			if(ch.HasKey(keyRequired))
			{
				ch.Remove(keyRequired);
				onUnlock?.Invoke();
				
				if(unlockTarget)
				{
					if(unlockSlide)
					{
						unlockTarget.DOMove(unlockTransition, unlockSpeed).SetEase(easeType);
					}
					else
					{
						unlockTarget.DORotate(unlockTransition, unlockSpeed, RotateMode.LocalAxisAdd).SetEase(easeType);
					}
					activated = true;
				}
			}
			else
			{
				onTryLock?.Invoke();
			}
		}
	}
}
