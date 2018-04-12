using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIItemObject : MonoBehaviour {
	[SerializeField]
	private Image mIcon;
	private ItemData mItemData;
	private RectTransform mThisRect;
	public ItemData ItemData { get { return mItemData; } }
	
	public bool IsData {get { return (mItemData != null) ? true : false; } }

	public void Init(ItemData _itemData){
		mThisRect = (RectTransform)this.transform;
		mItemData = _itemData;

		if(_itemData == null){
			mIcon.color = Color.clear;
			return;
		}

		mIcon.color = Color.white;
		mIcon.sprite = mItemData.Sprite;
	}
	
	// Update is called once per frame
	public void ChangeSize(Vector2 _size)
	{
		mThisRect.sizeDelta = _size;
		mIcon.rectTransform.sizeDelta = _size;
	}
}
