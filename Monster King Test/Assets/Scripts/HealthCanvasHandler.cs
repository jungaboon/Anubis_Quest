using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class HealthCanvasHandler : MonoBehaviour
{
	[SerializeField] protected RectTransform hpCanvas;
	[SerializeField] protected Image[] healthBars;
	[SerializeField] protected int numHealthBars;
	[SerializeField] protected int maxHealthBars = 4;
	[Space]
	[SerializeField] protected float fillDuration = 1f;
	[SerializeField] protected Vector3 punchVector;
	
	protected void Start() {
		for (int i = 0; i < healthBars.Length; i++) {
			healthBars[i].fillAmount = 0f;
			healthBars[i].gameObject.SetActive(false);
		}
	}
	
	protected void Update() {
		if(Input.GetKeyDown(KeyCode.F))
		{
			SpawnHealthBar($"Enemy {numHealthBars+1}");
		}
		if(Input.GetKeyDown(KeyCode.G))
		{
			hpCanvas.DORewind();
			hpCanvas.DOPunchPosition(punchVector, 0.75f);
		}
	}
	
	public void SpawnHealthBar(string charName)
	{
		if(numHealthBars < maxHealthBars)
		{
			healthBars[numHealthBars].gameObject.SetActive(true);
			healthBars[numHealthBars].GetComponentInChildren<TextMeshProUGUI>().text = charName;
			healthBars[numHealthBars].DOFillAmount(1f, fillDuration).SetEase(Ease.InOutSine);
			numHealthBars++;
		}
	}
	
	public void DespawnHealthBar()
	{
		
	}
}
