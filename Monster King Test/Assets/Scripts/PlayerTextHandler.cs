using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTextHandler : MonoBehaviour
{
	protected Camera cam;
	protected Coroutine textCoroutine;
	
	[SerializeField] protected Transform textTargetPosition;
	[SerializeField] protected TextMeshProUGUI dialogue;
	
	protected void Awake() {
		cam = Camera.main;
		dialogue.gameObject.SetActive(false);
	}
	
	public void SetText(string s, float duration)
	{
		dialogue.gameObject.SetActive(true);
		dialogue.text = s;
		
		if(textCoroutine != null) StopCoroutine(textCoroutine);
		textCoroutine = StartCoroutine(_SetTextPosition());
		
		IEnumerator _SetTextPosition()
		{
			float t = 0f;
			while(t < duration)
			{
				dialogue.rectTransform.position = cam.WorldToScreenPoint(textTargetPosition.position);
				t += Time.deltaTime;
				yield return null;
			}
			dialogue.gameObject.SetActive(false);

		}
	}
	
	
}
