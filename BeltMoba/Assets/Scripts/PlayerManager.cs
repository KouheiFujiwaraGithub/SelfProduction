using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour {
	[SerializeField]
	private int mLocalPlayerId;
	[SerializeField]
	private Camera mUnitTargetCmaera;
	[SerializeField]
	private CanvasScaler mUnitTargetCanvasScaler;

	void Start()
	{
		var localPlayerCnt = GameData.Instance.LocalPlayerCount;
		//---------------------------------------
		//	プレイヤーカメラの設定
		var cameraViewPos = Vector2.zero;
		var cameraViewSiz = Vector2.one;
		GameData.Instance.GetLocalPlayerCameraSetting(mLocalPlayerId, ref cameraViewPos, ref cameraViewSiz);
		mUnitTargetCmaera.rect = new Rect(cameraViewPos,cameraViewSiz);

		//---------------------------------------
		//	プレイヤーUIの大きさ設定
		mUnitTargetCanvasScaler.scaleFactor = 0.5f;//1f - ( 0.25f * (localPlayerCnt -1));
	}
}