using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerTower : UnitManagerTeam {
	[SerializeField]
	private Transform mTeamSymbolParent;
	[SerializeField]
	private SkillManager mTowerShield;
	[SerializeField]
	private MeshRenderer mMatShieldArea;

	private float mRecoveryTime = 30f;
	void FixedUpdate(){
		if(mIsDead) {
			return;
		}
	}
	
	protected override void Init(){
		base.Init();
		
		mTowerShield.SetUnitManager(this);
		
		if(mTeamManager != null){
			var teamSymbol = (GameObject)Instantiate(mTeamManager.TeamSymbol);
			teamSymbol.transform.position = mTeamSymbolParent.position;
			teamSymbol.transform.SetParent(mTeamSymbolParent);
			teamSymbol.transform.localScale = new Vector3(1f,1f,1f);
		}

		if(mTeamId == 1){
			mMatShieldArea.material = GameData.Instance.MatShieldTeam_1;
		}
		else if(mTeamId == 2){
			mMatShieldArea.material = GameData.Instance.MatShieldTeam_2;
		}
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override IEnumerator Dead()
	{
		mIsDead = true;
		this.gameObject.GetComponent<Collider>().enabled = false;
		
		var timer = 0f;
		while(mRecoveryTime >= timer){
			timer += Time.deltaTime;
			mHpGauge.value = timer / mRecoveryTime;
			yield return null;
		}
		mCurrentHp = mStatus.Hp;

		this.gameObject.GetComponent<Collider>().enabled = true;
		mIsDead = false;
		yield return null;
	}

	void OnTriggerEnter(Collider _collider)
	{
		if(_collider.gameObject.tag == "ThrowItem") {
			var item = _collider.gameObject.GetComponent<ItemObject>();
			
			//	TODO: 直値でチーム内の所持アイテム最大数を決定している
			if(mTeamManager != null && mTeamManager.TeamProperty.Count < 20 && item.Data.Kind != Common.ITEM_KIND.DISH){
				mTeamManager.TeamProperty.Add(item.Data);
				Destroy(item.gameObject);
			}
		}
	}
}
