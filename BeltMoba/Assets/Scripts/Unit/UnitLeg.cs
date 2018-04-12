using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLeg : MonoBehaviour {
	private bool mIsJump = false;
	public bool IsJump { get {return mIsJump; } }
	void OnTriggerStay(Collider _collider)
	{
		mIsJump = true;
	}
	void OnTriggerExit(Collider _collider)
	{
		mIsJump = false;
	}
}
