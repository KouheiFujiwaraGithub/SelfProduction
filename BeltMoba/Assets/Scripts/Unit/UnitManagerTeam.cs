using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerTeam : UnitManager {
	protected TeamManager mTeamManager;
	protected Color mTeamColor;
	
	public void SetTeamManager(TeamManager _teamManager){
		mTeamManager = _teamManager;
	}

	override protected void Init(){
		base.Init();
		if(mTeamManager != null){
			SetTeamId();
		}else{
			mTeamId = 0;
		}
	}

	private void SetTeamId(){
		if(mTeamManager.TeamId == 1){
			mTeamColor = Color.red;
		}else{
			mTeamColor = Color.blue;
		}
		mHpGauge.fillRect.gameObject.GetComponent<Image>().color = mTeamColor;

		mTeamId = mTeamManager.TeamId;
	}
}
