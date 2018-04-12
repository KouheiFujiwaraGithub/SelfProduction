using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStatusData : MonoBehaviour
{
	private int mLevel = 1;
	private int mId;
	private List<Dictionary<string,string>> mSkillkDataList = new List<Dictionary<string,string>>();
	private List<string> mDropDataList = new List<string>();
	private List<float> mExpTableList = new List<float>();

	private Dictionary<Common.ITEM_UNIT_STATUS, float> mItemStatus = new Dictionary<Common.ITEM_UNIT_STATUS, float>();
	
	public int Level { get { return mLevel; } }
	public int Id { get { return mId; } }
	public List<Dictionary<string,string>> SkillkData { get { return mSkillkDataList;} }
	public float Hp { get { return mItemStatus[Common.ITEM_UNIT_STATUS.HEALTH] * GetCurrentLevelUpValue();} }
	public float Attack { get { return mItemStatus[Common.ITEM_UNIT_STATUS.ATTACK]  * GetCurrentLevelUpValue(); } }
	public float Defense { get { return mItemStatus[Common.ITEM_UNIT_STATUS.DEFENSE] * GetCurrentLevelUpValue(); } }
	public float Speed { get { return mItemStatus[Common.ITEM_UNIT_STATUS.SPEED]; } }
	public float Stamina { get { return mItemStatus[Common.ITEM_UNIT_STATUS.STAMINA] * GetCurrentLevelUpValue(); } }
	public List<string> DropDataList { get {return mDropDataList;} }
	public List<float> ExpTableList { get {return mExpTableList;} }
	public Dictionary<Common.ITEM_UNIT_STATUS, float> ItemStatus { get { return mItemStatus; } }

	public UnitStatusData(int _id){
		mId = _id;
		if(GameData.Instance.UnitStatusData.ContainsKey(mId.ToString())){
			var data = GameData.Instance.UnitStatusData[mId.ToString()];
			
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.HEALTH, float.Parse(data["Hp"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.STAMINA, float.Parse(data["Stamina"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.ATTACK, float.Parse(data["Attack"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.DEFENSE, float.Parse(data["Defense"]));
			mItemStatus.Add(Common.ITEM_UNIT_STATUS.SPEED, float.Parse(data["Speed"]));

			var dropData = data["DropList"];
			if(dropData != "0"){
				mDropDataList = dropData.Split(' ').ToList();
			}

			var expData = data["ExpLevelUp"];
			if(expData != "0"){
				expData = expData.Replace("\"","");
				var list = expData.Split(' ');
				for(var i = 0; i < list.Length; i++){
					mExpTableList.Add(float.Parse(list[i]));
				}
			}
		}
	}

	private float GetCurrentLevelUpValue()
	{
		return (((float)mLevel * 0.05f) * (float)mLevel) + 1f;
	}

	public void OnLevelUp(int _levelUpValue, Dictionary<Common.ITEM_UNIT_STATUS,float> _itemStatus)
	{
		mLevel += _levelUpValue;
		foreach(var status in _itemStatus){
			if(mItemStatus.ContainsKey(status.Key)){
				mItemStatus[status.Key] += status.Value;
			}else{
				mItemStatus.Add(status.Key, status.Value);
			}
		}
	}

	public ItemData GetDropItemData()
	{
		var totalWeight = mDropDataList.Sum(x=>int.Parse(x.Split(':')[1]));
		var value = Random.Range(1, totalWeight + 1);
		var itemId = 0;

        for (var i = 0; i < mDropDataList.Count; ++i){
			var id 		= int.Parse(mDropDataList[i].Split(':')[0]);
			var weight 	= int.Parse(mDropDataList[i].Split(':')[1]);

            if (weight >= value)
            {
				itemId = id;
                break;
            }
            value -= weight;
		}
        return new FoodData(itemId);
	}

	
}