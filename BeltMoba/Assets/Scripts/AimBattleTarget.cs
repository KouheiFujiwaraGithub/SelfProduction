using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimBattleTarget : MonoBehaviour {
	RectTransform mRectTransform = null;
	Transform mTarget = null;

	[SerializeField]
	private RectTransform mCanvasRect;
	[SerializeField]
	private Camera mCmaera;

	void Awake()
	{
		mRectTransform = GetComponent<RectTransform>();
	}
	
	void Start()
	{
	
	}

	void Update ()
	{
		if(mTarget == null){
			this.GetComponent<Image>().color = Color.clear;
			return;
		}
		this.GetComponent<Image>().color = Color.white;
		
		// Vector2 pos;
 
        // Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, mTarget.position);
 
        // RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvasRect, screenPos, Camera.main, out pos);
 
        // mRectTransform.position = pos;

		var pos = Vector2.zero;
		var screenPos = RectTransformUtility.WorldToScreenPoint(mCmaera, mTarget.position);

		RectTransformUtility.ScreenPointToLocalPointInRectangle(mCanvasRect, screenPos, mCmaera, out pos);
		mRectTransform.localPosition = pos;
	}

	public void SetTarget(Transform _target){
		mTarget = _target;
	}
}
