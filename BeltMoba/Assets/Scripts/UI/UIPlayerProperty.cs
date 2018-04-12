using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerProperty : MonoBehaviour {
	[SerializeField]
	protected UnitManagerPlayer mPlayerManager;
	[SerializeField]
	private Transform mPropertysTarget;
	[SerializeField]
	protected Vector2 mPropertyOffset;
	protected List<UIProperty> mPropertys = new List<UIProperty>();
	[SerializeField]
	protected Image mSelectImage;

	protected enum CONTROL {
		RIGHT	= 1,
		LEFT	= 2,
		UP		= 3,
		DOWN	= 4
	};

	protected Common.ITEM_KIND mPropertyKind;
	protected bool mIsControl = true;
	protected int mCurrentPropertyIndx = 0;

	private int mPrePropertyListCount = 0;

	protected List<ItemData> mPropertyList = new List<ItemData>();

	void Awake()
	{
		Init();
	}

	void OnEnable()
	{
		OnPropertyListUpdate(true);
	}

	virtual protected void Update()
	{
		if(mPrePropertyListCount != mPropertyList.Count){
			OnPropertyListUpdate();
		}

		if(mIsControl){
			InputUpdate();
		}

		mPrePropertyListCount = mPropertyList.Count;
	}

	virtual protected void InputUpdate()
	{
		var sticX = Input.GetAxis("joycon_horizontal_" + mPlayerManager.PlayerId);
		var sticY = Input.GetAxis("joycon_vertical_" + mPlayerManager.PlayerId);
		if(sticY > 0 && mPropertyOffset.y > 0) {
			StartCoroutine(OnChangeSelect(CONTROL.UP));
		}
		else if(sticY < 0 && mPropertyOffset.y > 0) {
			StartCoroutine(OnChangeSelect(CONTROL.DOWN));
		}
		else if(sticX > 0 && mPropertyOffset.x > 0) {
			StartCoroutine(OnChangeSelect(CONTROL.RIGHT));
		}
		else if(sticX < 0 && mPropertyOffset.x > 0) {
			StartCoroutine(OnChangeSelect(CONTROL.LEFT));
		}

		if(Input.GetButtonDown("joycon_button_1_" + mPlayerManager.PlayerId)) {
			StartCoroutine(OnDecide());
		}
		else if(Input.GetButtonDown("joycon_button_0_" + mPlayerManager.PlayerId)) {
			StartCoroutine(OnCancel());
		}
		else if(Input.GetButtonDown("joycon_button_3_" + mPlayerManager.PlayerId)) {
			StartCoroutine(OnAction());
		}
		else if(Input.GetButtonDown("joycon_button_2_" + mPlayerManager.PlayerId)) {
			StartCoroutine(OnDrop());
		}
	}

	public void OnPropertyListUpdate(bool _isAppointClear = false)
	{
		List<int> selectingIndex = new List<int>();
		for(var i = 0; i < mPropertys.Count; i++) {
			if(mPropertys[i].IsAppoint){
				selectingIndex.Add(i);
			}
		}

		var propertyList  = mPropertyList;
		for(var i = 0; i < propertyList.Count; i++) {
			mPropertys[i].SetData(propertyList[i], _isAppointClear);
		}
		for(var i = propertyList.Count; i < mPropertys.Count; i++) {
			mPropertys[i].OnEmpty();
		}
		
		for(var i = 0; i < selectingIndex.Count; i++){
			if(selectingIndex[i] == 0 || selectingIndex[i] < propertyList.Count){
				continue;
			}
			mPropertys[selectingIndex[i] -1].OnAppoint();
			mPropertys[selectingIndex[i]].RemoveAppoint();
		}
		
		if(_isAppointClear){
			mCurrentPropertyIndx 			= 0;
			mSelectImage.transform.position = mPropertys[mCurrentPropertyIndx].transform.position;
		}
	}
	protected IEnumerator OnChangeSelect( CONTROL _control )
	{
		mIsControl = false;

		switch(_control){
		case CONTROL.RIGHT:
			if((mCurrentPropertyIndx + 1) % mPropertyOffset.x == 0) {
				mCurrentPropertyIndx -= (int)(mPropertyOffset.x -1);
			}
			else {
				mCurrentPropertyIndx += 1;
			}
			break;
		case CONTROL.LEFT:
			if(mCurrentPropertyIndx % mPropertyOffset.x == 0 || mCurrentPropertyIndx == 0) {
				mCurrentPropertyIndx += (int)(mPropertyOffset.x -1);
			}
			else {
				mCurrentPropertyIndx -= 1;
			}
			break;
		case CONTROL.UP:
			if(mCurrentPropertyIndx - mPropertyOffset.x < 0) {
				mCurrentPropertyIndx += (int)((mPropertyOffset.y  -1) * mPropertyOffset.x);
			}
			else {
				mCurrentPropertyIndx -= (int)mPropertyOffset.x;
			} 
			break;
		case CONTROL.DOWN:
			if(mCurrentPropertyIndx + mPropertyOffset.x > mPropertys.Count -1) {
				mCurrentPropertyIndx -= (int)((mPropertyOffset.y  -1) * mPropertyOffset.x);
			}
			else {
				mCurrentPropertyIndx += (int)mPropertyOffset.x;
			}
			break;
		}

		var limitTime = 0.1f;
		var countTime = 0f;
		while(limitTime > countTime) {
			countTime += Time.deltaTime;
			mSelectImage.transform.position = Vector3.Lerp(mSelectImage.transform.position, mPropertys[mCurrentPropertyIndx].transform.position, 0.5f);

			yield return null;
		}
		mSelectImage.transform.position = mPropertys[mCurrentPropertyIndx].transform.position;

		mIsControl = true;
		yield return null;
	}

	public ItemData GetCurrentPropertyData(){
		if(mPropertys[mCurrentPropertyIndx].IsEmpty) {
			return null;
		}
		else {
			return mPropertys[mCurrentPropertyIndx].Data;
		}
	}

	public UIProperty GetCurrentProprty(){
		return mPropertys[mCurrentPropertyIndx];
	}

	public void OnRemoveAllAppoint()
	{
		List<int> appointIndex = new List<int>();
		for(var i = 0; i < mPropertys.Count; i++){
			if(mPropertys[i].IsAppoint){
				appointIndex.Add(i);
				mPropertys[i].RemoveAppoint();
				Debug.Log(i);
			}
		}
		for(var i = 0; i < appointIndex.Count; i++){			
			mPropertyList.RemoveAt(appointIndex[i]-i);
		}
	}

	virtual	protected void Init()
	{
		//mSelectImage = mPropertysTarget.GetChild(0).GetComponent<Image>();
		for(var i = 1; i < mPropertysTarget.childCount; i++) {
			var uiProperty = mPropertysTarget.GetChild(i).gameObject.GetComponent<UIProperty>();
			mPropertys.Add(uiProperty);
		}
	}
	virtual public void ChangeActiveDescription(bool _isActive)
    {
        mIsControl = _isActive;
		mSelectImage.gameObject.SetActive(_isActive);
    }

	virtual protected IEnumerator OnDecide(){ yield return null; }
	virtual protected IEnumerator OnCancel(){ yield return null; }
	virtual protected IEnumerator OnAction(){ yield return null; }
	virtual protected IEnumerator OnDrop()
	{
		if(!mIsControl || GetCurrentProprty().IsEmpty || GetCurrentProprty().IsAppoint) {
			yield break;
		}

		var itemData	= mPropertyList[mCurrentPropertyIndx];
		var itemObj		= (GameObject)Instantiate(GameData.Instance.ItemObject);

		itemObj.transform.position = mPlayerManager.transform.position + new Vector3(0f, 3f, 0f);
		itemObj.GetComponent<ItemObject>().Init(itemData, mPlayerManager);

		itemObj.GetComponent<Rigidbody>().AddForce(mPlayerManager.Body.forward * 400f);

		mPropertyList.RemoveAt(mCurrentPropertyIndx);
		
		OnPropertyListUpdate();
		yield return null; 
	}
}
