using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DishData : ItemData {
	/// <summary>IDから直接作成</summary>
	/// <param name="_id">料理ID</param>
	public DishData (int _id, float _exp = 0f, Dictionary<Common.ITEM_UNIT_STATUS, float> _statusDir = null){
		mId = _id;
		mKind = Common.ITEM_KIND.DISH;
		if(GameData.Instance.DishData.ContainsKey(mId.ToString())){
			var datas 		= GameData.Instance.DishData[mId.ToString()];
			mName 			= datas["Name"];
			mDescription 	= datas["Description"];
			mExp			= float.Parse(datas["Exp"]) + _exp;
			mSprite			= Resources.Load<Sprite>(datas["Asset"]);

			mItemStatus.Add(Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY, float.Parse(datas["HealthRecovery"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.STAMINA_RECOVERY, float.Parse(datas["StaminaRecovery"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.HEALTH, float.Parse(datas["Health"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.STAMINA, float.Parse(datas["Stamina"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.ATTACK, float.Parse(datas["Attack"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.DEFENSE, float.Parse(datas["Defense"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.SPEED, float.Parse(datas["Speed"]));

			foreach(var status in _statusDir){
				if(mItemStatus.ContainsKey(status.Key)){
					mItemStatus[status.Key] += status.Value;
				}else{
					mItemStatus.Add(status.Key, status.Value);
				}
			}
		}
	}
}
