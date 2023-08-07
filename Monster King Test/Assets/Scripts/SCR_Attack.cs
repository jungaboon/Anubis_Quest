using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Scriptable Objects/Create SCR_Attack")]
public class SCR_Attack : ScriptableObject
{
	public float range = 1.5f;
	public float angle = 60f;
	public int damage = 1;
	public AudioClip attackSound;
}
