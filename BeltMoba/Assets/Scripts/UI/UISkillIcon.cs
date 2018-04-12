using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillIcon : MonoBehaviour {
	[SerializeField]
	private Image mImgIcon;
	[SerializeField]
	private Image mImgRest;
	[SerializeField]
	private Text mSp;
	private SkillManager mSkillManager;

	private float mRestTimer = 0f;

	public void SetSkillManager(SkillManager _skillManager)
	{
		mSkillManager	= _skillManager;
		StartCoroutine(WaitSetSkillManager());
	}

	private IEnumerator WaitSetSkillManager(){
		yield return new WaitWhile(()=> mSkillManager.SkillData == null);
		
		mImgIcon.sprite = mSkillManager.SkillData.Sprite;
		mSp.text 		= mSkillManager.SkillData.NeedStamina.ToString();
		yield return null;
	}

	void Update(){
		if(mSkillManager != null && mSkillManager.SkillData != null){
			if(mSkillManager.IsSkill){
				mRestTimer = 0f;
				mImgRest.fillAmount = 0f;
			}
			else{
				mRestTimer += Time.deltaTime;
				mImgRest.fillAmount =　1f - (mRestTimer / mSkillManager.SkillData.RestTime);
			}
		}
	}
}
