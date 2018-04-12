using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer mIcon;

	private ItemData mData;
	public ItemData Data { get { return mData; } }
	public int TeamId { get { return mUnitManager != null ? mUnitManager.TeamId : 0 ; } }

	private UnitManager mUnitManager; 
	public UnitManager UnitManager { get { return mUnitManager; } }

	public void Init(ItemData _data, UnitManager _unitManager = null)
	{
		mData = _data;
		mIcon.sprite = _data.Sprite;
		mUnitManager = _unitManager;
		
		StartCoroutine(Drop());
		StartCoroutine(DestroyTimer());
	}

	private IEnumerator Drop()
	{
		mIcon.color = new Color(1f, 1f, 1f, 0.8f);
		yield return new WaitForSeconds(0.5f);
		this.gameObject.tag = "Item";
		mIcon.color = Color.white;
	}

	private IEnumerator DestroyTimer()
	{
		yield return new WaitForSeconds(5f);
		Destroy(this.gameObject);
	}
}
