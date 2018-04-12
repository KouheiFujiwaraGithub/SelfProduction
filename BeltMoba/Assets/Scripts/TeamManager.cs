using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour {
	[SerializeField]
	private int mTeamId;
	[SerializeField]
	private Transform mPlayerParent;
	[SerializeField]
	private GameObject[] mTeamSymbolList;
	private List<UnitManagerPlayer> mPlayerList = new List<UnitManagerPlayer>();
	[SerializeField]
	private Transform mTowerParent;
	private List<UnitManagerTeam> mTowerList = new List<UnitManagerTeam>();
	private List<ItemData> mTeamProperty = new List<ItemData>();
	public List<ItemData> TeamProperty { get { return mTeamProperty; } }
	public int TeamPropertyMax { get { return 20; } }
	private List<ItemData> mTeamDishProperty = new List<ItemData>();
	public List<ItemData> TeamDishProperty { get { return mTeamDishProperty; } }
	public int TeamDishPropertyMax { get { return 5; } }
	private float mTeamPoint = 0f;

	public int TeamId { get { return mTeamId; } }
	public GameObject TeamSymbol { get { return mTeamSymbolList[mTeamId-1]; } }

	void Awake()
	{
		for(var i = 0; i < mPlayerParent.childCount; i++){
			var unit = mPlayerParent.GetChild(i).Find("UnitManagerPlayer").GetComponent<UnitManagerPlayer>();
			unit.SetTeamManager(this);
			mPlayerList.Add(unit);
		}

		for(var i = 0; i < mTowerParent.childCount; i++){
			var unit = mTowerParent.GetChild(i).gameObject.GetComponent<UnitManagerTeam>();
			unit.SetTeamManager(this);
			mTowerList.Add(unit);
		}
	}
}
