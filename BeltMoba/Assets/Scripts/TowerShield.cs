using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerShield : MonoBehaviour {
	[SerializeField]
	private UnitManagerTower mTowerManager;
	[SerializeField]
	private MeshRenderer mAreaMesh;

	private List<UnitManager> mShieldInEnemyList = new List<UnitManager>();
	
	void Start()
	{
		if(mTowerManager.TeamId == 1){
			mAreaMesh.material = GameData.Instance.MatShieldTeam_1;
		}
		else{
			mAreaMesh.material = GameData.Instance.MatShieldTeam_2;
		}
	}

	void Update()
	{
		mShieldInEnemyList.Remove(null);
	}

	void OnTriggerStay(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;

		if(layer == (int)Common.GAMEOBJECT_LAYER.UNIT){
			var hitUnit = _collider.gameObject.GetComponent<UnitManager>();
			if(hitUnit.TeamId != mTowerManager.TeamId){
				if(!mShieldInEnemyList.Contains(hitUnit)){
					mShieldInEnemyList.Add(hitUnit);
				}
			}
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var tag		= _collider.gameObject.tag;
		var layer	= _collider.gameObject.layer;
		if(layer == (int)Common.GAMEOBJECT_LAYER.UNIT){
			var hitUnit = _collider.gameObject.GetComponent<UnitManager>();
			if(mShieldInEnemyList.Contains(hitUnit)){
				mShieldInEnemyList.Remove(hitUnit);
			}
		}
	}
}
