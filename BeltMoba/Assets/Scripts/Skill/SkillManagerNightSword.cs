using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerNightSword : SkillManager
{
	[SerializeField]
	private SkillDecision mSkillDecision;

	void Start()
	{
		mSkillData = new SkillData(1);
	}

	protected override IEnumerator Ability()
	{
		mIsSkill = false;

		for(var i = 0; i < mSkillDecision.HitUnitList.Count; i++){
			var hitData = mSkillDecision.HitUnitList[i];
			var hitUnit = hitData.Unit;
			var hitPos  = hitData.Position;
			if(hitUnit.TeamId == mUnitManager.TeamId){
				continue;
			}
			hitUnit.OnDamage(mUnitManager, mSkillData, hitPos);
			var knockBackVelocity = (hitUnit.transform.position - mUnitManager.transform.position).normalized * 5f;
			hitUnit.gameObject.GetComponent<Rigidbody>().AddForce(knockBackVelocity, ForceMode.VelocityChange);
		}

		yield return new WaitForSeconds(mSkillData.RestTime);

		mIsSkill = true;
		yield return null;
	}
}
