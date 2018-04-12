using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitManagerAnimal : UnitManager
{
	[SerializeField]
	private Slider mLifeGuge;
	[SerializeField]
	private ItemObject mItemObject;
	[SerializeField]
	private List<int> mDropList;
	[SerializeField]
	private List<Vector3> mRandomDir = new List<Vector3>();

	private Vector3 mMoveDir;

	protected override void Init()
	{
		base.Init();
		StartCoroutine(RandomMove());
	}

	void FixedUpdate()
	{	
		this.transform.position += mMoveDir * MoveValue;
	}

	protected override void Update()
	{
		base.Update();
		mLifeGuge.value = mCurrentHp / mStatus.Hp;
	}

	protected override IEnumerator Dead()
	{
		mIsDead = true;

		var itemObj = (GameObject)Instantiate(mItemObject.gameObject);
		itemObj.transform.position = this.transform.position + Vector3.up * 2f;
		itemObj.GetComponent<Rigidbody>().AddForce(Vector3.up * 30f * Time.deltaTime, ForceMode.Impulse);

		var item = itemObj.GetComponent<ItemObject>();
		var index = Random.Range(0, mDropList.Count);
		var itemData = new FoodData(mDropList[index]);
		item.Init(itemData);

		Destroy(this.gameObject);
		yield return null;
	}

	private IEnumerator RandomMove()
	{
		while(true) {
			if(mRandomDir.Count <= 0){
				yield break;
			}
			var random = Random.Range(0, mRandomDir.Count);
			mMoveDir = mRandomDir[random];

			if(mMoveDir.x != 0) {
				var lookDir = new Vector3(mMoveDir.x, 0f, 0f);
				mBody.transform.rotation = Quaternion.LookRotation(lookDir);
			}

			yield return new WaitForSeconds(1f);
		}
	}
}
