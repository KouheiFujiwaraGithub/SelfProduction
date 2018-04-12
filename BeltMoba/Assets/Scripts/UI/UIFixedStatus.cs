using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFixedStatus : MonoBehaviour
{
	[SerializeField]
	private UnitManager mTargetUnit;
	[SerializeField]
	private UIItemDescriptionStatus[] mUIStatusList;

	void Update()
	{
		var status = mTargetUnit.Status;
		mUIStatusList[0].SetStatus(Common.ITEM_UNIT_STATUS.HEALTH, status.Hp);
		mUIStatusList[1].SetStatus(Common.ITEM_UNIT_STATUS.STAMINA, status.Stamina);
		mUIStatusList[2].SetStatus(Common.ITEM_UNIT_STATUS.ATTACK, status.Attack);
		mUIStatusList[3].SetStatus(Common.ITEM_UNIT_STATUS.DEFENSE, status.Defense);
		mUIStatusList[4].SetStatus(Common.ITEM_UNIT_STATUS.SPEED, status.Speed);
	}
}
