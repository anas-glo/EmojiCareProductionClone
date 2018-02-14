﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTitleManager : MonoBehaviour {
	public Fader fader;
	public SceneLoader sceneLoader;

	string nextScene;

	void Start(){
		SoundManager.Instance.PlayBGM(BGMList.BGMTitle);

		Input.multiTouchEnabled = false;
//		PlayerPrefs.DeleteAll();
		Fader.OnFadeOutFinished += HandleFadeOutFinished;
		GameSparkManager.Instance.OnLoginSuccessful += GoToSceneMain;

//		if(! string.IsNullOrEmpty(PlayerData.Instance.PlayerAuthToken)){
//			//authenticate with playertoken.(BUT HOW????)
//		}

		//PlayerData.Instance.Shop = 1;
	}

	IEnumerator FaderFadeIn()
	{
		yield return new WaitForSeconds(1f);
		fader.FadeIn();
	}

	void HandleFadeOutFinished(){
//		panelLoadingBar.gameObject.SetActive(true);
//		panelLoadingBar.NextScene = nextScene;
		sceneLoader.gameObject.SetActive(true);

		if(PlayerData.Instance.PlayerFirstPlay == 0){
			sceneLoader.NextScene = "ScenePrologue";
		} else{
			sceneLoader.NextScene = "SceneMain";
		}

		Fader.OnFadeOutFinished -= HandleFadeOutFinished;

		//SceneManager.LoadScene("SceneMain");

	}

	public void TapToStart ()
	{
		if (GooglePlayGamesManager.Instance)
			GooglePlayGamesManager.Instance.GPGSLogin ();
		if (SoundManager.Instance)
			SoundManager.Instance.PlaySFXOneShot (SFXList.TapToStart);
		fader.FadeOut();
	}

	public void DownloadEmojiObject()
	{
//		GameSparkManager.Instance.GetDownloadableURL(PlayerData.Instance.PlayerEmojiType);
	}

	void GoToSceneMain()
	{
		fader.gameObject.SetActive(true);
		fader.FadeOut();
	}

}