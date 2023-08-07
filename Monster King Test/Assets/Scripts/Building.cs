using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Building : Health
{
	[SerializeField] private GameObject destroyParticles;
	[SerializeField] private GameObject rubble;
	
	protected override void Die() {
		base.Die();
		Instantiate(destroyParticles, transform.position, Quaternion.identity);
		GameObject _rubble = Instantiate(rubble, transform.position + Vector3.down, transform.rotation);
		transform.DOLocalMoveY(-2f, 5f).SetEase(Ease.InQuint);
		_rubble.transform.DOLocalMoveY(0f, 5f).SetEase(Ease.OutSine);
	}
}
