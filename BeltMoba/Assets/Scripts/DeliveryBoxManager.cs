using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryBoxManager : MonoBehaviour {
	[SerializeField]
	private Transform mWayPointParent;

	private List<Transform> mWayPoints = new List<Transform>();

	private float mMoveValueTeam_1 = 0f;
	private float mMoveValueTeam_2 = 0f;

	private int mNextWayPointTeam_1 = 0;
	private int mNextWayPointTeam_2 = 0;

	void Start()
	{
		for(var i = 0; i < mWayPointParent.childCount; i++){
			mWayPoints.Add(mWayPointParent.GetChild(i));
		}
		this.transform.position = mWayPoints[Mathf.FloorToInt(mWayPoints.Count/2)].position;
	}

	void FixedUpdate()
	{	
		var velocityTeam_1	= (mWayPoints[mNextWayPointTeam_1].position - this.transform.position).normalized * mMoveValueTeam_1 * Time.deltaTime;
		var velocityTeam_2	= (mWayPoints[mNextWayPointTeam_2].position - this.transform.position).normalized * mMoveValueTeam_2 * Time.deltaTime;

		this.transform.position += velocityTeam_1 + velocityTeam_2;
	}

	void OnTriggerEnter(Collider _collider){
		var tag = _collider.gameObject.tag;
		var layer = _collider.gameObject.layer;
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.THROW_ITEM) || tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.ITEM)){
			var itemObj = _collider.gameObject.GetComponent<ItemObject>();
			if(itemObj.TeamId == 1 || itemObj.TeamId == 2 ){
				StartCoroutine(OnGetItem(itemObj.Data.Exp, itemObj.TeamId));
			}
			Destroy(itemObj.gameObject);
		}

		// if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.PLAYER)){
		// 	var unitPlayer = _collider.gameObject.GetComponent<UnitManagerPlayer>();
		// 	if(unitPlayer.PropertyDishData != null){
		// 		StartCoroutine(OnGetItem(unitPlayer.PropertyDishData.Point, unitPlayer.TeamId));
		// 		unitPlayer.RemovePropertyDishData();
		// 	}
		// }
	}

	void OnTriggerStay(Collider _collider){
		var tag = _collider.gameObject.tag;
		if( tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.WAY_POINT) ){
			var currentWayPoint = 0;
			for(var i = 0; i < mWayPoints.Count; i++){
				if(_collider.transform == mWayPoints[i]){
					currentWayPoint = i;
					break;
				}
			}
			//	配列オーバーしないように制限
			if(currentWayPoint + 1 == mWayPoints.Count || currentWayPoint -1 == -1){
				if(currentWayPoint + 1 == mWayPoints.Count){
					GameData.Instance.SetWinText(2);
				}else{
					GameData.Instance.SetWinText(1);
				}
				return;
			}
			mNextWayPointTeam_1 = currentWayPoint -1;
			mNextWayPointTeam_2 = currentWayPoint +1;
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var tag = _collider.gameObject.tag;
		if(tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.WAY_POINT)){
			var preWayPoint = 0;
			for(var i = 0; i < mWayPoints.Count; i++){
				if(_collider.transform == mWayPoints[i]){
					preWayPoint = i;
					break;	
				}
			}
			var disntance1 = Vector3.Distance(this.transform.position, mWayPoints[mNextWayPointTeam_1].position) - Vector3.Distance(mWayPoints[mNextWayPointTeam_1].position, mWayPoints[preWayPoint].position) ;
			var disntance2 = Vector3.Distance(this.transform.position, mWayPoints[mNextWayPointTeam_2].position) - Vector3.Distance(mWayPoints[mNextWayPointTeam_2].position, mWayPoints[preWayPoint].position) ;;

			if(disntance1 > disntance2){
				mNextWayPointTeam_1 = preWayPoint;
			}else{
				mNextWayPointTeam_2 = preWayPoint;
			}
			Debug.Log("distance1 = " + disntance1);
			Debug.Log("mNextWayPointTeam_1 = " + mNextWayPointTeam_1);
			Debug.Log("distance2 = " + disntance2);
			Debug.Log("mNextWayPointTeam_2 = " + mNextWayPointTeam_2);
		}
	}

	private IEnumerator OnGetItem( float _point, int _teamId)
	{
		var addValue 	= _point * 0.1f;
		var timer		= _point * 0.1f;
		
		if(_teamId == 1){
			mMoveValueTeam_1 += addValue;
		}
		else if(_teamId == 2){
			mMoveValueTeam_2 += addValue;
		}

		yield return new WaitForSeconds(timer);
		
		if(_teamId == 1){
			mMoveValueTeam_1 -= addValue;
		}
		else if(_teamId == 2){
			mMoveValueTeam_2 -= addValue;
		}
	}

	
}
