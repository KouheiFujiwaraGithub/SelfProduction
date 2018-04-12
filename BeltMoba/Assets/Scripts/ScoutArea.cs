using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ScoutArea : MonoBehaviour {
	private List<UnitManager> mUnitTargetList = new List<UnitManager>();
	public List<UnitManager> UnitTargetList { get { return mUnitTargetList; } }

	void OnTriggerEnter(Collider _collider)
	{
		if(_collider.gameObject.tag == "Player" || _collider.gameObject.tag == "Minion") {
			var unitManager = _collider.gameObject.GetComponent<UnitManager>();
			if(!mUnitTargetList.Contains(unitManager)) {
				mUnitTargetList.Add(unitManager);
			}
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		if(_collider.gameObject.tag == "Player" || _collider.gameObject.tag == "Minion") {
			var unitManager = _collider.gameObject.GetComponent<UnitManager>();
			if(mUnitTargetList.Contains(unitManager)) {
				mUnitTargetList.Remove(unitManager);
			}
		}
	}
}
