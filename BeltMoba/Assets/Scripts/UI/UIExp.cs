using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIExp : MonoBehaviour
{
	[SerializeField]
	private Slider mExpGuage;
	[SerializeField]
	private Text mExpValue;

	[SerializeField]
	private List<UIItemDescriptionStatus> mUIStatusList = new List<UIItemDescriptionStatus>();

	private float mExpGugaeMaxValue = 0f;

	void Start()
	{
		for(var i = 0; i < mUIStatusList.Count; i++){
			mUIStatusList[i].gameObject.SetActive(false);
		}
	}
	
	public void UIUpdateExp(float _current, float _max)
	{
		mExpGuage.value = _current / _max;
		mExpValue.text	= ((int)_current).ToString() + " / " + ((int)_max).ToString();
	}

	public void UIUpdateStatus(Dictionary<Common.ITEM_UNIT_STATUS, float> _itemStatus)
	{
		var index = 0;
		foreach(var status in _itemStatus){
			if(status.Value <= 0f || status.Key == Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY || status.Key == Common.ITEM_UNIT_STATUS.STAMINA_RECOVERY){
				continue;
			}
			mUIStatusList[index].gameObject.SetActive(true);
			mUIStatusList[index].SetStatus(status.Key, status.Value);
			index++;
		}
		for(var i = index; i < mUIStatusList.Count; i++){
			mUIStatusList[index].gameObject.SetActive(false);
		}
	}

	public void UILevelMax()
	{
		mExpGuage.value = 1f;
		mExpValue.text	= "<LVEL MAX>";
	}

	public void UIResetStatus()
	{
		for(var i = 0; i < mUIStatusList.Count; i++){
			mUIStatusList[i].gameObject.SetActive(false);
		}
	}
}
