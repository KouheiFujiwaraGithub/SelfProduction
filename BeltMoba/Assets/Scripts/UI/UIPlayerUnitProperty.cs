using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayerUnitProperty : MonoBehaviour {
	// [SerializeField]
	// private UnitManagerPlayer mUnitPlayer;
	
	private List<ItemData> mPropertyList = new List<ItemData>();
	private int mPropertyMax = 0;

	[SerializeField]
	private Transform mUIItemObjectListParent;
	[SerializeField]
	private UIItemDescription mUIItemDescription;

	private List<UIItemObject> mUIItemObjectList = new List<UIItemObject>();
	private int mPrePlayerPropertyCount = 0;
	int mCurrentIndex = 0;
	public int CurrentPropertyIndex { get {return mUIItemObjectList[mCurrentIndex].IsData ? mCurrentIndex : -1; } }

	void Start()
	{
		var scrollContentCnt = 0;
		for(var i = 0; i < mUIItemObjectListParent.childCount; i++){
			var obj = mUIItemObjectListParent.GetChild(i).gameObject;

			if(scrollContentCnt < mPropertyMax && obj.tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.SCROLL_VIEW_CONTENT)){
				var uiItemObject = obj.GetComponent<UIItemObject>();
				
				uiItemObject.gameObject.SetActive(true);
				uiItemObject.Init(null);

				mUIItemObjectList.Add(uiItemObject);
				scrollContentCnt++;
			}
		}
	}

	void Update()
	{
		if(mPrePlayerPropertyCount != mPropertyList.Count){
			UpdatePropertyList();
		}
		
		mPrePlayerPropertyCount = mPropertyList.Count;
	}

	private void UpdatePropertyList()
	{
		for(var i = 0; i < mPropertyList.Count; i++){
			mUIItemObjectList[i].Init(mPropertyList[i]);
		}
		for(var i = mPropertyList.Count; i < mUIItemObjectList.Count; i++){
			mUIItemObjectList[i].Init(null);
		}
		if(mPropertyList.Count != 0 && mCurrentIndex > mPropertyList.Count -1){
			UpdateInput(false);
		}
		UpdateDescription();
	}

	private void UpdateDescription()
	{
		if(mUIItemDescription == null){
			return;
		}

		var currentData = mUIItemObjectList[mCurrentIndex];
		if(currentData.IsData){
			mUIItemDescription.gameObject.SetActive(true);
			mUIItemDescription.SetItemData(currentData.ItemData);
		}else{
			mUIItemDescription.gameObject.SetActive(false);
		}
	}

	public void SetPropertyList(List<ItemData> _list, int _listMax)
	{
		mPropertyList	= _list;
		mPropertyMax	= _listMax;
	}
	
	public void UpdateInput(bool _isFeedRight){
		var preIndex = mCurrentIndex;
		if(_isFeedRight){
			mCurrentIndex++;
			if(mCurrentIndex > mUIItemObjectList.Count-1){
				mCurrentIndex = 0;
			}
		}
		else {
			mCurrentIndex--;
			if(mCurrentIndex < 0){
				mCurrentIndex = mUIItemObjectList.Count-1;
			}			
		}
		var preSize = ((RectTransform)mUIItemObjectList[mCurrentIndex].transform).sizeDelta;
		var currentSize = ((RectTransform)mUIItemObjectList[preIndex].transform).sizeDelta;
	
		mUIItemObjectList[preIndex].ChangeSize(preSize);
		mUIItemObjectList[mCurrentIndex].ChangeSize(currentSize);
		UpdateDescription();
	}
}
