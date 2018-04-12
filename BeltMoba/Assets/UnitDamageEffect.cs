using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class UnitDamageEffect : MonoBehaviour {
	void Start()
	{
		StartCoroutine(EffectStart());
	}

	private IEnumerator EffectStart()
	{
		var particle = this.gameObject.GetComponent<ParticleSystem>();
		yield return new WaitWhile(()=> particle.IsAlive(true));
		Destroy(this.gameObject);
	}
}
