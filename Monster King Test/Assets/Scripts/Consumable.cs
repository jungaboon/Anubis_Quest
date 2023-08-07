using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ConsumableType
{
	POINT,
	TROPHY,
	HEALTH,
	RED_KEY,
	BLUE_KEY,
	GREEN_KEY
}
public class Consumable : MonoBehaviour
{
	[SerializeField] protected ConsumableType cType;
	[SerializeField] protected Transform target;
	
	protected void Start() {
		if(target)
		{
			target.DORotate(new Vector3(0f,360f,0f), 3f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
		}
	}
	
	protected void OnTriggerEnter(Collider other) {
		if(other.TryGetComponent(out ConsumableHandler ch))
		{
			ch.Add(cType, gameObject);
		}
	}
}
