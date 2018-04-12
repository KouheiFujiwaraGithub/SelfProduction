using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotation : MonoBehaviour {
	 // Update is called once per frame
	[SerializeField]
	private Vector3 mAxis = Vector3.up;
	[SerializeField]
	private float mAngule = 45f;
    void Update () {
        // Y軸(Vector3.up)周りを１フレーム分の角度だけ回転させるQuaternionを作成
        Quaternion rot = Quaternion.AngleAxis(mAngule * -Time.deltaTime, mAxis);

        // 元の回転値と合成して上書き
        this.transform.localRotation = rot * this.transform.localRotation;
    }
}
