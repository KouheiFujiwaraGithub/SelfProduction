using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AutoLineRenderer : MonoBehaviour {
	private List<Vector3> mLinePointList = new List<Vector3>();
	void Start()
	{
		var lineRenderer = this.gameObject.GetComponent<LineRenderer>();
		lineRenderer.positionCount = this.transform.childCount;
		for(var i = 0; i < this.transform.childCount; i++){
			mLinePointList.Add(this.transform.GetChild(i).position);
			lineRenderer.SetPosition(i, this.transform.GetChild(i).position);
		}
		
		// lineRenderer.SetPositions(mLinePointList.ToArray());
	}
}
