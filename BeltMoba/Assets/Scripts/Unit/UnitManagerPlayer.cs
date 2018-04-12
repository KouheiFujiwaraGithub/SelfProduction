using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerPlayer : UnitManagerTeam
{
	[SerializeField]
	private int mPlayerId;
	[SerializeField]
	private FollowTargetCamera mFollowTargetCamera;
	[SerializeField]
	private UIUnitProperty mUIPlayerUnitProperty;
	[SerializeField]
	private UIUnitProperty mUICreateUnitProperty;
	[SerializeField]
	private UIUnitCreateLocationDish mUIUnitCreateLocationDish;
	[SerializeField]
	private UIButtonDescription mUIButtonDescription;
	[SerializeField]
	private Slider mStaminaGauge;
	[SerializeField]
	private List<SkillManager> mSkillMamagerList = new List<SkillManager>();
	[SerializeField]
	private List<UISkillIcon> mUISkillIconList = new List<UISkillIcon>();
	[SerializeField]
	private Collider mCollision;
	[SerializeField]
	private Transform mRespawnTarget;
	[SerializeField]
	private float mRespawnInterval;
	[SerializeField]
	private Text mUIStatusLevel;
	[SerializeField]
	private Slider mUIStatusHpGauge;
	[SerializeField]
	private UIExp mUIExp;
	[SerializeField]
	private UnitLeg mUnitLeg;
	[SerializeField]
	private SpriteRenderer mMinimapIcon;
	[SerializeField]
	private ParticleSystem mFxTireSmoke;
	[SerializeField]
	private GameObject mFxDoge;
	[SerializeField]
	private ScoutUnitPlayer mScoutUnitPlayer;
	[SerializeField]
	private AimBattleTarget mAimBatlleTarget;

	public int PlayerId { get { return mPlayerId; } }

	private enum UNIT_STATE {
		NONE 			= 0,
		IS_JUMP			= 1,
		IS_MOVE_DOWN	= 1 << 1,
		IS_GET_ITE 		= 1 << 2,
		IS_PUSE 		= 1 << 3,
		IS_HIT_LADDER 	= 1 << 4,
		IS_HIT_TOWER	= 1 << 5,
		IS_HIT_CREATE_LOCATION = 1 << 7,
		IS_HIT_DISH_LOCATION = 1 << 8,
		IS_PUSHED_ACTION_BUTTON = 1 << 9,
		IS_DASH = 1 << 10,
		CAN_DOGE = 1 << 11,
	}
	private UNIT_STATE mUnitState = UNIT_STATE.NONE | UNIT_STATE.IS_GET_ITE | UNIT_STATE.CAN_DOGE;

	private int mCurrentJumpCount = 0;
	private int mLimitJumpCount = 1;
	private Animation mAnimation;
	private List<ItemData> mPropertyList = new List<ItemData>();
	public int PropertyMax	{ get { return 7; } }
	private UnitManagerCreateLocation mCurrentCreateLocation;
	
	private float mInteractiveButtonTimer;
	private const float mInteractiveButtonSwitchTime = 0.5f;

	private float mActionButtonTimer;
	private const float mActionButtonSwitchTime = 1f;
	private const float mButtonSwtichGapTime = 0.3f;
	private float mCurrentStamina;
	private float mCurrentExp;
	private UnitManager mUnitBattleTarget;
	private Dictionary<Common.ITEM_UNIT_STATUS, float> mItemStatusDictionary = new Dictionary<Common.ITEM_UNIT_STATUS, float>();
	public UnitManager UnitBattleTarget { get {return mUnitBattleTarget; } }

	protected override void Init()
	{
		base.Init();

		mUIPlayerUnitProperty.SetPropertyList(mPropertyList, PropertyMax);
		mUIPlayerUnitProperty.SetIsInput(true);

		mCurrentStamina = mStatus.Stamina;

		if(mRespawnTarget != null){
			this.transform.position = mRespawnTarget.position;
			mBody.LookAt(mRespawnTarget.TransformDirection(Vector3.forward));
		}
		
		mFollowTargetCamera.OnPositionReset();

		mAnimation = mBody.gameObject.GetComponent<Animation>();

		mUIExp.UIUpdateExp(0f, mStatus.ExpTableList[0]);

		mMinimapIcon.color = mTeamColor;

		mUIStatusLevel.color = mTeamColor;
		mUIStatusHpGauge.fillRect.gameObject.GetComponent<Image>().color = mTeamColor;
		
		for(var i = 0; i < mSkillMamagerList.Count && i < mUISkillIconList.Count; i++){
			mSkillMamagerList[i].SetUnitManager(this);
			mUISkillIconList[i].SetSkillManager(mSkillMamagerList[i]);
		}

		mFxTireSmoke.Stop();
	}

	float mJumpFlame = 0f;
	void FixedUpdate()
	{
		if(mIsDead) {
			this.transform.GetComponent<Rigidbody>().isKinematic = true;
			return;
		}
		else {
			this.transform.GetComponent<Rigidbody>().isKinematic = false;
		}

		if(!GetIsUnitState(UNIT_STATE.IS_PUSE)) {
			//------------
			//	左右奥行き移動
			var moveX = Input.GetAxisRaw("joycon_horizontal_" + mPlayerId);
			var moveZ = Input.GetAxisRaw("joycon_vertical_" + mPlayerId) * 0.75f;	//	奥行きの移動を通常の半分に制限
			var moveY = 0f;
			if(GetIsUnitState(UNIT_STATE.IS_HIT_LADDER)) {
				this.transform.GetComponent<Rigidbody>().useGravity = false;
				if(moveZ != 0){
					moveY = Mathf.Abs(moveZ);
				}
				if(moveX != 0){
					moveY = Mathf.Abs(moveX);
				}
			}
			
			var moveVelocity = new Vector3(moveX, moveY, moveZ) * MoveValue;
			if( moveX != 0 && moveZ != 0 ){
				moveVelocity = moveVelocity * 0.75f;
			}

			if(GetIsUnitState(UNIT_STATE.IS_DASH)){
				moveVelocity = moveVelocity * 1.5f;
			}

			//------------
			// 向き変更 : 移動先 or エイムターゲット(+移動量下げる)
			if(mUnitBattleTarget == null){
				if(moveX != 0 || moveZ != 0) {
					var lookDir = new Vector3(moveVelocity.x, 0f, moveVelocity.z);
					mBody.rotation = Quaternion.LookRotation(lookDir);
				}
			}
			else{
				var look = mUnitBattleTarget.transform.position;
				look.y = mBody.transform.position.y;
				mBody.transform.LookAt(look);
				moveVelocity = moveVelocity * 0.75f;
			}
			
			//-----------
			//	キャラクター移動
			this.transform.position += moveVelocity;

			//-----------
			//	歩きアニメーション
			if(moveX != 0 || moveZ != 0) {
				if(!mAnimation.IsPlaying("Attack")) {
					mBody.gameObject.GetComponent<Animation>().CrossFade("Walk");
				}
			}
			
			//------------
			//	スキル
			if(Input.GetButtonDown("joycon_button_1_" + mPlayerId) && Input.GetButtonDown("joycon_button_3_" + mPlayerId) ){
				OnSkill(2);
			}
			else if(Input.GetButtonDown("joycon_button_1_" + mPlayerId)){
				OnSkill(0);
				//	motion
				if(!mAnimation.IsPlaying("Attack")) {
					mAnimation.CrossFade("Attack");
				}
			}
			else if(Input.GetButtonDown("joycon_button_3_" + mPlayerId)){
				OnSkill(1);
			}

			//------------
			//	通り抜け
			if(Input.GetButton("joycon_sr_" + mPlayerId)) {
				mUnitState |= UNIT_STATE.IS_MOVE_DOWN;
			}
			else {
				mUnitState &= ~UNIT_STATE.IS_MOVE_DOWN;
			}
		}
		
		if(!mAnimation.isPlaying) {
			mBody.GetComponent<Animation>().CrossFade("Wait");
		}

		if(!GetIsUnitState(UNIT_STATE.IS_HIT_LADDER)) {
			this.transform.GetComponent<Rigidbody>().useGravity = true;
			this.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * -20f);
		}
	}

	protected override void Update()
	{
		base.Update();
		mUIStatusLevel.text = "Lv." + mStatus.Level;
		mUIStatusHpGauge.value = mHpGauge.value;

		//------------------------------------
		//	地面についているか
		if(mUnitLeg.IsJump){
			mUnitState |= UNIT_STATE.IS_JUMP;
			mCurrentJumpCount = 0;
		}else{
			mUnitState &= ~UNIT_STATE.IS_JUMP;
		}

		//------------------------------------
		//	所持アイテムチェンジ
		//------------
		if(Input.GetButtonDown("joycon_sl_"+mPlayerId) && Input.GetButtonDown("joycon_sr_"+mPlayerId)){
			if(mUICreateUnitProperty.gameObject.activeSelf){
				if(mUIPlayerUnitProperty.IsInput){
					mUICreateUnitProperty.SetIsInput(true);
					mUIPlayerUnitProperty.SetIsInput(false);
				}
				else{
					mUICreateUnitProperty.SetIsInput(false);
					mUIPlayerUnitProperty.SetIsInput(true);
				}
			}
		}
		else if(Input.GetButtonDown("joycon_sr_" + mPlayerId)){
			if(mUIPlayerUnitProperty.IsInput){
				mUIPlayerUnitProperty.UpdateInput(true);
			}
			else if(mUICreateUnitProperty.IsInput){
				mUICreateUnitProperty.UpdateInput(true);
			}
		}
		else if(Input.GetButtonDown("joycon_sl_" + mPlayerId)){
			if(mUIPlayerUnitProperty.IsInput){
				mUIPlayerUnitProperty.UpdateInput(false);
			}
			else if(mUICreateUnitProperty.IsInput){
				mUICreateUnitProperty.UpdateInput(false);
			}
		}

		//------------------------------------
		//	アクションボタン
		if(Input.GetButton("joycon_button_2_" + mPlayerId)){
			mActionButtonTimer += Time.deltaTime;
			if(mActionButtonTimer > mButtonSwtichGapTime){
				mUIButtonDescription.ButtonPushed(mActionButtonTimer - mButtonSwtichGapTime, mActionButtonSwitchTime - mButtonSwtichGapTime);
			}
			if(mActionButtonSwitchTime < mActionButtonTimer ){
				if(GetIsUnitState(UNIT_STATE.IS_HIT_CREATE_LOCATION)){
					StartCoroutine(OnItemUseToCreateLocation());
				}
				else{
					StartCoroutine(OnItemUse());
				}
				mUnitState |= UNIT_STATE.IS_PUSHED_ACTION_BUTTON;
				mActionButtonTimer = -mButtonSwtichGapTime;
				mUIButtonDescription.ResetButtonPushed();
			}
		}
		if(Input.GetButtonUp("joycon_button_2_" + mPlayerId)){
			if(mActionButtonTimer > 0f 
			&& mActionButtonTimer < mButtonSwtichGapTime
			&& !GetIsUnitState(UNIT_STATE.IS_PUSHED_ACTION_BUTTON)){
				if(GetIsUnitState(UNIT_STATE.IS_HIT_CREATE_LOCATION)){
					if(GetIsUnitState(UNIT_STATE.IS_HIT_DISH_LOCATION)){
						StartCoroutine(OnTeakDishForCreateLocation());
					}
					else{
						StartCoroutine(OnItemDropToCreateLocation());
					}
				}
				else{
					StartCoroutine(OnItemDrop());
				}
			}
			mUnitState &= ~UNIT_STATE.IS_PUSHED_ACTION_BUTTON;
			mUIButtonDescription.ResetButtonPushed();
			mActionButtonTimer = 0;
		}
		
		if(GetIsUnitState(UNIT_STATE.IS_HIT_CREATE_LOCATION)){
			if(mActionButtonTimer > mButtonSwtichGapTime){
				mUIButtonDescription.SetText("料理を開始する");
			}
			else if(GetIsUnitState(UNIT_STATE.IS_HIT_DISH_LOCATION)){
				mUIButtonDescription.SetText("料理をとる");	
			}
			else{
				mUIButtonDescription.SetText("食材をいれる");
			}
		}else{
			if(mActionButtonTimer > mButtonSwtichGapTime){
				mUIButtonDescription.SetText("食べる！");
			}else{
				mUIButtonDescription.SetText("アイテムドロップ");
			}
		}

		//------------------------------------
		//	スティック押し込み
		if(Input.GetButtonDown("joycon_button_stick_" + mPlayerId)){
			if(mUnitBattleTarget == null){
				if(mScoutUnitPlayer.NearUnitEnemy != null){
					mUnitBattleTarget = mScoutUnitPlayer.NearUnitEnemy;
				}
			}
			else{
				mUnitBattleTarget = null;
			}
			mAimBatlleTarget.SetTarget(mUnitBattleTarget == null ? null : mUnitBattleTarget.transform);
		}

		//------------------------------------
		//	ダッシュ
		if(Input.GetButton("joycon_button_0_" + mPlayerId)){
			mInteractiveButtonTimer += Time.deltaTime;
			if(mInteractiveButtonTimer > mInteractiveButtonSwitchTime){
				mUnitState |= UNIT_STATE.IS_DASH;
				mCurrentStamina -= Time.deltaTime;
				if(!mFxTireSmoke.isPlaying && !mFxTireSmoke.main.loop){
					mFxTireSmoke.Play();
					
					var main = mFxTireSmoke.main;
					main.loop = true;
				}
			}
		}
		else{
			var main = mFxTireSmoke.main;
			main.loop = false;
			mUnitState &= ~ UNIT_STATE.IS_DASH;
		}

		//------------
		//	ジャンプ
		if(Input.GetButtonUp("joycon_button_0_" + mPlayerId)) {
			if(mInteractiveButtonTimer < mButtonSwtichGapTime){
				if(mUnitBattleTarget == null){
					StartCoroutine(Jump());	
				}
				else{
					var moveX = Input.GetAxisRaw("joycon_horizontal_" + mPlayerId);
					var moveZ = Input.GetAxisRaw("joycon_vertical_" + mPlayerId);
					StartCoroutine(Doge(new Vector3(moveX, 0f, moveZ)));
				}
			}
			mUnitState &= ~UNIT_STATE.IS_DASH;
			mInteractiveButtonTimer = 0f;
		}

		//------------------------------------
		//	スタミナゲージ更新
		if(mCurrentStamina < 0f){
			mCurrentHp -= Time.deltaTime * 0.5f;
		}else{
			mCurrentStamina -= Time.deltaTime * 0.5f;
		}
		mStaminaGauge.value = mCurrentStamina / mStatus.Stamina;
	}

	void OnTriggerEnter(Collider _collider)
	{
		var layer	= _collider.gameObject.layer;
		var tag		= _collider.gameObject.tag;

		if(layer == (int)Common.GAMEOBJECT_LAYER.GROUND_UNDER) {
			mCollision.isTrigger = true;
		}
		// if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.CREATE_LOCATION)){
		// 	var unitCreate = _collider.gameObject.GetComponent<UnitManagerCreateLocation>();
		// 	if(!mUICreateUnitProperty.gameObject.activeSelf && unitCreate.TeamId == mTeamId){
		// 		// mUICreateUnitProperty.gameObject.SetActive(true);
		// 		// mUICreateUnitProperty.SetPropertyList(unitCreate.ItemDataList, unitCreate.PropertyMax);
		// 		mUICreateUnitProperty.SetIsInput(false);
		// 	}
		// }
	}

	void OnTriggerStay(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;

		if(layer == (int)Common.GAMEOBJECT_LAYER.FIELD && tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.SEMITRANSPARENT)) {
			if(GetIsUnitState(UNIT_STATE.IS_MOVE_DOWN)){
				mCollision.isTrigger = true;
			}
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.ITEM) && GetIsUnitState(UNIT_STATE.IS_GET_ITE)) {
			var itemObj = _collider.gameObject.GetComponent<ItemObject>();
			if(mPropertyList.Count < PropertyMax) {
				StartCoroutine(GetItem(itemObj.Data));
				Destroy(itemObj.gameObject);
			}
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.TOWER)){
			var tower = _collider.gameObject.GetComponent<UnitManagerTower>();
			if(tower.TeamId == mTeamId){
				mUnitState |= UNIT_STATE.IS_HIT_TOWER;
			}
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.CREATE_LOCATION)){
			var unitCreateLocation = _collider.gameObject.GetComponent<UnitManagerCreateLocation>();
			if(unitCreateLocation.PropertyDishData != null){
				
			}
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.LADDER)) {
			mUnitState |= UNIT_STATE.IS_HIT_LADDER;
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.CREATE_LOCATION)){
			var unitCreateLocation = _collider.gameObject.GetComponent<UnitManagerCreateLocation>();
			if(unitCreateLocation.TeamId == mTeamId){
				mUnitState |= UNIT_STATE.IS_HIT_CREATE_LOCATION;
				mCurrentCreateLocation = unitCreateLocation;
				if(mCurrentCreateLocation.PropertyDishData != null){
					mUnitState |= UNIT_STATE.IS_HIT_DISH_LOCATION;
					if(!mUIUnitCreateLocationDish.gameObject.activeSelf){
						mUIUnitCreateLocationDish.gameObject.SetActive(true);
						mUIUnitCreateLocationDish.SetDishData(mCurrentCreateLocation.PropertyDishData);
						mUICreateUnitProperty.gameObject.SetActive(false);
					}					
				}
				else{
					mUnitState &= ~UNIT_STATE.IS_HIT_DISH_LOCATION;
					if(!mUICreateUnitProperty.gameObject.activeSelf){
						mUICreateUnitProperty.gameObject.SetActive(true);
						mUICreateUnitProperty.SetPropertyList(mCurrentCreateLocation.ItemDataList, mCurrentCreateLocation.PropertyMax);
						mUICreateUnitProperty.SetIsInput(false);

						mUIUnitCreateLocationDish.gameObject.SetActive(false);
					}
				}
			}
			else{
				mCurrentCreateLocation = null;
				mUnitState &= ~UNIT_STATE.IS_HIT_CREATE_LOCATION;
				mUnitState &= ~UNIT_STATE.IS_HIT_DISH_LOCATION;
			}
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;

		if(layer == (int)Common.GAMEOBJECT_LAYER.GROUND_UNDER) {
			mCollision.isTrigger = false;
		}

		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.TOWER)){
			var tower = _collider.gameObject.GetComponent<UnitManagerTower>();
			if(tower.TeamId == mTeamId){
				mUnitState &= ~UNIT_STATE.IS_HIT_TOWER;
			}
		}
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.LADDER)) {
			mUnitState &= ~UNIT_STATE.IS_HIT_LADDER;
		}
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.CREATE_LOCATION)){
			var unitCreate = _collider.gameObject.GetComponent<UnitManagerCreateLocation>();
			mUnitState &= ~UNIT_STATE.IS_HIT_CREATE_LOCATION;
			mCurrentCreateLocation = null;

			mUICreateUnitProperty.gameObject.SetActive(false);
			mUIUnitCreateLocationDish.gameObject.SetActive(false);

			mUIPlayerUnitProperty.SetIsInput(true);
		}
	}

	private IEnumerator Jump()
	{
		if(!GetIsUnitState(UNIT_STATE.IS_JUMP)){
			yield break;
		}
		mUnitState &= ~UNIT_STATE.IS_JUMP;
		mCurrentJumpCount++;

		this.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * 800f);
		yield return new WaitForEndOfFrame();

		if(mCurrentJumpCount < mLimitJumpCount) {
			mUnitState |= UNIT_STATE.IS_JUMP;
		}

		yield return null;
	}

	protected override IEnumerator Dead()
	{
		mIsDead = true;
		
		this.transform.position = mRespawnTarget.position;
		mBody.LookAt(mRespawnTarget.TransformDirection(Vector3.forward * 10f));

		var timer = 0f;
		while(mRespawnInterval >= timer){
			timer += Time.deltaTime;
			mHpGauge.value = timer / mRespawnInterval;
			yield return null;
		}
		mCurrentHp = mStatus.Hp;
		mCurrentStamina = mStatus.Stamina;
		mIsDead = false;
	}

	private IEnumerator GetItem(ItemData _data){
		if(!GetIsUnitState(UNIT_STATE.IS_GET_ITE)){
			yield break;
		}
		mUnitState &= ~UNIT_STATE.IS_GET_ITE;
		
		mPropertyList.Add(_data);

		yield return new WaitForEndOfFrame();

		mUnitState |= UNIT_STATE.IS_GET_ITE;
	}

	private bool GetIsUnitState(UNIT_STATE _state)
	{
		if((mUnitState & _state) == _state){
			return true;
		}
		return false;
	}
	private void PropertyListRemoveAt(int _index){
		mPropertyList.RemoveAt(_index);
	}

	private void OnSkill(int _skillListIndex){
		if(mSkillMamagerList.Count <= _skillListIndex){
			return;
		}
		if(mSkillMamagerList[_skillListIndex].IsSkill){
			mCurrentStamina -= mSkillMamagerList[_skillListIndex].SkillData.NeedStamina;
			mSkillMamagerList[_skillListIndex].OnAbility();
		}
	}

	private IEnumerator OnItemDrop()
	{
		if(mPropertyList.Count <= 0){
			yield break;
		}

		var propertyIndex = mUIPlayerUnitProperty.CurrentPropertyIndex;
		if(propertyIndex == -1){
			yield break;
		}

		var dropDir = mBody.TransformDirection(Vector3.forward);
		var itemObj = (GameObject)Instantiate(GameData.Instance.ItemObject);
		itemObj.transform.position = mBody.position + new Vector3(0f,3f,0f);
		
		itemObj.GetComponent<ItemObject>().Init(mPropertyList[propertyIndex], this);
		itemObj.GetComponent<Rigidbody>().AddForce(dropDir * 400f);

		PropertyListRemoveAt(propertyIndex);

		yield return null;
	}

	private IEnumerator OnItemUse()
	{
		if(mPropertyList.Count <= 0){
			yield break;
		}

		var propertyIndex = mUIPlayerUnitProperty.CurrentPropertyIndex;
		if(propertyIndex == -1){
			yield break;
		}

		var useData = mPropertyList[propertyIndex];
		mCurrentExp += useData.Exp;

		mCurrentHp += useData.ItemStatus[Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY];
		if(mCurrentHp > mStatus.Hp){
			mCurrentHp = mStatus.Hp;
		}

		mCurrentStamina += useData.ItemStatus[Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY];
		if(mCurrentStamina > mStatus.Stamina){
			mCurrentStamina = mStatus.Stamina;
		}

		var expTableIndex	= mStatus.ExpTableList.Count(x=> x <= mCurrentExp);
		if(mStatus.ExpTableList.Count > expTableIndex){
			foreach(var status in useData.ItemStatus){
				if(status.Value <= 0f || status.Key == Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY || status.Key == Common.ITEM_UNIT_STATUS.STAMINA_RECOVERY){
					continue;
				}
				Debug.Log("Key = " + status.Key + " / Value = " + status.Value);
				if(mItemStatusDictionary.ContainsKey(status.Key)){
					mItemStatusDictionary[status.Key] += status.Value;
				}
				else{
					mItemStatusDictionary.Add(status.Key, status.Value);
				}
			}
			mUIExp.UIUpdateStatus(mItemStatusDictionary);
			
			var currentLevel	= expTableIndex + 1;
			if(mStatus.Level < currentLevel){
				var levelUp = currentLevel - mStatus.Level;
				mStatus.OnLevelUp(levelUp, mItemStatusDictionary);
				mItemStatusDictionary.Clear();
				mUIExp.UIResetStatus();
			}

			var diffPreLeveExp	= 0f;
			if(expTableIndex > 0){
				diffPreLeveExp = mStatus.ExpTableList[expTableIndex-1];
			}
			mUIExp.UIUpdateExp(mCurrentExp - diffPreLeveExp, mStatus.ExpTableList[expTableIndex] - diffPreLeveExp);
		}
		else if(mItemStatusDictionary != null){
			mUIExp.UILevelMax();
			mUIExp.UIResetStatus();
			mItemStatusDictionary.Clear();
			mItemStatusDictionary = null;
		}

		PropertyListRemoveAt(propertyIndex);
		yield return null;
	}

	private IEnumerator OnItemDropToCreateLocation()
	{
		// var propertyIndex = mUIPlayerUnitProperty.CurrentPropertyIndex;
		// if(propertyIndex == -1){
		// 	yield break;
		// }
		// Debug.Log("OnItemDropToCreateLocation");
		if(!mCurrentCreateLocation.IsNewItemCreate){
			yield break;
		}
		
		if(mUIPlayerUnitProperty.IsInput){
			mUIPlayerUnitProperty.SwitchPropertyData(mUICreateUnitProperty);
		}else if(mUICreateUnitProperty.IsInput){
			mUICreateUnitProperty.SwitchPropertyData(mUIPlayerUnitProperty);
		}
		mCurrentCreateLocation.UpdateUIObjectProperty();

		// if(mCurrentCreateLocation.IsAddProperty){
		// 	mCurrentCreateLocation.OnAddMaterial(mPropertyList[propertyIndex]);
		// 	PropertyListRemoveAt(propertyIndex);
		// }

		yield return null;
	}

	private IEnumerator OnTeakDishForCreateLocation(){
		if(mPropertyList.Count < PropertyMax){
			mPropertyList.Add(mCurrentCreateLocation.PropertyDishData);
			mCurrentCreateLocation.OnResetPropertyDish();
		}
		yield return null;
	}

	private IEnumerator OnItemUseToCreateLocation()
	{
		mCurrentCreateLocation.OnCreate();
		yield return null;
	}

	private IEnumerator Doge(Vector3 _dir){
		if(!GetIsUnitState(UNIT_STATE.CAN_DOGE)){
			yield break;
		}
		mUnitState &= ~UNIT_STATE.CAN_DOGE;
		
		mFxDoge.SetActive(true);
		
		var velocity = _dir * 10f;
		this.gameObject.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);

		mCurrentStamina -= 2f;
		
		mIsDaamge = false;
		yield return new WaitForSeconds(0.3f);
		
		mIsDaamge = true;
		yield return new WaitForSeconds(0.5f);

		mFxDoge.SetActive(false);
		mUnitState |= UNIT_STATE.CAN_DOGE;
		yield return null;
	}
}
