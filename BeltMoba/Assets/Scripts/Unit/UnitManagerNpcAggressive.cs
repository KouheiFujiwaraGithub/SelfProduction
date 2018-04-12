using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManagerNpcAggressive : UnitManagerNpc {
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
		StartCoroutine(UpdateUiAggressive());
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

	private IEnumerator UpdateUiAggressive()
	{
		var extraChaseTimer = 0f;
		while(true){
			//	索敵範囲内に発見 → 追従 → スキル射程内なら攻撃
			if(mScoutUnitNpc.FirstScoutUnit != null){
				extraChaseTimer = mExtraChaseTime;
				mIsMoveJust	= false;
				if(mUnitPlayerMaster != null 
					&& mUnitPlayerMaster.UnitBattleTarget != null 
					&& mUnitPlayerMaster.UnitBattleTarget.TeamId != mTeamId){
						mTargetUnit = mUnitPlayerMaster.UnitBattleTarget;
				}
				else{
					mTargetUnit	= mScoutUnitNpc.FirstScoutUnit;
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
					var randomWaitTime = Random.Range(0.5f, 2f);
					var randomIndex = Random.Range(0, activeSkillList.Count);
					var activeSkill = activeSkillList[randomIndex];
					activeSkill.OnAbility(randomWaitTime);
				}
			}
			//	追従　→ 索敵
			else if(extraChaseTimer > 0f && mTargetUnit != null && mTargetUnit.TeamId != mTeamId){
				var dir = (mTargetUnit.transform.position - this.transform.position).normalized;
				dir.y	= 0f;
				if(dir != Vector3.zero){
					mBody.rotation = Quaternion.LookRotation(dir);
					this.transform.position += dir * MoveValue;
				}
				extraChaseTimer -= Time.deltaTime;
			}
			//	索敵 → ランダム移動
			else{
				extraChaseTimer = 0f;
				mIsMoveJust = true;
			}
			yield return null;
		}
	}
}
