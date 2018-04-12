using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManagerDoge : SkillManager {
	[SerializeField]
	private ParticleSystem mParticleSsytem;
	private float mInvincibleTime = 0.1f;
	private Rigidbody mUnitRigidbody;
	
	void Start()
	{
		mSkillData = new SkillData(3);
		mParticleSsytem.Stop();
	}

	protected override IEnumerator Ability()
	{
		mIsSkill = false;
		mParticleSsytem.Play();

		var velocity = this.transform.TransformDirection(Vector3.forward * 10f);
		mUnitManager.gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
		
		yield return new WaitForSeconds(mSkillData.RestTime);
		mIsSkill = true;
	}
}
