﻿using System.Collections;
using UnityEngine;

public enum EmojiAgeType{
	Baby,
	Juvenille,
	Adult
}

public class EmojiGrowth : MonoBehaviour {

	public delegate void NewGrowth(EmojiAgeType type);
	public event NewGrowth OnNewGrowth;

	#region attributes
	public Emoji emoji;
	public float scaleSmall = 0.5f;
	public float scaleMedium = 0.65f;
	public float scaleLarge = 0.8f;

	const float tresholdLow = 0.3f;
	const float tresholdMed = 0.7f;
	const float tresholdHigh = 1f;

	float newScaleValue;

	float currentScale{
		get{return PlayerPrefs.GetFloat(PlayerPrefKeys.Emoji.CURRENT_SCALE,scaleSmall);}
		set{PlayerPrefs.SetFloat(PlayerPrefKeys.Emoji.CURRENT_SCALE,value);}
	}

	public Vector3 GetScale()
	{
		return new Vector3(currentScale,currentScale,1f);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	public void Init()
	{
		
	}

	public void UpdateGrowth(float progress,bool absScale)
	{
		float oldScaleValue = emoji.transform.localScale.x;

		if(absScale){
			oldScaleValue = Mathf.Abs (oldScaleValue);
		}

		newScaleValue = GetScaleValue(progress);
//		Debug.Log ("oldscale:" + oldScaleValue);
//		Debug.Log ("newscale:" + newScaleValue);
		if (oldScaleValue != newScaleValue) {
//			Debug.Log ("growth event");
			EmojiAgeType type = newScaleValue == scaleSmall ? EmojiAgeType.Baby : newScaleValue == scaleMedium ? EmojiAgeType.Juvenille : EmojiAgeType.Adult;
			if(OnNewGrowth != null) OnNewGrowth(type);
			//panggil popup celebration
			//register close popUp
		}
	
		//NewGrowth(newScaleValue);
	}

	public void OnClosePopup() {
		DoGrow(newScaleValue);
	}

	public void DoGrow(float newScaleValue) {
		print("Current Scale = "+emoji.transform.localScale.x);
		print("New Scale = "+newScaleValue);
		if(emoji.transform.localScale.x != scaleLarge && newScaleValue == scaleLarge){
			print("Emoji Grow to Adult!");
			emoji.GetComponent<BabyEmoji>().GrowToAdult();
		}else if(newScaleValue > currentScale){
			print("Emoji Grow to Teenager!");
			currentScale = newScaleValue;
			emoji.GetComponent<BabyEmoji>().GrowToJuvenille();
		}
	}

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules
	float GetScaleValue(float progress)
	{
		print("Progress = "+progress);
		if(progress < tresholdLow){
			return scaleSmall;
		}else if(progress >= tresholdLow && progress < tresholdMed){
			return scaleMedium;
		}else{
			return scaleLarge;
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
}