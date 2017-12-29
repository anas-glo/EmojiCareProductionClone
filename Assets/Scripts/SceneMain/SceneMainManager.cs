﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class SceneMainManager : MonoBehaviour {
	#region attributes
	[Header("Attributes")]
	public RoomController roomController;
	public Fader fader;

	[Header("Event Attributes")]
	public ScreenTutorial screenTutorial;
	public FloatingStatsManager floatingStats;
	public EmojiStatsExpressionController statsExpressionController;
	public UICelebrationManager celebrationManager;
	public GachaReward gachaReward;
	public HotkeysAnimation hotkeys;
	public RandomBedroomObjectController randomBedroomController;
	public Bedroom bedroom;
	public RandomBedroomObjectController randomBedroomObjectController;
	public FloatingStatsManager floatingStatsManager;

	//sementara
	public GameObject[] emojiSamples;

	//growth
	const float tresholdLow = 0.3f;
	const float tresholdMed = 0.7f;
	const float tresholdHigh = 1f;

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initialization
	void Start()
	{
		PlayerPrefs.DeleteAll();
		PlayerData.Instance.PlayerFirstPlay = 1;
		InitMain();
	}

	void OnEmojiDoneLoading ()
	{
		fader.FadeIn();
	}

	void InitMain()
	{
		
		int totalExpression = 60;


		//load expression data
		EmojiType emojiType = PlayerData.Instance.PlayerEmoji.emojiBaseData.emojiType;
		//load from json	
		if(PlayerPrefs.HasKey(PlayerPrefKeys.Emoji.UNLOCKED_EXPRESSIONS+emojiType.ToString())){
			string data = PlayerPrefs.GetString(PlayerPrefKeys.Emoji.UNLOCKED_EXPRESSIONS+emojiType.ToString());
			Debug.Log (data);
			JSONNode node = JSON.Parse(data);


			PlayerData.Instance.emojiParentTransform = roomController.rooms[(int)roomController.currentRoom].transform;

			float progress = (float)(node["EmojiUnlockedExpressions"].Count / totalExpression);
			if(progress < 0.3f){
				//baby
				PlayerData.Instance.InitPlayerBabyEmoji(EmojiAgeType.Baby, emojiSamples[PlayerData.Instance.PlayerEmojiType]);
			}else if(progress >= 0.3f && progress <  0.7f){
				//juvenille
				PlayerData.Instance.InitPlayerBabyEmoji(EmojiAgeType.Juvenille, emojiSamples[PlayerData.Instance.PlayerEmojiType]);
			}else{
				//adult

				PlayerData.Instance.InitPlayerEmoji(emojiSamples[PlayerData.Instance.PlayerEmojiType]);
			}
		}



		PlayerData.Instance.emojiParentTransform = roomController.rooms[(int)roomController.currentRoom].transform;
		PlayerData.Instance.InitPlayerEmoji(emojiSamples[PlayerData.Instance.PlayerEmojiType]);

		if(PlayerData.Instance.PlayerEmoji.EmojiSleeping){
			roomController.currentRoom = RoomType.Bedroom;
			roomController.transform.position = new Vector3(-32f,0f,0f);
			foreach(BaseRoom r in roomController.rooms) if(r != null) r.OnRoomChanged(roomController.currentRoom);

			PlayerData.Instance.PlayerEmoji.transform.parent = roomController.rooms[(int)roomController.currentRoom].transform;
			PlayerData.Instance.PlayerEmoji.transform.position = new Vector3(0,0.0025f,-2f);
			PlayerData.Instance.PlayerEmoji.emojiExpressions.SetExpression(EmojiExpressionState.SLEEP,-1);
			PlayerData.Instance.PlayerEmoji.body.DoSleep();
			bedroom.DimLight();
			randomBedroomController.StartGeneratingObjects();
		}
		PlayerData.Instance.PlayerEmoji.InitEmojiStats();

		if(PlayerData.Instance.PlayerEmoji.EmojiSleeping){
			floatingStatsManager.OnEmojiSleepEvent(true);
		}



		if(PlayerPrefs.GetInt(PlayerPrefKeys.Game.HAS_INIT_INGREDIENT,0) == 0){
			PlayerPrefs.SetInt(PlayerPrefKeys.Game.HAS_INIT_INGREDIENT,1);
			for(int i = 0;i<(int)IngredientType.COUNT;i++){
				PlayerData.Instance.inventory.SetIngredientValue((IngredientType)i,2);
			}
		}

		statsExpressionController.Init();

		roomController.Init();
		gachaReward.Init ();


		celebrationManager.Init();
		floatingStats.Init ();

		screenTutorial.RegisterEmojiEvents();
		statsExpressionController.RegisterEmojiEvents();
		celebrationManager.RegisterEmojiEvents();
		roomController.RegisterEmojiEvents();
		floatingStats.RegisterEmojiEvents();
		gachaReward.RegisterEmojiEvents();
		hotkeys.RegisterEmojiEvents();
		bedroom.RegisterEmojiEvents();
		randomBedroomController.RegisterEmojiEvents();

		if (PlayerData.Instance.TutorialFirstVisit == 0) {
			screenTutorial.ShowUI (screenTutorial.screenTutorialObj);
		}

		if(AdmobManager.Instance) AdmobManager.Instance.ShowBanner();

		fader.FadeIn();

		SoundManager.Instance.PlayBGM(BGMList.BGMMain);

		PlayerData.Instance.PlayerEmoji.OnEmojiDestroyed += OnEmojiDestroyed;
	}

	void OnEmojiDestroyed ()
	{
		PlayerData.Instance.PlayerEmoji.OnEmojiDestroyed -= OnEmojiDestroyed;

		screenTutorial.UnregisterEmojiEvents();
		statsExpressionController.UnregisterEmojiEvents();
		celebrationManager.UnregisterEmojiEvents();
		roomController.UnregisterEmojiEvents();
		floatingStats.UnregisterEmojiEvents();
		gachaReward.UnregisterEmojiEvents();
		hotkeys.UnregisterEmojiEvents();
		bedroom.UnregisterEmojiEvents();
		randomBedroomController.UnregisterEmojiEvents();
	}

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public modules

	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------	
}