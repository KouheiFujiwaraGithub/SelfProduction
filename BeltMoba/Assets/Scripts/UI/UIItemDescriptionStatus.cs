using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemDescriptionStatus : MonoBehaviour
{
	[SerializeField]
	private Text mTitle;
	[SerializeField]
	private Text mValue;

	public void SetStatus(Common.ITEM_UNIT_STATUS _status, float _value)
	{
		switch(_status)	{
			case Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY:	mTitle.text = "HP回復 : ";	break;
			case Common.ITEM_UNIT_STATUS.STAMINA_RECOVERY:	mTitle.text = "SP回復 : ";	break;
			case Common.ITEM_UNIT_STATUS.HEALTH:			mTitle.text = "HP : ";		break;
			case Common.ITEM_UNIT_STATUS.STAMINA:			mTitle.text = "SP : ";		break;
			case Common.ITEM_UNIT_STATUS.ATTACK:			mTitle.text = "攻撃 : ";	break;
			case Common.ITEM_UNIT_STATUS.DEFENSE:			mTitle.text = "防御 : ";	break;
			case Common.ITEM_UNIT_STATUS.SPEED:				mTitle.text = "速さ : ";	break;
		}
		mValue.text = _value.ToString();
	}
}
