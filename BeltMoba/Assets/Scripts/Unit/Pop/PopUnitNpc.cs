using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUnitNpc : MonoBehaviour {
	[SerializeField]
	private float mPopInterval;
	[SerializeField]
	private int mPopMax;

	[SerializeField]
	private List<UnitManager> mUnitNpc;

	void Start()
	{
		StartCoroutine(Pop());
	}

	private IEnumerator Pop()
	{
		while(true) {
			if(mPopMax <= this.transform.childCount) {
				yield return new WaitForSeconds(mPopInterval);
				continue;
			}
			var randomIndex	= Random.Range(0,mUnitNpc.Count);
			var animalObj	= (GameObject)Instantiate(mUnitNpc[randomIndex].gameObject); 
			animalObj.transform.position = this.transform.position;
			animalObj.transform.parent = this.transform;

			yield return new WaitForSeconds(mPopInterval);
		}
	}

	void OnTriggerExit(Collider _collider)
	{
		var tag		= _collider.gameObject.tag;
		var layer	= _collider.gameObject.layer;
		var parent	= _collider.transform.parent;
		if(parent == this.transform && tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.MINION)){
			var unitNpc = _collider.gameObject.GetComponent<UnitManagerNpc>();
			var exitPos = _collider.ClosestPointOnBounds(this.transform.position);
			exitPos.y = 2f;
			
			unitNpc.UpdateOutsideFiledToBackDir(this.transform.position);
		}
	}

	void OnTriggerStay(Collider _collider)
	{	
		var tag		= _collider.gameObject.tag;
		var layer	= _collider.gameObject.layer;
		var parent	= _collider.transform.parent;
		if(parent == this.transform && tag == Common.GetGameObjectTagName(Common.GAMEOBJECT_TAG.MINION)){
			var unitNpc = _collider.gameObject.GetComponent<UnitManagerNpc>();
			unitNpc.UpdateOutsideFiledToBackDir(Vector3.zero);
		}
	}
}
