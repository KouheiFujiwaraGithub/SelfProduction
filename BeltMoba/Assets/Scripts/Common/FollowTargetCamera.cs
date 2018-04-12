using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetCamera : MonoBehaviour {
	[SerializeField]
	private Transform mTarget;
	[SerializeField]
	private Transform mBody;
	[SerializeField]
	private float mOffsetY;
	[SerializeField]
	private float mOffsetZ;
	[SerializeField]
	private float mOffsetFront;
	[SerializeField]
	private float mSmoothTime = 0.5f;
	[SerializeField]
	private float mAngleX = 0f;

	void LateUpdate()
	{
		var front = 1;
		if(mBody.rotation.y < 0){
			front = -1;
		}
		var offset = new Vector3(front * mOffsetFront, mOffsetY, mOffsetZ);
		this.transform.position = Vector3.Lerp(this.transform.position, mTarget.position + offset, mSmoothTime);
		this.transform.rotation = Quaternion.Euler(Vector3.right * mAngleX);
	}

	public void OnPositionReset()
	{
		var front = 1;
		if(mBody.rotation.y < 0){
			front = -1;
		}
		var offset = new Vector3(front * mOffsetFront, mOffsetY, mOffsetZ);
		this.transform.position = mTarget.position + offset;
	}
}
