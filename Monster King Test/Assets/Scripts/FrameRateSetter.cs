using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRateSetter : MonoBehaviour
{
	[SerializeField, Min(0)] protected int targetFrameRate = 60;
	[SerializeField] protected RenderTexture rt;
	[SerializeField] protected Camera cam;
	
	protected void Awake() {
		Application.targetFrameRate = targetFrameRate;
		rt.Release();
		rt.width = Screen.width;
		rt.height = Screen.height;
		rt.Create();
		cam.targetTexture = rt;
	}
}
