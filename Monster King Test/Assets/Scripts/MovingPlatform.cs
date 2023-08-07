using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MovingPlatform : MonoBehaviour
{
	protected Vector3 startPosition;
	public bool bezier = false;
	[HideIf("bezier")] public Vector3 targetPosition;
	[ShowIf("bezier")] public Vector3[] bezierPositions;
	[Space]
	[SerializeField] protected float moveDuration = 1f;
	
	protected void Start() {
		startPosition = transform.position;
		if(bezier)
		{
			transform.DOPath(bezierPositions, moveDuration, PathType.CatmullRom).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(UpdateType.Fixed);
		}
		else
		{
			transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(UpdateType.Fixed);
		}
	}
	
	protected void OnTriggerEnter(Collider other) {
		other.transform.SetParent(transform);
	}
	
	protected void OnTriggerExit(Collider other) {
		other.transform.SetParent(null);
	}
}

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor
{
	protected void OnSceneGUI() {
		var movingPlatform = (MovingPlatform)target;
		if(movingPlatform == null) return;
		
		if(!movingPlatform.bezier)
		{
			movingPlatform.targetPosition = Handles.PositionHandle(movingPlatform.targetPosition, Quaternion.identity);
		
			Handles.color = Color.red;
			Handles.DrawDottedLine(movingPlatform.transform.position, movingPlatform.targetPosition, 10f);
		}
		else
		{
			if(movingPlatform.bezierPositions.Length > 0)
			{
				Handles.DrawDottedLine(movingPlatform.bezierPositions[0], movingPlatform.transform.position, 10f);
				
				for (int i = 0; i < movingPlatform.bezierPositions.Length; i++) {
					movingPlatform.bezierPositions[i] = Handles.PositionHandle(movingPlatform.bezierPositions[i], Quaternion.identity);
				
					Handles.color = Color.red;
					Handles.DrawDottedLine(movingPlatform.bezierPositions[i], movingPlatform.bezierPositions[i-1 >= 0 ? i-1 : 0], 10f);
				}
			}
		}
		
	}
}
