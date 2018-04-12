using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData 
{
	private int mId;
	private string mName;
	private string mDescription;
	private float mRestTime;
	private float mNeedStamina;
	private float mExtraAttack;
	private float mRange;
	private Sprite mSprite;
	
	public int Id				{get {return mId;}}
	public string Name			{get{return mName;}}
	public string Description	{get{return mDescription;}}
	public float RestTime		{get{return mRestTime;}}
	public float NeedStamina	{get{return mNeedStamina;}}
	public float ExtraAttack 	{get {return mExtraAttack;}}
	public float Range			{get {return mRange;}}
	public Sprite Sprite 		{get {return mSprite;}}

	public SkillData(int _id, bool _isLoadSprite = true){
		mId = _id;
		if(GameData.Instance.SkillData.ContainsKey(mId.ToString())){
			var datas = GameData.Instance.SkillData[mId.ToString()];
			mName			= datas["Name"];
			mDescription	= datas["Description"];
			mRestTime		= float.Parse(datas["RestTime"]);
			mNeedStamina	= float.Parse(datas["NeedStamina"]);
			mRange			= float.Parse(datas["Range"]);
			mExtraAttack	= float.Parse(datas["ExtraAttack"]);
			if(_isLoadSprite){
				mSprite		= Resources.Load<Sprite>(datas["Asset"]);	
			}
		}
	}
}
