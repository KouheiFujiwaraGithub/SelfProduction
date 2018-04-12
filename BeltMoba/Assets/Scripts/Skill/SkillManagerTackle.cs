using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerTackle : SkillManager
{
	[SerializeField]
	private SkillDecision mSkillDecision;
	
	void Start()
	{
		var isLoadSprite = mUnitManager is UnitManagerPlayer ? true : false;
		mSkillData = new SkillData(5, true);
	}

	protected override IEnumerator Ability()
	{
		mIsSkill = false;

		var velocity = mUnitManager.Body.transform.TransformDirection(new Vector3(0,1f,2f) * 5f);
		mUnitManager.gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

		List<UnitManager> damagedList = new List<UnitManager>();
		var timer = 0f;
		while(timer < 0.5f){
			timer += Time.deltaTime;

			for(var i = 0; i < mSkillDecision.HitUnitList.Count; i++){
				var hitData = mSkillDecision.HitUnitList[i];
				var hitUnit = hitData.Unit;				
				var hitPos	= hitData.Position;
				
				if(hitUnit.TeamId == mUnitManager.TeamId){
					continue;
				}
				if(!damagedList.Contains(hitUnit) && hitUnit.IsDamage){
					var blowOfVelocity = (hitUnit.transform.position - mUnitManager.transform.position).normalized;
					hitUnit.GetComponent<Rigidbody>().AddForce(blowOfVelocity * 10f, ForceMode.VelocityChange);
					hitUnit.OnDamage(mUnitManager, SkillData, hitPos);
					damagedList.Add(hitUnit);
				}
			}
			yield return null;
		}
		mUnitManager.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

		yield return new WaitForSeconds(mSkillData.RestTime -1f);

		mIsSkill = true;
		yield return null;
	}
}
