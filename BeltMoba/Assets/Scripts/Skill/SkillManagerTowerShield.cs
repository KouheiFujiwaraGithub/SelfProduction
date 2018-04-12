using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerTowerShield : SkillManager 
{
	[SerializeField]
	private SkillDecision mSkillDecision;
	void Start()
	{
		mSkillData = new SkillData(6,false);
	}

	void Update()
	{
		var hitEnemyArray = mSkillDecision.HitUnitList.Where(x=> x.Unit.TeamId != mUnitManager.TeamId).ToArray();
		if(hitEnemyArray.Length > 0){
			OnAbility();
		}
	}

	protected override IEnumerator Ability()
	{
		mIsSkill = false;

		var hitEnemyArray = mSkillDecision.HitUnitList.Where(x=> x.Unit.TeamId != mUnitManager.TeamId).ToArray();

		var attackRate = 1f - (hitEnemyArray.Length * 0.1f);
		var attack = attackRate * mUnitManager.Status.Attack;

		for(var i = 0; i < hitEnemyArray.Length; i++){
			var hitData = hitEnemyArray[i];
			var hitUnit = hitData.Unit;
			var hitPos  = hitData.Position;
			
			hitUnit.OnDamage(mUnitManager, mSkillData, hitPos);
		}

		yield return new WaitForSeconds(mSkillData.RestTime);

		mIsSkill = true;
		yield return null;
	}
}
