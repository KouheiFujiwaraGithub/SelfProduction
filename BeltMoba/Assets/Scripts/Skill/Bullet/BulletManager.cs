using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {
	[SerializeField]
	private bool mIsPenetration = false;
	private UnitManager mInvocatingUnit;
	private SkillData mSkillData;


	public void OnShoot(UnitManager _unitManager, Vector3 _startPos ,Vector3 _velocity, SkillData _skillData)
	{
		mSkillData 		= _skillData;
		mInvocatingUnit = _unitManager;

		this.transform.position = _startPos;

		StartCoroutine(Shoot(_velocity));
	}

	protected virtual IEnumerator Shoot(Vector3 _velocity)
	{
		var startPos = mInvocatingUnit.transform.position;	
		var distance = 0f;
		while(distance < mSkillData.Range){
			distance = Vector3.Distance(startPos, this.transform.position);
			this.transform.position += _velocity;
			yield return null;
		}
		OnDead();
	}

	public void OnDead(){
		Destroy(this.gameObject);
	}

	public void OnTriggerEnter(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;
		if(Common.IsDamageUnitObjectTagLayer(tag,layer)) {
			var hitUnit = _collider.gameObject.GetComponent<UnitManager>();
			if(mInvocatingUnit.TeamId != hitUnit.TeamId) {
				var hitPos = _collider.ClosestPointOnBounds(this.transform.position);
				hitUnit.OnDamage(mInvocatingUnit, mSkillData, hitPos);
				if(!mIsPenetration){
					OnDead();
				}
			}
		}
		else if(layer == (int)Common.GAMEOBJECT_LAYER.FIELD){
			OnDead();
		}
	}
}
