using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleStatus : MonoBehaviour {
	[SerializeField]
	private Slider mTeam_1;
	[SerializeField]
	private Slider mTeam_2;
	[SerializeField]
	private Text mBattleRemainTime;
	[SerializeField]
	private Text mBattleFinish;

	private bool mIsBattleFinish = false;

	void Update(){
		var unitTeam_1 = GameData.Instance.UnitTowerTeam_1;
		mTeam_1.value = unitTeam_1.CurrentHp / unitTeam_1.Status.Hp;
		
		var unitTeam_2 = GameData.Instance.UnitTowerTeam_2;
		mTeam_2.value = unitTeam_2.CurrentHp / unitTeam_2.Status.Hp;

		var remainTimer =  Mathf.FloorToInt(GameData.Instance.BattleRemaminTimer);
		mBattleRemainTime.text = remainTimer.ToString();

		if((unitTeam_1.CurrentHp <= 0f || unitTeam_2.CurrentHp <= 0f || remainTimer <= 0f) && !mIsBattleFinish){
			mIsBattleFinish = true;
			mBattleFinish.gameObject.SetActive(true);
			if(remainTimer <= 0f){
				if(unitTeam_1.CurrentHp > unitTeam_2.CurrentHp){
					mBattleFinish.text = "チーム1の勝利";
				}else if(unitTeam_1.CurrentHp < unitTeam_2.CurrentHp){
					mBattleFinish.text = "チーム2の勝利";
				}else{
					mBattleFinish.text = "引き分け";
				}
			}
			else if(unitTeam_1.CurrentHp <= 0f){
				mBattleFinish.text = "チーム2の勝利";
			}
			else if(unitTeam_2.CurrentHp <= 0f){
				mBattleFinish.text = "チーム1の勝利";
			}
		}
	}
}
