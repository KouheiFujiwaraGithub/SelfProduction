using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnitProperty : MonoBehaviour 
{
	private List<ItemData> mPropertyList = new List<ItemData>();
	private int mPropertyMax = 0;

	[SerializeField]
	private Transform mUIItemObjectListParent;
	[SerializeField]
	private UIItemDescription mUIItemDescription;
	[SerializeField]
	private GameObject mUIButtonL;
	[SerializeField]
	private GameObject mUIButtonS;

	private List<UIItemObject> mUIItemObjectList = new List<UIItemObject>();
	private int mPrePropertyCount = 0;
	int mCurrentIndex = 0;
	public int CurrentPropertyIndex { get {return mUIItemObjectList[mCurrentIndex].IsData ? mCurrentIndex : -1; } }
	private bool mIsInput = false;
	private readonly Vector2 mSmallSizeDelta = new Vector2(100f,100f);
	private readonly Vector2 mBigSizeDelta = new Vector2(150f,150f);

	public bool IsInput { get { return mIsInput; } }
	public int PropertyMax { get { return mPropertyMax; } }
	public List<ItemData> PropertyList {get { return mPropertyList; } }

	public void Init()
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
		if(mPrePropertyCount != mPropertyList.Count){
			UpdatePropertyList();
		}
		
		mPrePropertyCount = mPropertyList.Count;
	}

	private void UpdateDescription()
	{
		if(mUIItemDescription == null || !mIsInput){
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

	public void UpdateInput(bool _isFeedRight){
		if(!mIsInput){
			return;
		}

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
	
		mUIItemObjectList[preIndex].ChangeSize(mSmallSizeDelta);
		mUIItemObjectList[mCurrentIndex].ChangeSize(mBigSizeDelta);
		UpdateDescription();
	}

	public void SetPropertyList(List<ItemData> _list, int _listMax)
	{
		mUIItemObjectList.Clear();
		
		mPropertyList	= _list;
		mPropertyMax	= _listMax;

		for(var i = 0; i < mUIItemObjectListParent.childCount && i <= mPropertyMax; i++){
			var childObj = mUIItemObjectListParent.GetChild(i).gameObject;
			if(childObj.tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.SCROLL_VIEW_CONTENT)){
				var uiItemObject = childObj.GetComponent<UIItemObject>();
				uiItemObject.gameObject.SetActive(true);
				mUIItemObjectList.Add(uiItemObject);
			}
		}
		
		for(var i = 0; i < mPropertyList.Count; i++){
			mUIItemObjectList[i].Init(mPropertyList[i]);
		}
		for(var i = mPropertyList.Count; i < mUIItemObjectList.Count; i++){
			mUIItemObjectList[i].Init(null);
		}
	}

	public void SetIsInput(bool _isInput)
	{
		mIsInput = _isInput;
		if(_isInput){
			for(var i = 0; i < mUIItemObjectList.Count; i++){
				if(i == mCurrentIndex){
					mUIItemObjectList[i].ChangeSize(mBigSizeDelta);
				}else{
					mUIItemObjectList[i].ChangeSize(mSmallSizeDelta);
				}
			}
			mUIButtonL.SetActive(true);
			mUIButtonS.SetActive(true);
			UpdateDescription();
		}
		else{
			for(var i = 0; i < mUIItemObjectList.Count; i++){
				mUIItemObjectList[i].ChangeSize(mSmallSizeDelta);
			}
			mUIButtonL.SetActive(false);
			mUIButtonS.SetActive(false);
		}
	}

	public void SwitchPropertyData(UIUnitProperty _toUIUnitProperty)
	{
		var itemObject = mUIItemObjectList[mCurrentIndex];
		if(!mIsInput || !itemObject.IsData || _toUIUnitProperty.PropertyList.Count >= _toUIUnitProperty.PropertyMax){
			Debug.Log("りたーん");
			return;
		}
		var itemData = itemObject.ItemData;
		_toUIUnitProperty.PropertyList.Add(itemObject.ItemData);
		_toUIUnitProperty.UpdatePropertyList();
		mPropertyList.Remove(itemData);
	}
}
