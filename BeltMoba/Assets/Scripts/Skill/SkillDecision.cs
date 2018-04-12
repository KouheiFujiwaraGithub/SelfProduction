using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SkillDecision : MonoBehaviour {
	[SerializeField]
	private bool mIsHitStayUpdate = false;
	public class HitData{
		public UnitManager Unit {get; private set;}
		public Vector3 Position {get; private set;}
		public HitData(UnitManager _hitUnit, Vector3 _hitPos){
			Unit		= _hitUnit;
			Position	= _hitPos;
		}
		public void UpdatePosition(Vector3 _hitPos){
			Position = _hitPos;
		}
	}

	private List<HitData> mHitUnitList = new List<HitData>();
	public List<HitData> HitUnitList { get { return mHitUnitList; } }
	
	private Collider mCollider;

	void Awake()
	{
		mCollider = this.gameObject.GetComponent<Collider>();
	}

	void Update()
	{
		var removeTargetList = HitUnitList.Where(x=>x.Unit == null).ToList();
		foreach(var item in removeTargetList){
			mHitUnitList.Remove(item);
		}
	}
	
	void OnTriggerEnter(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;

		if(Common.IsDamageUnitObjectTagLayer(tag, layer)){
			var unitManager = _collider.gameObject.GetComponent<UnitManager>();
			var hitPosition = _collider.ClosestPointOnBounds(this.transform.position);
			var hitData = new HitData(unitManager, hitPosition);
			if(!mHitUnitList.Contains(hitData)){
				mHitUnitList.Add(hitData);
			}
		}
	}
	
	void OnTriggerExit(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;

		if(Common.IsDamageUnitObjectTagLayer(tag, layer)){
			var hitData = mHitUnitList.Where(x=>x.Unit == _collider.gameObject.GetComponent<UnitManager>()).First();
			if(hitData != null){
				mHitUnitList.Remove(hitData);
			}
		}
	}

	void OnTriggerStay(Collider _collider)
	{
		if(mIsHitStayUpdate){
			var tag = _collider.gameObject.tag;
			var layer = _collider.gameObject.layer;

			if(Common.IsDamageUnitObjectTagLayer(tag, layer)){
				var unitManager = _collider.gameObject.GetComponent<UnitManager>();
				var hitPosition = _collider.ClosestPointOnBounds(this.transform.position);
				var hitData 	= mHitUnitList.Where(x=>x.Unit == _collider.gameObject.GetComponent<UnitManager>()).First();
				hitData.UpdatePosition(hitPosition);
			}
		}
	}
}
