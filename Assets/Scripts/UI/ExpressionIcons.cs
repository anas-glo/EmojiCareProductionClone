﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionIcons : MonoBehaviour {

	public Sprite[] emojiExpressionIcons = new Sprite[40];
	public Sprite[] gumijiExpressionIcons = new Sprite[40];
	public string[] emojiExpressionUnlockCondition = new string[40];

	public Sprite GetExpressionIcon(EmojiType emojiType, int expression){
		if(emojiType == EmojiType.Emoji){
			return emojiExpressionIcons[expression-1];
		} else if(emojiType == EmojiType.Gumiji){
			return gumijiExpressionIcons[expression-1];
		}
		else return emojiExpressionIcons[0]; //temp default
	}

	public string GetExpressionName(EmojiType emojiType, int expression){
		return emojiExpressionIcons[expression].name.Substring(3);
	}

	public string GetExpressionUnlockCondition(EmojiType emojiType, int expression){
		return emojiExpressionUnlockCondition[expression];
	}
}
