using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CullingMasSwitchPositionCamera : MonoBehaviour {
	[SerializeField]
	private Transform[] mSwitchTargets;
	[SerializeField]
	private float mOffsetDepth = 0f;
	private Camera mCamera;
	
	void Start()
	{
		mCamera = this.gameObject.GetComponent<Camera>();
	}

	void Update()
	{
		var off = 0;
		var on	= 0;
		for(var i = 0; i < mSwitchTargets.Length; i++){
			if(this.transform.position.z + mOffsetDepth > mSwitchTargets[i].position.z){
				off |= 1 << mSwitchTargets[i].gameObject.layer;
			}
			else
			{
				on |= 1 << mSwitchTargets[i].gameObject.layer;
			}
		}
		mCamera.cullingMask &= ~off;
		mCamera.cullingMask |= on;
	}	
}
