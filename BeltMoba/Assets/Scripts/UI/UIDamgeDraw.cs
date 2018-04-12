using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamgeDraw : MonoBehaviour {	
	private Text mDraw;
	private bool mIsDrawing;

	void Awake()
	{
		mDraw = this.GetComponent<Text>();
	}
	public void Init(float _damage ,Vector3 _pos)
	{
		StartCoroutine(DrawStart(_damage, _pos));
	}

	private IEnumerator DrawStart(float _damage, Vector3 _pos)
	{
		if(mIsDrawing){
			yield break;
		}

		mIsDrawing					 = true;
		mDraw.color 				 = Color.magenta; 
		mDraw.text					 = _damage.ToString();
		this.transform.position 	 = _pos + new Vector3(0f, 0f, -2f);

		var timer = 3f;
		while(timer > 0f){
			timer -= Time.deltaTime;
			this.transform.localPosition += Vector3.up * timer * 0.01f;
			var alpha = 1f - (timer / 3f);
			mDraw.color -= new Color(0f,0f,0f,alpha);
			yield return null;
		}

		Destroy(this.gameObject);
		yield return null;
	}
}
