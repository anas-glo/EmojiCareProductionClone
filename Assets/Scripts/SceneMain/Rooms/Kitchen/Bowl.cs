﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowl : MonoBehaviour {
	[Header("Reference")]
	public GameObject[] prefabIngredients;
	public Transform content;
	public Transform roomTransform;
	[Header("Dynamic variables")]
	public List<GameObject> instantiatedIngredients = new List<GameObject>();
	public bool initialized = false;

	Vector3 startPos = new Vector3(0f,-5.5f, -1f);
	Vector3 startScale = new Vector3(2f,2f,2f);
	Vector3 flipPos = new Vector3(0f,1.5f,-1f);
	Vector3 flipRotation = new Vector3(0f,0f,-100f);

	public void  Init(GameObject[] ingredients)
	{
		transform.position = startPos;
		transform.eulerAngles = Vector3.zero;
		transform.localScale = startScale;

		InstantiateObjects(ingredients);

		this.gameObject.SetActive(true);
		StartCoroutine(_Shrink);
	}

	void InstantiateObjects(GameObject[] objects)
	{
		for(int i = 0;i<objects.Length;i++){
			int index = (int)objects[i].GetComponent<IngredientObject>().type;
			GameObject temp = (GameObject) Instantiate(prefabIngredients[index],content);
			temp.transform.localPosition = new Vector3(0f,0.3f,-1f);
			foreach(GameObject g in instantiatedIngredients){
				Physics2D.IgnoreCollision(g.GetComponent<Ingredient>().thisCollider,temp.GetComponent<Ingredient>().thisCollider,true);
			}
			instantiatedIngredients.Add(temp);
		}
	}

	void LateUpdate()
	{
		if(initialized){
			Vector3 tempMousePosition = new Vector3(Input.mousePosition.x,Input.mousePosition.y,19f);
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(tempMousePosition);
			transform.position = new Vector3(mouseWorldPos.x,mouseWorldPos.y+0.3f,mouseWorldPos.z);

			if(Input.GetMouseButtonUp(0)){
				initialized = false;
				if(instantiatedIngredients.Count <= 0){
					gameObject.SetActive(false);
				}else{
					StartCoroutine(_MoveToTableAndFlip);
				}
			}
		}
	}

	const string _Shrink = "Shrink";
	IEnumerator Shrink()
	{
		float t = 0;
		Vector3 currentScale = transform.localScale;
		while (t < 1){
			Vector3 tempMousePosition = new Vector3(Input.mousePosition.x,Input.mousePosition.y,19f);
			Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(tempMousePosition);
			transform.position = Vector3.Lerp(transform.position,mouseWorldPos,t);

			transform.localScale = Vector3.Lerp(currentScale,Vector3.one,t);
			t+= Time.deltaTime * 5;
			yield return null;
		}
		initialized = true;
	}

	const string _MoveToTableAndFlip = "MoveToTableAndFlip";
	IEnumerator MoveToTableAndFlip()
	{
		float t = 0;
		Vector3 currentPos = transform.position;
		Vector3 angle = transform.eulerAngles;
		while(t < 1){
			transform.position = Vector3.Lerp(currentPos,flipPos,t);
			t+= Time.deltaTime * 5;
			yield return null;
		}
		transform.position = flipPos;
		t = 0;
		while(t< 1){
			transform.eulerAngles = Vector3.Lerp(angle,flipRotation,t);
			t+= Time.deltaTime * 2;
			yield return null;
		}
		transform.eulerAngles = flipRotation;

		foreach(GameObject g in instantiatedIngredients){
			g.transform.SetParent(roomTransform,true);

			g.transform.position = new Vector3(g.transform.position.x,g.transform.position.y,-1f);
			g.transform.eulerAngles = Vector3.zero;
			g.transform.localScale = Vector3.one;

			Ingredient i = g.GetComponent<Ingredient>();
			i.thisCollider.enabled = true;
			i.thisRigidbody.simulated = true;
		}

		yield return new WaitForSeconds(1f);
		instantiatedIngredients.Clear();
		gameObject.SetActive(false);
	}
}
