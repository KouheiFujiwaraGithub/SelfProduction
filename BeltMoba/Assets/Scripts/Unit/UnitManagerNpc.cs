using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerNpc : UnitManager {
	[SerializeField]
	List<Vector3> mMoveRandomDirectionList = new List<Vector3>();

	[SerializeField]
	private float mMoveTime;

	protected bool mIsMoveJust = true;
	
	protected Vector3 mBackToHomePos = Vector3.zero;

	private Dictionary<UnitManager, float> mUnitTaimeData = new Dictionary<UnitManager, float>();

	protected UnitManagerPlayer mUnitPlayerMaster;

	protected override void Init()
	{
		base.Init();
		StartCoroutine(UpdateMove());
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override IEnumerator Dead()
	{
		mIsDead = true;

		var itemData   = mStatus.GetDropItemData();
		if(itemData != null){
			var itemObject = (GameObject)Instantiate(GameData.Instance.ItemObject);
			itemObject.transform.position = this.transform.position + Vector3.up;
			itemObject.GetComponent<ItemObject>().Init(itemData);
		}

		Destroy(this.gameObject);
		yield return null;
	}

	private IEnumerator UpdateMove()
	{
		if(mMoveRandomDirectionList.Count == 0){
			yield break;
		}
		while(true){
			if(mUnitPlayerMaster == null && mIsMoveJust){
				var timer = 0f;
				var randomIndex = Random.Range(0,mMoveRandomDirectionList.Count);
				var randomDir   = new Vector3(mMoveRandomDirectionList[randomIndex].x, 0f, mMoveRandomDirectionList[randomIndex].z);
				var dir         = mBackToHomePos == Vector3.zero ? randomDir : (mBackToHomePos - this.transform.position).normalized;
				while(timer < mMoveTime){
					if(!mIsMoveJust && dir == Vector3.zero && mBackToHomePos != Vector3.zero && mUnitPlayerMaster != null){
						break;
					}

					timer += Time.deltaTime;
					if(dir != Vector3.zero){
						mBody.rotation = Quaternion.LookRotation(dir);
						this.transform.position += mBody.transform.TransformDirection(Vector3.forward * MoveValue);
					}
					yield return null;
				}
			}
			else if(mUnitPlayerMaster != null && mIsMoveJust){
				var moveDir = (mUnitPlayerMaster.transform.position - this.transform.position).normalized;
				moveDir.y	= 0f;
				mBody.rotation = Quaternion.LookRotation(moveDir);

				var distance = Vector3.Distance(this.transform.position, mUnitPlayerMaster.transform.position);
				if(distance > 5f){
					this.transform.position += mBody.transform.TransformDirection(Vector3.forward * MoveValue);
				}
			}
			yield return null;
		}
	}

	public void UpdateOutsideFiledToBackDir( Vector3 _backToHome)
	{
		mBackToHomePos		= _backToHome;
		mBackToHomePos.y	= 0f;
	}

	void OnTriggerEnter(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;
		
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.ITEM) || tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.THROW_ITEM)){
			var itemObj = _collider.gameObject.GetComponent<ItemObject>();
			if(itemObj.UnitManager is UnitManagerPlayer){
				var unitPlayerManager = itemObj.UnitManager;
				if(unitPlayerManager != null){
					if(mUnitTaimeData.ContainsKey(unitPlayerManager)){
						mUnitTaimeData[unitPlayerManager] += itemObj.Data.Exp;
					}
					else{
						mUnitTaimeData.Add(unitPlayerManager, itemObj.Data.Exp);
					}
					Destroy(_collider.gameObject);

					if(mUnitTaimeData[unitPlayerManager] > 10f){
						var color = Color.white;
						if(unitPlayerManager.TeamId == 1){
							color = Color.red;
						}else{
							color = Color.blue;
						}
						mHpGauge.fillRect.gameObject.GetComponent<Image>().color = color;
						
						mUnitPlayerMaster = (UnitManagerPlayer)unitPlayerManager;
						mTeamId			  = mUnitPlayerMaster.TeamId;
						mUnitTaimeData.Clear();
					}

					foreach(var addStatus in itemObj.Data.ItemStatus){
						if(addStatus.Key == Common.ITEM_UNIT_STATUS.HEALTH_RECOVERY){
							Debug.Log(mCurrentHp);
							mCurrentHp += addStatus.Value;
							if(mCurrentHp > mStatus.Hp){
								mCurrentHp = mStatus.Hp;
							}
							Debug.Log(mCurrentHp);
						}
						if(mStatus.ItemStatus.ContainsKey(addStatus.Key)){
							mStatus.ItemStatus[addStatus.Key] += addStatus.Value;
						}
						else{
							mStatus.ItemStatus.Add(addStatus.Key, addStatus.Value);
						}
					}
				}
			}
		}
	}

}