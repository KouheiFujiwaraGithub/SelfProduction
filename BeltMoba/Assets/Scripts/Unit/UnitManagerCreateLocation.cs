using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerCreateLocation : UnitManagerTeam {
	[SerializeField]
	private Slider mReceptionTimeGuge;
	[SerializeField]
	private Transform mPropertysParent;
	[SerializeField]
	private UIItemObject mPropertyDish;
	[SerializeField]
	private SpriteRenderer mMinimapIcon;
	private List<UIItemObject> mUIItemObjectList = new List<UIItemObject>();
	private List<ItemData> mItemDataList = new List<ItemData>();
	private float mReceptionTime = 0f;
	private float mReceptionTimeMax = 0f;
	private bool mIsNewItemCreate 	= true;
	private bool mIsAddProperty		= true;
	
	private float mTeamPoint = 0f;
	private const float mTeamSwitchTime = 40f;

	public ItemData PropertyDishData { get { return mPropertyDish.ItemData; } }
	public bool IsAddProperty { get { return (mIsAddProperty && mUIItemObjectList.Count(x=>!x.IsData) > 0) ? true : false; } }
	public int PropertyMax {get { return mUIItemObjectList.Count; } }
	public List<ItemData> ItemDataList {get { return mItemDataList; } }
	public bool IsNewItemCreate { get { return mIsNewItemCreate; } }

	protected override void Init()
	{
		base.Init();
		for(var i = 0; i < mPropertysParent.childCount; i++){
			var uiItemObj = mPropertysParent.GetChild(i).GetComponent<UIItemObject>();
			uiItemObj.gameObject.SetActive(false);
			uiItemObj.Init(null);
			mUIItemObjectList.Add(uiItemObj);
		}
		mReceptionTimeGuge.gameObject.SetActive(false);
		if(mTeamId == 1){
			mMinimapIcon.color = Color.red;
		}
		else if(mTeamId == 2){
			mMinimapIcon.color = Color.blue;
		}
	}

	protected override void Update()
	{
		base.Update();
		if(mTeamId == 0){
			mIsDead = true;
			if(mTeamPoint > 0){
				mHpGauge.fillRect.gameObject.GetComponent<Image>().color = Color.red;
			}else{
				mHpGauge.fillRect.gameObject.GetComponent<Image>().color = Color.blue;
			}
			var absPoint = Mathf.Abs(mTeamPoint);
			mHpGauge.value = absPoint / mTeamSwitchTime;

			if(absPoint > mTeamSwitchTime){
				if(mTeamPoint > 0){
					mTeamId = 1;
				}else{
					mTeamId = 2;
				}
				mMinimapIcon.color = mHpGauge.fillRect.gameObject.GetComponent<Image>().color;
				mIsDead = false;
			}
		}
	}

	void OnTriggerStay(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.PLAYER)){
			var unitPlayer = _collider.gameObject.GetComponent<UnitManagerPlayer>();
			if(mTeamId == 0){
				if(unitPlayer.TeamId == 1){
					mTeamPoint += 20f * Time.deltaTime;
				}
				else if(unitPlayer.TeamId == 2){
					mTeamPoint -= 20f * Time.deltaTime;
				}
			}
			else if(mCurrentHp > 0 && mTeamId != unitPlayer.TeamId){
				mCurrentHp -= mTeamSwitchTime * Time.deltaTime;
			}
			else if(mCurrentHp < mStatus.Hp){
				mCurrentHp += mTeamSwitchTime * Time.deltaTime;
			}
		}
	}	

	private IEnumerator OnNewItemCreating()
	{
		if(!mIsNewItemCreate){
			yield break;
		}
		mIsNewItemCreate = false;
		mReceptionTimeGuge.gameObject.SetActive(true);

		while(mReceptionTime > 0 && !mIsDead){
			mReceptionTime -= Time.deltaTime;
			mReceptionTimeGuge.value = mReceptionTime / mReceptionTimeMax;
			yield return null;
		}
		mIsAddProperty = false;

		if(!mIsDead){
			var recipe = "";
			var foodPoint = 0f;
			Dictionary<Common.ITEM_UNIT_STATUS,float> statusDir = new Dictionary<Common.ITEM_UNIT_STATUS, float>();
			for(var i = 0; i < mUIItemObjectList.Count; i++){
				if(!mUIItemObjectList[i].IsData){
					break;
				}
				var itemData = mUIItemObjectList[i].ItemData;
				recipe += itemData.Id + ",";
				foodPoint += itemData.Exp;

				foreach(var status in itemData.ItemStatus){
					if(statusDir.ContainsKey(status.Key)){
						statusDir[status.Key] += status.Value;
					}
					else{
						statusDir.Add(status.Key, status.Value);
					}
				}

				mUIItemObjectList[i].gameObject.SetActive(false);
				mUIItemObjectList[i].Init(null);
			}
			mItemDataList.Clear();
			recipe = recipe.Remove(recipe.Length-1);

			var dishData = new DishData(GameData.Instance.GetRecipeToDish(recipe.Split(',')), foodPoint, statusDir);
			mPropertyDish.Init(dishData);
			mPropertyDish.gameObject.SetActive(true);
		}
		
		mReceptionTimeGuge.gameObject.SetActive(false);
	}

	protected override IEnumerator Dead()
	{
		mIsDead = true;

		var collider = this.gameObject.GetComponent<Collider>();
		collider.enabled = false;

		if(mTeamId == 1){
			mTeamId = 2;
			mTeamColor = Color.blue;
		}
		else if(mTeamId == 2){
			mTeamId = 1;
			mTeamColor = Color.red;
		}
		mHpGauge.fillRect.gameObject.GetComponent<Image>().color = mTeamColor;
		mMinimapIcon.color = mTeamColor;

		mCurrentHp = 10f;

		mIsDead				= false;
		collider.enabled	= true;
		mIsAddProperty		= true;
		mIsNewItemCreate	= true;
		yield return null;
	}

	// public void OnAddMaterial(ItemData _itemData)
	// {
	// 	//	登録されているUIItemObject(UI = Image)をチェックして、追加可能ならオブジェクト有効へ
	// 	for(var i = 0; i < mUIItemObjectList.Count; i++){
	// 		if(mUIItemObjectList[i].IsData){
	// 			mIsAddProperty = false;
	// 			continue;
	// 		}
	// 		mUIItemObjectList[i].gameObject.SetActive(true);
	// 		mUIItemObjectList[i].Init(_itemData);
	// 		mIsAddProperty = true;
	// 		mItemDataList.Add(_itemData);
	// 		break;
	// 	}

	// 	//	チェック後追加可能ならヒットしたItemObjectを削除して、料理時間を延長
	// 	if(mIsAddProperty){
	// 		mReceptionTime += _itemData.Exp;
	// 		mReceptionTimeMax = mReceptionTime;
	// 	}
	// }

	public void UpdateUIObjectProperty()
	{
		var timer = 0f;
		for(var i = 0; i < mItemDataList.Count && i < mUIItemObjectList.Count; i++){
			mUIItemObjectList[i].gameObject.SetActive(true);
			mUIItemObjectList[i].Init(mItemDataList[i]);
			timer += mItemDataList[i].Exp;
		}
		for(var i = mItemDataList.Count; i < mUIItemObjectList.Count; i++){
			mUIItemObjectList[i].gameObject.SetActive(false);
			mUIItemObjectList[i].Init(null);
		}
		//	チェック後追加可能ならヒットしたItemObjectを削除して、料理時間を延長
		if(mIsAddProperty){
			mReceptionTime = timer;
			mReceptionTimeMax = mReceptionTime;
		}
	}

	public void OnCreate()
	{
		var isCreate = false;
		for(var i = 0; i < mUIItemObjectList.Count; i++){
			if(mUIItemObjectList[i].IsData){
				isCreate = true;
				break;
			}
		}
		
		if(isCreate){
			StartCoroutine(OnNewItemCreating());
		}
	}

	public void OnResetPropertyDish()
	{
		mPropertyDish.Init(null);
		mIsAddProperty = true;
		mIsNewItemCreate = true;
	}
}
