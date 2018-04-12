using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData  {
	protected int mId;
	protected string mName;
	protected string mDescription;
	protected float mExp;
	protected Common.ITEM_KIND mKind;
	protected Sprite mSprite;

	protected Dictionary<Common.ITEM_UNIT_STATUS, float> mItemStatus = new Dictionary<Common.ITEM_UNIT_STATUS, float>();

	public int Id { get { return mId; } }
	public string Name { get { return mName; } }
	public string Description { get { return mDescription; } }
	public float Exp{ get { return mExp; } }
	public Common.ITEM_KIND Kind { get { return mKind; } }
	public Sprite Sprite { get { return mSprite; } }

	public Dictionary<Common.ITEM_UNIT_STATUS, float> ItemStatus { get { return mItemStatus;} }

}
