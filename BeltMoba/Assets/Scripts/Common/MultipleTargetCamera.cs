using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MultipleTargetCamera : MonoBehaviour {
	[SerializeField]
	private List<Transform> mTargets = new List<Transform>();
	[SerializeField]
	private Vector3 mOffset = Vector3.zero;
	[SerializeField]
	private float mSmoothTime = 0.5f;
	[SerializeField]
	private float mMinZoom = 0f;
	[SerializeField]
	private float mMaxZoom = 0f;
	[SerializeField]
	private float mZoomLimit = 0f;

	private Camera mCamera;
	private Vector3 mMoveVelocity;

	void Start()
	{
		mCamera = this.GetComponent<Camera>();
	}

	void LateUpdate()
	{
		if(mTargets.Count == 0) {
			return;
		}
		Move();
		Zoom();
	}

	private void Move()
	{
		var centerPos = GetCenterPoint();
		var newPos = centerPos + mOffset;

		this.transform.position = Vector3.SmoothDamp(this.transform.position, newPos, ref mMoveVelocity, mSmoothTime);
	}

	private void Zoom()
	{
		var newZoom = Mathf.Lerp(mMaxZoom, mMinZoom, GetGreatesDistance() / mZoomLimit);
		mCamera.fieldOfView = Mathf.Lerp(mCamera.fieldOfView, newZoom, Time.deltaTime);
	}

	private float GetGreatesDistance()
	{
		var bounds = new Bounds(mTargets[0].position, Vector3.zero);
		for(var i = 1; i < mTargets.Count; ++i) {
			if(!mTargets[i] || !mTargets[i].gameObject.activeSelf) {
				continue;
			}
			bounds.Encapsulate(mTargets[i].position);
		}

		var value = bounds.size.x;
		if(bounds.size.x < bounds.size.y) {
			value = bounds.size.y;
		}
		return value;
	}

	private Vector3 GetCenterPoint()
	{
		var bounds = new Bounds(mTargets[0].position, Vector3.zero);
		for(var i = 1; i < mTargets.Count; ++i) {
			if(!mTargets[i]) {
				continue;
			}
			bounds.Encapsulate(mTargets[i].position);
		}
		return bounds.center;
	}
}