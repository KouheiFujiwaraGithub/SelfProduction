using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour {
	[SerializeField]
	protected int mCharId;
	[SerializeField]
	protected Transform mBody;
	[SerializeField]
	protected Canvas mUnitUi;
	[SerializeField]
	protected Slider mHpGauge;
	protected UnitStatusData mStatus;
	protected bool mIsDaamge = true;
	public UnitStatusData Status { get { return mStatus; } }

	protected float MoveValue { get { return mStatus.Speed * Time.deltaTime; } }
	protected float mCurrentHp;
	public float CurrentHp { get { return mCurrentHp; } }

	protected bool mIsDead = false;
	public bool IsDead { get { return mIsDead; } }

	protected int mTeamId = 0;
	public int TeamId { get { return mTeamId; } }

	public Transform Body { get { return mBody; } }

	public bool IsDamage {get {return mIsDaamge;} }

	void Start()
	{
		Init();
	}

	protected virtual void Update()
	{
		if(mCurrentHp < 0) {
			OnDead();
		}

		if(!mIsDead){
			mHpGauge.value = mCurrentHp / mStatus.Hp;
		}
	}

	public void OnDead()
	{
		if(mIsDead) {
			return;
		}
		StartCoroutine(Dead());
	}

	public virtual void OnDamage(UnitManager _attackUnit, SkillData _skillData, Vector3 _hitPos)
	{
		if(!mIsDaamge){
			return;
		}
		//TODO:ドラクエ式（ 攻撃力*定数1 - 防御力*定数2 ）*補正
		var attack		= _attackUnit.Status.Attack + _skillData.ExtraAttack;
		var defense 	= mStatus.Defense;
		var damage 		= (attack * 0.5f) - (defense * 0.25f) * 1;
		if(damage <= 0f){
			damage = 1f;
		}
		
		mCurrentHp 	   -= damage;

		//	ダメージ数値
		if(mUnitUi != null){
			var damageDrawObj = (GameObject)Instantiate(GameData.Instance.UIDamageDraw);
			damageDrawObj.transform.SetParent(mUnitUi.transform);
			damageDrawObj.GetComponent<UIDamgeDraw>().Init(damage,_hitPos);
		}

		//	ダメージエフェクト
		var gameObj = (GameObject)Instantiate(GameData.Instance.EffectData[GameData.EFFECT_TYPE.EFFECT_UNIT_DAMAGE_NORMAL]);
		gameObj.transform.position = _hitPos;
	}

	public void OnLifeHeal(float _heal)
	{
		if(mCurrentHp <= mStatus.Hp) {
			mCurrentHp += _heal;
		}
	}

	protected virtual void Init()
	{
		mStatus = new UnitStatusData(mCharId);
		mCurrentHp = mStatus.Hp; 
	}

	protected virtual IEnumerator Dead(){ yield return null; }
}
