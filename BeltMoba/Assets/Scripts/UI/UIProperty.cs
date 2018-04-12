using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIProperty : MonoBehaviour {
	[SerializeField]
	private Image mIcon;
	[SerializeField]
	private Text mAppointText;

	private ItemData mData;
	public ItemData Data { get{ return mData; } }

	private bool mIsEmpty = true;
	public bool IsEmpty { get { return mIsEmpty;} }

	private bool mIsAppoint = false;
	public bool IsAppoint { get { return mIsAppoint; } }

	public void SetData(ItemData _itemData, bool _isAppointClear){
		mData			= _itemData;
		mIcon.sprite	= mData.Sprite;
		mIcon.color		= Color.white;
		mIsEmpty 		= false;
		if(_isAppointClear){
			RemoveAppoint();
		}
	}

	public void OnEmpty()
	{
		mIsEmpty 	= true;
		mIsAppoint 	= false;
		mIcon.color	= Color.clear;
		RemoveAppoint();
	}

	public void OnAppoint()
	{
		mIsAppoint 	= true;
		mAppointText.gameObject.SetActive(true);
	}

	public void RemoveAppoint()
	{
		mIsAppoint	= false;
		mAppointText.gameObject.SetActive(false);
	}
}
