using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerTeleport : MonoBehaviour {
	[SerializeField]
	private UnitManagerPlayer mUnitManagerPlayer;
	[SerializeField]
	private Transform mSelecter;
	[SerializeField]
	private Transform[] mSelectTargets;

	private bool mIsSelect = true;
	private int mCurrentIndex = 0;

	void OnDisable()
	{
		mCurrentIndex = 0;
		mSelecter.position = mSelectTargets[mCurrentIndex].position;
		mIsSelect = true;
	}
	void Update()
	{
		var inputValue = (int)Input.GetAxis("joycon_vertical_" + mUnitManagerPlayer.PlayerId);
		if(inputValue != 0){
			StartCoroutine(Select(inputValue));
		}
		mSelecter.position = Vector3.Lerp(mSelecter.position, mSelectTargets[mCurrentIndex].position, 0.5f);
	}
	private IEnumerator Select(int value)
	{
		if(!mIsSelect){
			yield break;
		}
		mIsSelect = false;
		
		mCurrentIndex -= value;
		if(mSelectTargets.Length <= mCurrentIndex){
			mCurrentIndex = 0;
		}
		if(0 > mCurrentIndex){
			mCurrentIndex = mSelectTargets.Length -1;
		}
		yield return new WaitForSeconds(0.1f);
		mIsSelect = true;
	}
}
