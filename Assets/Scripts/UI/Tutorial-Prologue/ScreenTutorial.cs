﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialType{
	FirstVisit,
	IdleLivingRoom,
	FirstBedroom,
	FirstBathroom,
	FirstKitchen,
	FirstPlayroom,
	FirstGarden,
	FirstProgressUI,
	FirstEditRoomUI,

	TriggerHungerRed,
	TriggerHygieneRed,
	TriggerHappinessRed,
	TriggerStaminaRed,
	TriggerHealthOrange,
	TriggerHealthRed,
	TriggerFirstExpressionFull,
	TriggerFirstDead
}

public class ScreenTutorial : BaseUI {
	public string[] firstVisit;
	public string[] idleLivingRoom;
	public string[] firstBedroom;
	public string[] firstBathroom;
	public string[] firstKitchen;
	public string[] firstPlayroom;
	public string[] firstGarden;
	public string[] firstProgressUI;
	public string[] firstEditRoomsUI;

	public string[] triggerHungerRed;
	public string[] triggerHygieneRed;
	public string[] triggerHappinessRed;
	public string[] triggerStaminaRed;
	public string[] triggerHealthOrange;
	public string[] triggerHealthRed;
	public string[] triggerFirstExpressionFull;
	public string[] triggerFirstDead;

	public ProloguePopupName popupName;
	public GameObject screenTutorialObj;
	public Text dialogText;

	public List<GameObject> firstTutorialPanels = new List<GameObject>();
	TutorialType currentTutorial = TutorialType.FirstVisit;
	int panelCount = 0;
	int dialogCount = 0;

	bool roomChange = false;

	void Start(){
		dialogText.text = firstVisit [0];
	}

	public override void InitUI ()
	{
		dialogCount = 0;
	}

	public void ShowTutorialPanels ()
	{
		if (panelCount < firstTutorialPanels.Count) {
			firstTutorialPanels [panelCount].SetActive (true);
			panelCount++;
		}
		if(panelCount == firstTutorialPanels.Count){
			PlayerData.Instance.PlayerFirstPlay = 1;
		}
	}

	void ShowPopup(){
		popupName.ShowUI(popupName.gameObject);
	}

	public void ClosePopup(){
		popupName.CloseUI (popupName.gameObject);
		OnClickNext ();
	}

	public void ShowFirstDialog(TutorialType type){
		string emojiName = PlayerData.Instance.EmojiName;
		if(type == TutorialType.IdleLivingRoom){
			dialogText.text = idleLivingRoom [0];
		} else if(type == TutorialType.FirstBathroom){
			dialogText.text = "If "+emojiName+" gets dirty, give it a good bath.";
		} else if(type == TutorialType.FirstBedroom){
			dialogText.text = "Sometimes "+emojiName+" will get tired";
		} else if(type == TutorialType.FirstEditRoomUI){
			dialogText.text = firstEditRoomsUI [0];
		} else if(type == TutorialType.FirstGarden){
			dialogText.text = firstGarden [0];
		} else if(type == TutorialType.FirstKitchen){
			dialogText.text = "Don't forget to feed "+emojiName;
		} else if(type == TutorialType.FirstPlayroom){
			dialogText.text = firstPlayroom [0];
		} else if(type == TutorialType.FirstProgressUI){
			dialogText.text = firstProgressUI [0];
		} else if(type == TutorialType.TriggerFirstDead){
			dialogText.text = triggerFirstDead [0];
		} else if(type == TutorialType.TriggerFirstExpressionFull){
			dialogText.text = triggerFirstExpressionFull [0];
		} else if(type == TutorialType.TriggerHappinessRed){
			dialogText.text = triggerHappinessRed [0];
		} else if(type == TutorialType.TriggerHealthOrange){
			dialogText.text = triggerHealthOrange [0];
		} else if(type == TutorialType.TriggerHealthRed){
			dialogText.text = triggerHealthRed [0];
		} else if(type == TutorialType.TriggerHungerRed){
			dialogText.text = triggerHungerRed [0];
		} else if(type == TutorialType.TriggerHygieneRed){
			dialogText.text = triggerHygieneRed [0];
		} else if(type == TutorialType.TriggerStaminaRed){
			dialogText.text = triggerStaminaRed [0];
		} else if(type == TutorialType.TriggerStaminaRed){
			dialogText.text = triggerStaminaRed [0];
		}
		base.ShowUI (screenTutorialObj);
	}

	public void CheckRoomPlayerPrefs(RoomType roomType){
		bool showTutorial = true;

		if(roomType == RoomType.Bedroom){
			currentTutorial = TutorialType.FirstBedroom;
			if(PlayerData.Instance.TutorialFirstBedroom == 0){
				PlayerData.Instance.TutorialFirstBedroom = 1;
			}else{
				showTutorial = false;
			}
		} else if(roomType == RoomType.Bathroom){
			currentTutorial = TutorialType.FirstBathroom;
			if(PlayerData.Instance.TutorialFirstBathroom == 0){
				PlayerData.Instance.TutorialFirstBathroom = 1;
			}else{
				showTutorial = false;
			}
		} else if(roomType == RoomType.Garden){
			currentTutorial = TutorialType.FirstGarden;
			if(PlayerData.Instance.TutorialFirstGarden == 0){
				PlayerData.Instance.TutorialFirstGarden = 1;
			}else{
				showTutorial = false;
			}
		} else if(roomType == RoomType.Kitchen){
			currentTutorial = TutorialType.FirstKitchen;
			if(PlayerData.Instance.TutorialFirstKitchen == 0){
				PlayerData.Instance.TutorialFirstKitchen = 1;
			}else{
				showTutorial = false;
			}
		} else if(roomType == RoomType.Playroom){
			currentTutorial = TutorialType.FirstPlayroom;
			if(PlayerData.Instance.TutorialFirstPlayroom == 0){
				PlayerData.Instance.TutorialFirstPlayroom = 1;
			}else{
				showTutorial = false;
			}
		} 

		if(showTutorial){
			ShowFirstDialog (currentTutorial);
		}
	}

	public void OnClickNext(){
		bool loadCustomDialog = false;
		string emojiName = PlayerData.Instance.EmojiName;
		//emojiName = "Hai";

		if(currentTutorial == TutorialType.FirstVisit){
			if(dialogCount < (firstVisit.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstVisit = 1;
				StartCoroutine (WaitForRoomChange ());
			}

			if(dialogCount == 1){
				ShowPopup ();
			} else if(dialogCount==2){
				dialogText.text = emojiName + firstVisit [dialogCount];
				loadCustomDialog = true;
			} else if(dialogCount == 3){
				dialogText.text = firstVisit [dialogCount] + emojiName;
				loadCustomDialog = true;
			} else if(dialogCount == 4){
				dialogText.text = "You can caress and hold "+emojiName+" with your finger.";
				loadCustomDialog = true;
			} else if(dialogCount == 5){
				dialogText.text = "If you raise with "+emojiName+" care, it will have many various expressions! ";
				loadCustomDialog = true;
			}
			if (!loadCustomDialog) {
				dialogText.text = firstVisit [dialogCount];
			}
		} else if(currentTutorial == TutorialType.IdleLivingRoom){
			if(dialogCount < (idleLivingRoom.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialIdleLivingRoom = 1;
			}
			dialogText.text = idleLivingRoom [dialogCount];

		} else if(currentTutorial == TutorialType.FirstBedroom){
			if(dialogCount < (firstBedroom.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstBedroom = 1;
			}
			dialogText.text = firstBedroom [dialogCount];

		} else if(currentTutorial == TutorialType.FirstBathroom){
			if(dialogCount < (firstBathroom.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstBathroom = 1;
			}

			if(dialogCount == 2){
				dialogText.text = "wash them nicely on "+emojiName;
				loadCustomDialog = true;
			}
			if(!loadCustomDialog)
				dialogText.text = firstBathroom [dialogCount];
			
		} else if(currentTutorial == TutorialType.FirstKitchen){
			if(dialogCount < (firstKitchen.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstKitchen = 1;
			}

			if(dialogCount == 5){
				dialogText.text = "When cooked, bring them to " + emojiName + " for eat";
				loadCustomDialog = true;
			}

			if(!loadCustomDialog)
				dialogText.text = firstKitchen [dialogCount];

		} else if(currentTutorial == TutorialType.FirstPlayroom){
			if(dialogCount < (firstPlayroom.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstPlayroom = 1;
			}
			dialogText.text = firstPlayroom [dialogCount];

		} else if(currentTutorial == TutorialType.FirstGarden){
			if(dialogCount < (firstGarden.Length-1)){
				dialogCount++;
			} else{
				base.CloseUI (screenTutorialObj);
				PlayerData.Instance.TutorialFirstGarden = 1;
			}
			dialogText.text = firstGarden [dialogCount];
		}
	}

	public void TriggerRoomChange(){
		roomChange = true;
	}

	IEnumerator WaitForRoomChange(){
		Debug.Log ("START WAITING");
		yield return new WaitForSeconds (10f);
		if (!roomChange) {
			currentTutorial = TutorialType.IdleLivingRoom;
			ShowFirstDialog (currentTutorial);
		}else{
			PlayerData.Instance.TutorialIdleLivingRoom = 1;
		}
	}
}
