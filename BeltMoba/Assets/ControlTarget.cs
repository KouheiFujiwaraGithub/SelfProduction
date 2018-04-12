using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlTarget : MonoBehaviour {
	// mControlGauge.fillRect.gameObject.GetComponent<Image>().color = mCurrentControlTeamColor;
	[SerializeField]
	Color mColor = Color.white;
	[SerializeField]
	MeshRenderer mMeshRenderer;

	void Update()
	{
		mMeshRenderer.material.color = mColor;
	}
}
