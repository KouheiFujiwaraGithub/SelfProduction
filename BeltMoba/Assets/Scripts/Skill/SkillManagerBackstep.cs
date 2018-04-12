using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerBackstep : SkillManager {
	void Start()
	{
		mSkillData = new SkillData(4);
	}
	protected override IEnumerator Ability()
	{
		mIsSkill = false;

		var velocity = this.transform.TransformDirection(Vector3.back * 10f);
		mUnitManager.gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
		
		yield return new WaitForSeconds(mSkillData.RestTime);

		mIsSkill = true;
	}
}
