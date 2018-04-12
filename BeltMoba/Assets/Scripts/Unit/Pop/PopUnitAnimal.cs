using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUnitAnimal : MonoBehaviour {
	[SerializeField]
	private float mPopInterval;
	[SerializeField]
	private int mPopMax;

	[SerializeField]
	private List<UnitManager> mUnitAnimals;

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
			var randomIndex	= Random.Range(0,mUnitAnimals.Count);
			var animalObj	= (GameObject)Instantiate(mUnitAnimals[randomIndex].gameObject); 
			animalObj.transform.position = this.transform.position;
			animalObj.transform.parent = this.transform;

			yield return new WaitForSeconds(mPopInterval);
		}
	}
}
