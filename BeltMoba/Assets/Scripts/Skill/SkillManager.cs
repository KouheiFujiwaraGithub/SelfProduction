using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {
	protected SkillData mSkillData;
	protected bool mIsSkill = true;
	protected UnitManager mUnitManager;
	public bool IsSkill { get { return mIsSkill; } }
	public SkillData SkillData {get {return mSkillData;} }
	
	public void SetUnitManager(UnitManager _unitManager){
		mUnitManager = _unitManager;
	}
	public void OnAbility(float _waitTime = 0f){
		if(mIsSkill){
			StartCoroutine(WaitAbility(_waitTime));
		}
	}
	protected virtual IEnumerator Ability(){
		yield return null;
	}

	private IEnumerator WaitAbility(float _waitTime)
	{
		mIsSkill = false;
		yield return new WaitForSeconds(_waitTime);
		yield return StartCoroutine(Ability());
	}
}
