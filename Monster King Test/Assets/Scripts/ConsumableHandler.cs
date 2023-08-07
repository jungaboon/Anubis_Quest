using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class ConsumableHandler : MonoBehaviour
{
	[SerializeField] protected PlayerTextHandler textHandler;
	[SerializeField] protected Health pHealth;
	[SerializeField] protected int pointCount;
	[SerializeField, ReadOnly] protected int[] keys = new int[3];
	
	[SerializeField] protected UnityEvent onAddHealth;
	[SerializeField] protected UnityEvent onAddPoint;
	[SerializeField] protected UnityEvent onAddTrophy;
	[SerializeField] protected UnityEvent onAddKey;
	[SerializeField] protected UnityEvent onTryLock;
	[SerializeField] protected UnityEvent onTryUnlock;
	
	public bool HasKey(ConsumableType c)
	{
		switch(c)
		{
		case ConsumableType.RED_KEY:
			if(keys[0] > 0)
			{
				onTryUnlock?.Invoke();
				return true;
			}
			else
			{
				onTryLock?.Invoke();
				textHandler.SetText($"I need a Red Key", 4f);
				return false;
			}
			break;
		case ConsumableType.GREEN_KEY:
			if(keys[1] > 0)
			{
				onTryUnlock?.Invoke();
				return true;
			}
			else
			{
				onTryLock?.Invoke();
				textHandler.SetText($"I need a Green Key", 4f);
				return false;
			}
			break;
		case ConsumableType.BLUE_KEY:
			if(keys[2] > 0)
			{
				onTryUnlock?.Invoke();
				return true;
			}
			else
			{
				onTryLock?.Invoke();
				textHandler.SetText($"I need a Blue Key", 4f);
				return false;
			}
			break;
		}
		
		return false;
	}
	
	public bool HasFullHealth()
	{
		return pHealth.HealthPercent() >= 1f;
	}
	
	public void Remove(ConsumableType c)
	{
		switch(c)
		{
		case ConsumableType.RED_KEY:
			keys[0]--;
			break;
		case ConsumableType.GREEN_KEY:
			keys[1]--;
			break;
		case ConsumableType.BLUE_KEY:
			keys[2]--;
			break;
		}
	}
	
	public void Add(ConsumableType c, GameObject g)
	{
		switch(c)
		{
		case ConsumableType.HEALTH:
			if(HasFullHealth()) return;
			pHealth.AddHealth(1);
			onAddHealth?.Invoke();
			Destroy(g);
			break;
		case ConsumableType.RED_KEY:
			keys[0]++;
			onAddKey?.Invoke();
			Destroy(g);
			break;
		case ConsumableType.BLUE_KEY:
			keys[1]++;
			onAddKey?.Invoke();
			Destroy(g);
			break;
		case ConsumableType.GREEN_KEY:
			keys[2]++;
			onAddKey?.Invoke();
			Destroy(g);
			break;
		case ConsumableType.TROPHY:
			onAddTrophy?.Invoke();
			Destroy(g);
			break;
		case ConsumableType.POINT:
			pointCount++;
			onAddPoint?.Invoke();
			Destroy(g);
			break;
		}
	}
}
