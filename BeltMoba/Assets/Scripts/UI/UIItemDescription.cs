using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescription : MonoBehaviour {
	[SerializeField]
	private Image mIcon;
	[SerializeField]
	private Text mName;
	[SerializeField]
	private Text mDescription;
	[SerializeField]
	private Text mExp;
	[SerializeField]
	private Transform mStatusContent;
	[SerializeField]
	private CanvasGroup mGroupDescription;
	[SerializeField]
	private CanvasGroup mGroupStatus;
	private float mSwitchTimer = 0f;
	
	private List<UIItemDescriptionStatus> mUIItemDescriptionStatus = new List<UIItemDescriptionStatus>();

	private ItemData mItemData;

	void Awake()
	{
		for(var i = 0; i < mStatusContent.childCount; i++){
			mUIItemDescriptionStatus.Add( mStatusContent.GetChild(i).GetComponent<UIItemDescriptionStatus>());
		}
	}

	void OnDisable()
	{
		mName.text			= "";
		mDescription.text	= "";
		mExp.text			= "";
		mIcon.color			= Color.clear;
	}

	public void SetItemData( ItemData _itemData ){
		mItemData = _itemData;
		
		if(mItemData == null){
			mName.text			= "";
			mDescription.text	= "";
			mExp.text			= "";
			mIcon.color			= Color.clear;
		}
		else{
			mName.text			= mItemData.Name;
			mDescription.text	= mItemData.Description;
			mExp.text			= mItemData.Exp + "exp";
			mIcon.color			= Color.white;
			mIcon.sprite		= mItemData.Sprite;

			var index = 0;
			foreach(var status in mItemData.ItemStatus){
				if(status.Value <= 0f){
					continue;
				}
				mUIItemDescriptionStatus[index].gameObject.SetActive(true);
				mUIItemDescriptionStatus[index].SetStatus(status.Key, status.Value);
				index++;
			}
			for(var i = index; i < mUIItemDescriptionStatus.Count; i++){
				mUIItemDescriptionStatus[i].gameObject.SetActive(false);
			}
			// StartCoroutine(SwitchCanvasGroup());
		}
	}

	private IEnumerator SwitchCanvasGroup()
	{
		var currentId = mItemData.Id;

		while(currentId == mItemData.Id){
			var feadIn	= 0f;
			var feadOut = 0f;
			var timer 	= 0f;
			var switchTime = 4f;
			while(timer < switchTime){
				if(currentId != mItemData.Id){
					yield break;
				}
				timer  += Time.deltaTime;
				feadIn	= timer / switchTime;
				feadOut	= 1f - (timer / switchTime);
				mGroupDescription.alpha = feadIn;
				mGroupStatus.alpha = feadOut;
				yield return null;
			}
			timer = 0f;
			while(timer < switchTime){
				if(currentId != mItemData.Id){
					yield break;
				}
				timer  += Time.deltaTime;
				feadIn	= timer / switchTime;
				feadOut	= 1f - (timer / switchTime);
				mGroupDescription.alpha = feadOut;
				mGroupStatus.alpha = feadIn;
				yield return null;
			}
			yield return null;
		}
		
		yield return null;
	}
}
