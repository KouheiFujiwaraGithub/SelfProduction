using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData {
	//--------------------------------
	private GameData(){}

	private Dictionary<int, float> mTeamPoint = new Dictionary<int, float>();

	private GameObject mItemObjcect;
	public GameObject ItemObject { get { return mItemObjcect; } }

	private GameObject mUIDamageDraw;
	public GameObject UIDamageDraw{ get {return mUIDamageDraw; } }

	private Material mMatShieldTeam_1;
	public Material MatShieldTeam_1 { get { return mMatShieldTeam_1; } }
	private Material mMatShieldTeam_2;
	public Material MatShieldTeam_2 { get { return mMatShieldTeam_2; } }

	private UnitManager mUnitTowerTeam_1;
	public UnitManager UnitTowerTeam_1 { get { return mUnitTowerTeam_1; } }
	private UnitManager mUnitTowerTeam_2;
	public UnitManager UnitTowerTeam_2 { get { return mUnitTowerTeam_2; } }
	private float mBattleRemainTimer;
	public float BattleRemaminTimer { get { return mBattleRemainTimer; } }

	private int mLocalPlayerCount = 1;
	public int LocalPlayerCount {get { return mLocalPlayerCount; } }

	public enum EFFECT_TYPE{
		EFFECT_UNIT_DAMAGE_NORMAL = 1,
	}

	private Dictionary<EFFECT_TYPE,GameObject> mEffectData = new Dictionary<EFFECT_TYPE, GameObject>();
	public Dictionary<EFFECT_TYPE,GameObject> EffectData {get {return mEffectData;} }

	//--------------------------------
	//	Data
	private Dictionary<string,Dictionary<string,string>> mFoodData = new Dictionary<string,Dictionary<string,string>>();
	public	Dictionary<string,Dictionary<string,string>> FoodData { get { return mFoodData; } }
	private Dictionary<string,Dictionary<string,string>> mDishData = new Dictionary<string,Dictionary<string,string>>();
	public	Dictionary<string,Dictionary<string,string>> DishData { get { return mDishData; } }
	private Dictionary<string,Dictionary<string,string>> mUnitStatusData = new Dictionary<string,Dictionary<string,string>>();
	public  Dictionary<string,Dictionary<string,string>> UnitStatusData { get { return mUnitStatusData; } }
	private Dictionary<string,Dictionary<string,string>> mSkillData = new Dictionary<string,Dictionary<string,string>>();
	public  Dictionary<string,Dictionary<string,string>> SkillData { get { return mSkillData; } }
	private Dictionary<string,Dictionary<string,string>> mDropData = new Dictionary<string,Dictionary<string,string>>();
	public  Dictionary<string,Dictionary<string,string>> DropData { get { return mDropData; } }

	//--------------------------------
	private static GameData mInstance;
	public static GameData Instance {
		get{ 
			if(mInstance == null) {
				mInstance = new GameData();
			}
			return mInstance;
		}
	}

	//--------------------------------
	public void Init()
	{
		DataInit("CSV/data_food", ref mFoodData);
		DataInit("CSV/data_dish", ref mDishData);
		DataInit("CSV/data_unit_status", ref mUnitStatusData);
		DataInit("CSV/data_skill", ref mSkillData);
		DataInit("CSV/data_drop", ref mDropData);

		InitTeamPoint();

		mItemObjcect	= (GameObject)Resources.Load("Prefabs/ItemObject");
		mUIDamageDraw	= (GameObject)Resources.Load("Prefabs/UIDamageDraw");
		mMatShieldTeam_1 = (Material)Resources.Load("Materials/team_1");
		mMatShieldTeam_2 = (Material)Resources.Load("Materials/team_2");
		
		mEffectData.Add(EFFECT_TYPE.EFFECT_UNIT_DAMAGE_NORMAL, (GameObject)Resources.Load("Effects/UnitDamageNormal"));
	}

	//--------------------------------
	private void InitTeamPoint()
	{
		if(mTeamPoint.ContainsKey(1)) {
			mTeamPoint[1] = 0f;
		}
		else {
			mTeamPoint.Add(1, 0f);
		}

		if(mTeamPoint.ContainsKey(2)) {
			mTeamPoint[2] = 0f;
		}
		else {
			mTeamPoint.Add(2, 0f);
		}
	}

	private class CanDisData
	{
		private string dishKey;
		public string DishKey{ get{ return dishKey; } }
		public float expValue;
		public float ExpValue{ get{ return expValue; } }
		private int duplicateCount;
		public int DuplicateCount{ get{ return duplicateCount; } }
		private int duplicateFirstIndex;
		public int DuplicateFirstIndex{ get{ return duplicateFirstIndex; } }
		public CanDisData(string _key, float _expValue, int _dupCnt, int _dupIdx){
			dishKey = _key;
			expValue = _expValue;
			duplicateCount = _dupCnt;
			duplicateFirstIndex = _dupIdx;
		}
	}

	public int GetRecipeToDish(string[] _foodstuffs)
	{
		var canDishList = new List<CanDisData>();
		var disKey = "0";
		foreach(var data in mDishData){
			var recipe	 = data.Value["Recipe"].Split(' ');
			var dupList = _foodstuffs.Intersect(recipe).ToArray();
			if(dupList.Length >= recipe.Length){
				var dupIndex = Array.IndexOf(_foodstuffs, dupList[0]);
				canDishList.Add(new CanDisData(data.Key, float.Parse(data.Value["Exp"]), dupList.Length, dupIndex));
			}
		}

		if(canDishList.Count > 0){
			var disData = canDishList
						.OrderByDescending(x=>x.DuplicateCount)
						.OrderByDescending(x=>x.ExpValue)
						.OrderBy(x=>x.DuplicateFirstIndex)
						.FirstOrDefault();
			disKey = disData.DishKey;
		}
		return int.Parse(disKey);
	}

	public void OnAddTeamPoint(int _teamId, float _point)
	{
		if(mTeamPoint.ContainsKey(_teamId)) {
			mTeamPoint[_teamId] += _point;
		}
		else {
			mTeamPoint.Add(_teamId, _point);
		}
	}

	public void DataInit( string _readCsvPath, ref Dictionary<string,Dictionary<string,string>> _data )
	{
		var csvFile = Resources.Load<TextAsset>(_readCsvPath);
		
		var reader = new StringReader(csvFile.text);
		var keys = reader.ReadLine().Split(',');
		
		while(reader.Peek() > -1){
			var values 	= reader.ReadLine().Split(',');
			var id 		= values[0];
			var addData = new Dictionary<string,string>();
			for(var i = 0; i < keys.Length; i++){
				if(keys[i].IndexOf("ref_") >= 0){ 
					continue;
				}
				if(addData.ContainsKey(keys[i])){
					continue;
				}
				addData.Add(keys[i],values[i]);
			}
			if(_data.ContainsKey(id)){
				continue;
			}
			_data.Add(id, addData);
		}
	}

	private string mWinText = "";
	public string WinText { get { return mWinText; } }
	public void SetUnitTower(UnitManager _team1, UnitManager _team2)
	{
		mUnitTowerTeam_1 = _team1;
		mUnitTowerTeam_2 = _team2;
	}

	public void SetWinText(int _teamId = 0)
	{
		mWinText = "チーム " + _teamId + "の勝利";
	}
	public void SetLocalPlayerCount(int _count)
	{
		mLocalPlayerCount = _count;
	}

	public void GetLocalPlayerCameraSetting(int _localPlayerId, ref Vector2 _viewPos, ref Vector2 _viewSiz)
	{
		switch(mLocalPlayerCount){
			case 1:
				_viewPos = Vector2.zero;
				_viewSiz = Vector2.one;
				break;
			case 2:
				_viewSiz = new Vector2(1,0.5f);
				switch(_localPlayerId){
					case 1:	_viewPos = new Vector2(0f,0.5f);	break;
					case 2: _viewPos = new Vector2(0f,0f);		break;
				}
				break;
			case 3:
				_viewSiz = new Vector2(0.5f,0.5f);
				switch(_localPlayerId){
					case 1:	_viewPos = new Vector2(0f,0.5f);	break;
					case 2: _viewPos = new Vector2(0.5f,0.5f);	break;
					case 3:
						_viewPos = new Vector2(0f,0f);
						_viewSiz = new Vector2(1,0.5f);
						break;
				}
				break;
			case 4:
				_viewSiz = new Vector2(0.5f,0.5f);
				switch(_localPlayerId){
					case 1:	_viewPos = new Vector2(0f,0.5f);	break;
					case 2: _viewPos = new Vector2(0.5f,0.5f);	break;
					case 3:	_viewPos = new Vector2(0f,0f);		break;
					case 4:	_viewPos = new Vector2(0.5f,0f);	break;
				}
				break; 
		}
	}

	public void SetBattleRemainTime(float _remainTime)
	{
		mBattleRemainTimer = _remainTime;
	}
}
