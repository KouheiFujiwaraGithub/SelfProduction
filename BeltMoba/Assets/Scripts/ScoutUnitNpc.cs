using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutUnitNpc : MonoBehaviour
{
	private UnitManagerNpc mUnitNpc;
	public void SetUnitManager(UnitManagerNpc _unitNpc){
		mUnitNpc = _unitNpc;
	}

	private List<UnitManager> mScoutEnemyUnitList = new List<UnitManager>();
	public UnitManager FirstScoutUnit { get { return mScoutEnemyUnitList.Count > 0 ? mScoutEnemyUnitList[0] : null; } }

	void Update(){
		// mScoutEnemyUnitList.Remove(null);
		var removeTargetList = mScoutEnemyUnitList.Where(x=>x == null || x.TeamId == mUnitNpc.TeamId).ToList();
		foreach(var item in removeTargetList){
			mScoutEnemyUnitList.Remove(item);
		}
	}

	void OnTriggerStay(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		if(mUnitNpc == null){
			return;
		}
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.PLAYER) || tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.MINION)){
			var hitUnitManger = _collider.gameObject.GetComponent<UnitManager>();
			if(hitUnitManger.TeamId != mUnitNpc.TeamId && mUnitNpc != hitUnitManger){
				if(!mScoutEnemyUnitList.Contains(hitUnitManger)){
					mScoutEnemyUnitList.Add(hitUnitManger);
				}
			}
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		if(mUnitNpc == null){
			return;
		}
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.PLAYER) || tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.MINION)){
			var hitUnitManger = _collider.gameObject.GetComponent<UnitManager>();
			if(hitUnitManger.TeamId != mUnitNpc.TeamId){
				if(mScoutEnemyUnitList.Contains(hitUnitManger)){
					mScoutEnemyUnitList.Remove(hitUnitManger);
				}
			}
		}
	}
}
