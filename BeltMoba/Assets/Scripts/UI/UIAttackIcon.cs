using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAttackIcon : MonoBehaviour {
	[SerializeField]
	private Image mImgRest;
	[SerializeField]
	private Image mImgIcon;
	
	private bool mIsRest = false;

	public void Init(Sprite _iconSprite)
	{
		mImgIcon.sprite 	= _iconSprite;
		mIsRest 			= false;
		mImgRest.fillAmount = 0f;
	}

	public void OnRest(float _restTime)
	{
		StartCoroutine(Rest(_restTime));
	}

	private IEnumerator Rest(float _restTime)
	{
		if(mIsRest){
			yield break;
		}
		
		mIsRest = true;
		mImgRest.fillAmount = 1f;

		var timer = 0f;
		while(_restTime > timer){
			timer += Time.deltaTime;
			mImgRest.fillAmount =　1f - (timer / _restTime);
			yield return null;
		}
		mImgRest.fillAmount = 0f;

		mIsRest = false;
		yield return null;
	}
}
