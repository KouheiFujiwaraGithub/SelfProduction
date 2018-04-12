using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {
	[SerializeField]
	private Text mWinLooseText;
	[SerializeField]
	private int mLocalPlayerCount;
	[SerializeField]
	private UnitManager mUnitTowerTeam_1;
	[SerializeField]
	private UnitManager mUnitTowerTeam_2;
	[SerializeField]
	private float mBattleRemainTime;
	void Awake()
	{
		GameData.Instance.Init();
		GameData.Instance.SetLocalPlayerCount(mLocalPlayerCount);
		GameData.Instance.SetUnitTower(mUnitTowerTeam_1, mUnitTowerTeam_2);

		StartCoroutine(CountBattleRemainTimer());
	}


	private IEnumerator CountBattleRemainTimer()
	{
		var timer = mBattleRemainTime;
		while(timer > 0f){
			GameData.Instance.SetBattleRemainTime(timer);
			timer -= Time.deltaTime;
			yield return null;
		}
		yield return null;
	}

}
