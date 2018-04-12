using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutUnitPlayer : MonoBehaviour {
	[SerializeField]
	private UnitManagerPlayer mUnitPlayer;
	private List<UnitManager> mScoutUnitEnemyList = new List<UnitManager>();
	public UnitManager NearUnitEnemy{ get { return mScoutUnitEnemyList.OrderBy(x=>Vector3.Distance(x.transform.position, mUnitPlayer.transform.position)).FirstOrDefault(); } }

	void Update()
	{
		mScoutUnitEnemyList.Remove(null);
	}
	
	void OnTriggerStay(Collider _collider)
	{
		var layer = _collider.gameObject.layer;
		if(layer == (int)Common.GAMEOBJECT_LAYER.UNIT){
			var hitUnitManager = _collider.gameObject.GetComponent<UnitManager>();
			if(hitUnitManager.TeamId != mUnitPlayer.TeamId){
				if(Physics.Linecast (transform.parent.transform.position + Vector3.up, hitUnitManager.transform.position + Vector3.up, (int)Common.GAMEOBJECT_LAYER.FIELD)){
					if(mScoutUnitEnemyList.Contains(hitUnitManager)){
						mScoutUnitEnemyList.Remove(hitUnitManager);
					}
					return;
				}
				if(!mScoutUnitEnemyList.Contains(hitUnitManager)){
					mScoutUnitEnemyList.Add(hitUnitManager);
				}
			}
			else{
				if(mScoutUnitEnemyList.Contains(hitUnitManager)){
					mScoutUnitEnemyList.Remove(hitUnitManager);
				}
			}
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var layer = _collider.gameObject.layer;
		if(layer == (int)Common.GAMEOBJECT_LAYER.UNIT){
			var hitUnitManager = _collider.gameObject.GetComponent<UnitManager>();
			if(mScoutUnitEnemyList.Contains(hitUnitManager)){
				mScoutUnitEnemyList.Remove(hitUnitManager);
			}
		}
	}
}
