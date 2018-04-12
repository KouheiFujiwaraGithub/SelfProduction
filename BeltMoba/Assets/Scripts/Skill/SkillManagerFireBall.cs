using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerFireBall : SkillManager {
	[SerializeField]
	private BulletManager mBulletFireBall;
	private const float mBulletSpeed = 1f;
	void Start()
	{
		mSkillData = new SkillData(2);
	}
	
	protected override IEnumerator Ability()
	{
		mIsSkill = false;

		var bulletObj = (GameObject)Instantiate(mBulletFireBall.gameObject);
		var bullet	  = bulletObj.GetComponent<BulletManager>();
		
		var velocity = this.transform.TransformDirection(Vector3.forward * mBulletSpeed);
		bullet.OnShoot(mUnitManager, this.transform.position, velocity, mSkillData);

		yield return new WaitForSeconds(mSkillData.RestTime);
		
		mIsSkill = true;
		yield return null;
	}

}
