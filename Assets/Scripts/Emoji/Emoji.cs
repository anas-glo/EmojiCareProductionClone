﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum EmojiStatss{
	Hunger,
	hygene,
	Happiness,
	Stamina,
	Health
}

public class Emoji : MonoBehaviour {
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region delegate events
	public delegate void EmojiTickStats();
	public delegate void EmojiDead();
	public delegate void UpdateStatsToExpression(float hunger, float hygiene, float happiness, float stamina, float health);

	public event EmojiTickStats OnEmojiTickStats;
	public static event EmojiDead OnEmojiDead;
	public event UpdateStatsToExpression OnUpdateStatsToExpression;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region attribute
	[Header("Reference")]
	public Rigidbody2D thisRigidbody;
	public EmojiBody body;
	public EmojiTriggerFall triggerFall;
	public EmojiPlayerInput playerInput;
	public EmojiSO emojiBaseData;

	[Header("")]
	public string emojiName;
	public EmojiStats hunger, hygiene,happiness,stamina, health;
	public EmojiExpression emojiExpressions;
	public EmojiActivity activity;
	public bool interactable = true;

	DateTime lastTimePlayed{
		get{return DateTime.Parse(PlayerPrefs.GetString(PlayerPrefKeys.Player.LAST_TIME_PLAYED));}
		set{PlayerPrefs.SetString(PlayerPrefKeys.Player.LAST_TIME_PLAYED,value.ToString());}
	}

	DateTime timeOnPause{
		get{return DateTime.Parse(PlayerPrefs.GetString(PlayerPrefKeys.Player.TIME_ON_PAUSE));}
		set{PlayerPrefs.SetString(PlayerPrefKeys.Player.TIME_ON_PAUSE,value.ToString());}
	}

	//stats
	public const float statsTresholdHigh = 0.9f;
	public const float statsTresholdMed = 0.4f;
	public const float statsTresholdLow = 0.2f;
	float[] healthTick = new float[]{0.0001f,0.0003f,0.0006f,0.0012f};

	bool isTickingStat = false;
	bool hasInit = false;
	bool emojiDead = false;

	//interactions
	bool flagHold = false;
	bool flagStroke = false;

	bool isDoubleTap = false;
	int doubleTapCounter = 0;

	int shakeCounter = 0;
	float prevX = 0;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	public void Init()
	{
		if(!hasInit){
			hasInit = true;
			InitEmojiExpression();
			InitEmojiStats();
		}
	}

	void InitEmojiStats()
	{
		hunger = 	new EmojiStats( PlayerPrefKeys.Emoji.HUNGER, 	emojiBaseData.hungerModifier, 	 emojiBaseData.maxStatValue, emojiBaseData.hungerStart );
		hygiene = 	new EmojiStats( PlayerPrefKeys.Emoji.HYGENE, 	emojiBaseData.hygeneModifier, 	 emojiBaseData.maxStatValue, emojiBaseData.hygeneStart );
		happiness = new EmojiStats( PlayerPrefKeys.Emoji.HAPPINESS, emojiBaseData.happinessModifier, emojiBaseData.maxStatValue, emojiBaseData.happinessStart );
		stamina = 	new EmojiStats( PlayerPrefKeys.Emoji.STAMINA, 	emojiBaseData.staminaModifier, 	 emojiBaseData.maxStatValue, emojiBaseData.staminaStart );
		health = 	new EmojiStats( PlayerPrefKeys.Emoji.HEALTH, 	emojiBaseData.healthModifier, 	 emojiBaseData.maxStatValue, emojiBaseData.healthStart );

		int totalTicks = 0;
		if(PlayerPrefs.HasKey(PlayerPrefKeys.Player.LAST_TIME_PLAYED)){
			if(DateTime.Now.CompareTo(lastTimePlayed) > 0){
				totalTicks = GetTotalTicks(DateTime.Now - lastTimePlayed);
			}
		}

		for(int i = 0;i<totalTicks;i++){ 
			if(!emojiDead) TickStats();
			else break;
		}

		if(!emojiDead){ 
			StartCoroutine(_TickingStats);
		}
	}

	void InitEmojiExpression()
	{
		emojiExpressions.Init();
		emojiExpressions.SetExpression(EmojiExpressionState.DEFAULT,0);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanic
	void ResumeTickingStats()
	{
		int totalTicks = 0;
		if(PlayerPrefs.HasKey(PlayerPrefKeys.Player.TIME_ON_PAUSE)){
			if(DateTime.Now.CompareTo(timeOnPause) > 0){
				totalTicks = GetTotalTicks(DateTime.Now - timeOnPause);
			}
		}

		for(int i = 0;i<totalTicks;i++){ 
			if(emojiDead) break;
			TickStats();
		}
		if(!emojiDead)	StartCoroutine(_TickingStats);
	}

	void TickStats()
	{
		hunger.TickStats();
		hygiene.TickStats();
		happiness.TickStats();
		stamina.TickStats();
		TickHealth();

		if(OnUpdateStatsToExpression != null) 
			OnUpdateStatsToExpression(
				hunger.StatValue	/ hunger.MaxStatValue,
				hygiene.StatValue	/ hygiene.MaxStatValue,
				happiness.StatValue	/ happiness.MaxStatValue,
				stamina.StatValue	/ stamina.MaxStatValue,
				health.StatValue	/ health.MaxStatValue
			);
	}

	void TickHealth()
	{
		float hungerValue = hunger.StatValue/hunger.MaxStatValue;
		float hygieneValue = hygiene.StatValue/hygiene.MaxStatValue;
		float happinessValue = happiness.StatValue/happiness.MaxStatValue;
		float staminaValue = stamina.StatValue/stamina.MaxStatValue;

		int LowStatsCounter = 0;
		if(hungerValue < statsTresholdLow) LowStatsCounter++;
		if(hygieneValue < statsTresholdLow) LowStatsCounter++;
		if(happinessValue < statsTresholdLow) LowStatsCounter++;
		if(staminaValue < statsTresholdLow) LowStatsCounter++;

		if(LowStatsCounter < 2){
			int highStatsCounter = 0;
			if(hungerValue >= statsTresholdHigh) highStatsCounter++;
			if(hygieneValue >= statsTresholdHigh) highStatsCounter++;
			if(happinessValue >= statsTresholdHigh) highStatsCounter++;
			if(staminaValue >= statsTresholdHigh) highStatsCounter++;

			if(highStatsCounter > 0){
				health.statsModifier = healthTick[highStatsCounter-1];
			}else{
				health.statsModifier = 0f;
			}
		}else{
			health.statsModifier = -1 * healthTick[LowStatsCounter-1];
		}

		health.TickStats();
	}

	int GetTotalTicks(TimeSpan duration)
	{
		int dayToSec = duration.Days * 24 * 60 * 60;
		int hourToSec = duration.Hours * 60 * 60;
		int minToSec = duration.Minutes * 60;
		int sec = duration.Seconds;

		int totalSec = dayToSec + hourToSec + minToSec;

		return totalSec;
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public module
	public void ResetEmojiStatsModifier()
	{
		hunger.statsModifier = emojiBaseData.hungerModifier;
		hygiene.statsModifier = emojiBaseData.hygeneModifier;
		happiness.statsModifier = emojiBaseData.happinessModifier;
		stamina.statsModifier = emojiBaseData.staminaModifier;
	}

	public void ModAllStats(float[] mod)
	{
		hunger.ModStats(mod[0]);
		hygiene.ModStats(mod[1]);
		happiness.ModStats(mod[2]);
		stamina.ModStats(mod[3]);
		health.ModStats(mod[4]);
	}

	public void SwitchDebugMode(bool debug)
	{
		hunger.Debug = debug;
		hygiene.Debug = debug;
		happiness.Debug = debug;
		stamina.Debug = debug;
		health.Debug = debug;

	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region coroutines
	const string _TickingStats = "StartTickingStats";
	IEnumerator StartTickingStats()
	{
		isTickingStat = true;

		while(true){
			yield return new WaitForSeconds(1f);
			TickStats();
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------

	void OnApplicationPause(bool isPaused)
	{
		if(isPaused){ 
			isTickingStat = false;
			StopCoroutine(_TickingStats);
			timeOnPause = DateTime.Now;
		}
		else{
//			ResumeTickingStats();
		}
	}

	void OnApplicationQuit()
	{
		isTickingStat = false;
		StopCoroutine(_TickingStats);
		lastTimePlayed = DateTime.Now;
	}
}