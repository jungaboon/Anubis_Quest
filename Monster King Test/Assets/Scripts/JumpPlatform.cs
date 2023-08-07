using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class JumpPlatform : MonoBehaviour
{
	[SerializeField] protected Vector3 punchScale;
	[SerializeField] protected float punchDuration = 0.35f;
	[SerializeField] protected Vector3 addedVelocity;
	[SerializeField] protected UnityEvent onJump;
	
	protected void OnTriggerEnter(Collider other) {
		if(other.TryGetComponent(out StandardControllerThirdPerson cc))
		{
			if(cc.Grounded()) return;
			
			transform.DORewind();
			transform.DOPunchScale(punchScale, punchDuration, 1);
			cc.SetMomentum(Vector3.zero);
			Vector3 adjVel = transform.right * addedVelocity.x + transform.up * addedVelocity.y + transform.forward * addedVelocity.z;
			cc.AddMomentum(addedVelocity);
			
			onJump?.Invoke();
		}
	}
}
