using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonDescription : MonoBehaviour {
	[SerializeField]
	private Slider mButtonPushedGuge;
	[SerializeField]
	private Text mDescriptionText;

	public void ButtonPushed(float _timer, float _limit)
	{
		mButtonPushedGuge.gameObject.SetActive(true);
		mButtonPushedGuge.value = _timer / _limit;
	}
	
	public void ResetButtonPushed()
	{
		mButtonPushedGuge.gameObject.SetActive(false);
		mButtonPushedGuge.value = 0f;
	}

	public void SetText(string _text)
	{
		mDescriptionText.text = _text;
	}
}
