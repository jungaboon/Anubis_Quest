using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Create SCR_Sound")]
public class SCR_Sounds : ScriptableObject
{
	[System.Serializable]
	public struct SoundType
	{
		public AudioClip[] sound;
	}
	
	public SoundType moveSounds;
	public SoundType attackSounds;
	public SoundType damageSounds;
	public SoundType hurtSounds;
	public SoundType miscSounds;
}
