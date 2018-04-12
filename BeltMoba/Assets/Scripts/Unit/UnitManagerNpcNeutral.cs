using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManagerNpcNeutral : UnitManagerNpc {
	[SerializeField]
	private ScoutUnitNpc mScoutUnitNpc;
	[SerializeField]
	private List<SkillManager> mSkillList = new List<SkillManager>();

	//	索敵範囲外での追従を数秒間行う
	private const float mExtraChaseTime = 3f;
	private UnitManager mTargetUnit;

	protected override void Init(){
		base.Init();
		
		mScoutUnitNpc.SetUnitManager(this);
		for(var i = 0; i < mSkillList.Count; i++){
			mSkillList[i].SetUnitManager(this);
		}

		StartCoroutine(UpdateUiNeutral());
	}

	public override void OnDamage(UnitManager _attackUnit, SkillData _skillData, Vector3 _hitPos)
	{
		base.OnDamage(_attackUnit, _skillData, _hitPos);
		if(mTargetUnit == null){
			mTargetUnit = _attackUnit;
		}
		mIsMoveJust = false;
	}

	protected override void Update()
	{
		base.Update();
		if(mUnitPlayerMaster != null){
			if(mUnitPlayerMaster.UnitBattleTarget != null && mUnitPlayerMaster.UnitBattleTarget.TeamId != mTeamId){
				mTargetUnit = mUnitPlayerMaster.UnitBattleTarget;
			}
			else{
				mTargetUnit = mUnitPlayerMaster;
			}
		}
	}

	private IEnumerator UpdateUiNeutral()
	{
		var extraChaseTimer = 0f;
		while(true){
			if(mTargetUnit != null){
				if(mScoutUnitNpc.FirstScoutUnit == mTargetUnit){
					extraChaseTimer = mExtraChaseTime;
				}
				var distance	= Vector3.Distance(mTargetUnit.transform.position, this.transform.position);
				var dir			= (mTargetUnit.transform.position - this.transform.position).normalized;
				dir.y = 0f;
				
				mBody.rotation = Quaternion.LookRotation(dir);

				var activeSkillList = mSkillList.Where(x=>x.SkillData.Range > distance).ToList();
				if(activeSkillList.Count <= 0){
					this.transform.position += dir * MoveValue;
				}
				else{
					if(mTargetUnit.TeamId != mTeamId){
						var randomWaitTime = Random.Range(1f,2f);
						var randomIndex = Random.Range(0, activeSkillList.Count);
						var activeSkill = activeSkillList[randomIndex];
						activeSkill.OnAbility(randomWaitTime);
						// if(activeSkill.IsSkill){
						// 	activeSkill.OnAbility(randomWaitTime);
						// }
					}
				}
				extraChaseTimer -= Time.deltaTime;
			}

			if(extraChaseTimer < 0f){
				mTargetUnit = null;
				mIsMoveJust = true;
			}
			yield return null;
		}
	}
}
