using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIUnitCreateLocationDish : MonoBehaviour 
{
	[SerializeField]
	private UIItemObject mUIItemObject;
	
	void Awake()
	{
		mUIItemObject.Init(null);
	}

	public void SetDishData(ItemData _dishData)
	{
		mUIItemObject.Init(_dishData);
	}
}
