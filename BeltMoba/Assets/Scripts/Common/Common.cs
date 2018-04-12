using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Common{
	private Common(){}

	private static Common mInstance;
	public static Common Instance {
		get{ 
			if(mInstance == null) {
				mInstance = new Common();
			}
			return mInstance;
		}
	}
	private static Dictionary<GAMEOBJECT_TAG, string> mTag = new Dictionary<GAMEOBJECT_TAG, string>()
	{
		{GAMEOBJECT_TAG.PLAYER,				"Player"			},
		{GAMEOBJECT_TAG.MINION,				"Minion"			},
		{GAMEOBJECT_TAG.TOWER,				"Tower"				},
		{GAMEOBJECT_TAG.LADDER, 			"Ladder"			},
		{GAMEOBJECT_TAG.ITEM,				"Item"				},
		{GAMEOBJECT_TAG.THROW_ITEM,			"ThrowItem"			},
		{GAMEOBJECT_TAG.SEMITRANSPARENT,	"Semitransparent"	},
		{GAMEOBJECT_TAG.CREATE_LOCATION,	"CreateLocation"	},
		{GAMEOBJECT_TAG.SCROLL_VIEW_CONTENT,"ScrollViewContent"	},
		{GAMEOBJECT_TAG.WAY_POINT,			"WayPoint"			},
	};

	public static string GetGameObjectTagName(GAMEOBJECT_TAG _tag){
		var retStr = "";
		if(mTag.ContainsKey(_tag)){
			retStr = mTag[_tag];
		}
		return retStr;
	}

	public enum GAMEOBJECT_TAG
	{
		PLAYER,
		TOWER,
		MINION,
		LADDER,
		ITEM,
		THROW_ITEM,
		SEMITRANSPARENT,
		CREATE_LOCATION,
		SCROLL_VIEW_CONTENT,
		WAY_POINT,
	}
	
	
	public enum GAMEOBJECT_LAYER{
		FIELD					= 8,
		GROUND_UNDER			= 10,
		UNIT					= 9,
		ITEM					= 11,
		UNIT_CREATE_LOCATION	= 12,
		BREAKBABLE				= 13,
		NOT_GET_ITEM			= 14,
		WALL_LINE_1				= 16,
		SCOUT_AREA				= 20,
	}

	public enum ITEM_KIND {
		FOOD	= 1,	//	食材
		DISH	= 2	//	料理
	}

	public enum NPC_UI {
		NEUTRAL,		//	中立で、攻撃を受けるまで攻撃を行わない
		AGGRESSIVE,		//	索敵反映に入った別チームのユニットを攻撃する
		COWARD,			//	索敵範囲に入った別チームから逃げる、反撃もしない
	}

	public enum NPC_ACTION_STATUS {
		RANDOM_MOVE = 1 << 1,
		SCOUT_ENEMY_CHASE = 1 << 2,
		SCOUT_ENEMY_ESCAPE = 1 << 3,
		ATTACK = 1 << 4,
	}

	public enum COOKING_OPERATION_TYPE{
		NONE = 0,
		BUTTON_UP = 1,
		BUTTON_LEFT,
		BUTTON_RIGHT,
		BUTTON_DOWN,
	}

	public enum ITEM_UNIT_STATUS {
		HEALTH_RECOVERY		= 0,
		STAMINA_RECOVERY	= 1,
		HEALTH				= 2,
		STAMINA				= 3,
		ATTACK				= 4,
		DEFENSE				= 5,
		SPEED				= 6,
	}

	public static bool IsDamageUnitObjectTagLayer(string _tag, int _layer){
		if(_tag == mTag[GAMEOBJECT_TAG.PLAYER] 
		|| _tag == mTag[GAMEOBJECT_TAG.MINION] 
		|| _tag == mTag[GAMEOBJECT_TAG.TOWER] 
		|| _tag == mTag[GAMEOBJECT_TAG.CREATE_LOCATION] 
		|| _layer == (int)GAMEOBJECT_LAYER.BREAKBABLE){
			return true;
		}
		return false;
	}
}
