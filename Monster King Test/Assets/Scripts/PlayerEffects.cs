using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerEffects : MonoBehaviour
{
	[SerializeField] private CinemachineImpulseSource impulse;
	
	public void GenerateImpulse(float amplitude = 1f)
	{
		impulse.m_ImpulseDefinition.m_AmplitudeGain = amplitude;
		impulse.GenerateImpulse();
	}
}
