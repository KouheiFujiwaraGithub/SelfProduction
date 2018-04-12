using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodData : ItemData
{
	

	public FoodData(int _id){
		mId = _id;
		mKind = Common.ITEM_KIND.FOOD;
		if(GameData.Instance.FoodData.ContainsKey(mId.ToString())){
			var datas 		= GameData.Instance.FoodData[mId.ToString()];
			mName 			= datas["Name"];
			mDescription 	= datas["Description"];
			mExp			= float.Parse( datas["Exp"] );
			mSprite			= Resources.Load<Sprite>(datas["Asset"]);

			mItemStatus.Add(Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY, float.Parse(datas["HealthRecovery"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.STAMINA_RECOVERY, float.Parse(datas["StaminaRecovery"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.HEALTH, float.Parse(datas["Health"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.STAMINA, float.Parse(datas["Stamina"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.ATTACK, float.Parse(datas["Attack"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.DEFENSE, float.Parse(datas["Defense"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.SPEED, float.Parse(datas["Speed"]));
		}
	}
}
