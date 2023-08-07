using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrainingDummy : MonoBehaviour, IDamage
{
	[SerializeField] protected UnityEvent onTakeDamage;
	
	public void Damage(int amount)
	{
		onTakeDamage?.Invoke();
	}
	
}
