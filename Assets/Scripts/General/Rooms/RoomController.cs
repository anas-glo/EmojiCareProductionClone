﻿using System;
using System.Collections;
using UnityEngine;

public class RoomController : MonoBehaviour {
	#region attributes
	//constants
	const float roomWidth = 7.2f;
	const float roomHeight = 12.8f;

	//public
	public float snapSpeed = 6f;

	//private
	BoxCollider2D thisCollider;
	RoomName currentRoom = RoomName.LivingRoom;

	int roomTotal = 0;
	float distance = 0;
	float xOnBeginDrag;
	bool snapping = false;
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region initializations
	void Awake()
	{
		Init();
	}

	void Init()
	{
		thisCollider = GetComponent<BoxCollider2D>();

		AdjustTouchAreaSize();
	}

	void Start()
	{
		GoToRoom(currentRoom);
	}

	/// <summary>
	///<para>Automatically adjust the room collider size for touch function.</para> 
	/// <para> </para>
	/// <para>- collider size (x,y) = (roomWidth * totalRoom,roomHeight)</para>
	/// <para>we want the collider position to be at 0,0 in world space, so:</para>
	/// <para>- collider offset (x,y) = ( (colliderSize.x / 2)-(roomWidth/2), 0)</para>
	/// </summary>
	void AdjustTouchAreaSize()
	{
		roomTotal = Enum.GetNames(typeof(RoomName)).Length;
		thisCollider.size = new Vector2((roomWidth*roomTotal),roomHeight);
		thisCollider.offset = new Vector2((thisCollider.size.x/2f)-(roomWidth/2f),0f);
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region mechanics
	public void BeginDrag()
	{
		if(!snapping){
			xOnBeginDrag = transform.localPosition.x;
			float x = getWorldPositionFromTouchInput().x;
			distance = transform.localPosition.x - x;
		}
	}

	public void Drag()
	{
		if(!snapping) transform.position = new Vector3(getWorldPositionFromTouchInput().x + distance,0f,0f);
	}

	public void EndDrag()
	{
		if(!snapping){
			Vector3 startPos = transform.position;
			Vector3 endpos = new Vector3(getXEndPosition(startPos.x),0f,0f);
			StartCoroutine(SmoothSnap(startPos,endpos));
		}
	}

	//proccessors
	Vector3 getWorldPositionFromTouchInput()
	{
		Vector3 tempMousePosition = new Vector3(Input.mousePosition.x,Input.mousePosition.y,10f);
		return Camera.main.ScreenToWorldPoint(tempMousePosition);
	}

	float getXEndPosition(float xPosOnEndDrag)
	{
		if(xPosOnEndDrag >= 3.6f){
			return 0;
		}else if(xPosOnEndDrag <= -1 * (roomTotal * (roomWidth-1))){
			return (-1 * (roomWidth * (roomTotal-1)) );
		}else{
			float ratio = Mathf.Abs(xPosOnEndDrag) / roomWidth;
			float tenths = ratio - Mathf.Floor(ratio);

			int index = Mathf.FloorToInt(ratio);

			if(xOnBeginDrag < xPosOnEndDrag){
				if(tenths > 0.9f) index++;
			}else{
				if(tenths > 0.1f) index++;
			}

			return -1f * (index * roomWidth);
		}
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region coroutines
	IEnumerator SmoothSnap(Vector3 startPos, Vector3 endPos)
	{
		snapping = true;
		float t = 0;

		while(t <= 1){
			t += Time.deltaTime * snapSpeed;
			transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t)));
			yield return new WaitForSeconds(Time.deltaTime);
		}

		transform.position = endPos;
		snapping = false;
		yield return null;
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------
	#region public methods
	public void GoToRoom(RoomName destination)
	{
		if(currentRoom == destination) return;

		Vector3 startPos = transform.position;
		Vector3 endpos = new Vector3((roomWidth*(int)destination*-1f),0f,0f);
		StartCoroutine(SmoothSnap(startPos,endpos));
	}
	#endregion
//-------------------------------------------------------------------------------------------------------------------------------------------------

}