using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
	public Animator Animator;
	private Text DamageText;

	private void SetText(string text)
	{
		DamageText = Animator.GetComponent<Text>();
		DamageText.text = text;
	}
}
