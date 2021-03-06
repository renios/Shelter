﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	private static GameManager gameManagerInstance = null;

	public GameObject MainCamera;
	public GameObject UICamera;
	public GameObject WhiteScreen;
	public Nutrient Nutrient;
	public Calender Calender;
	public ShelterStats ShelterStats;

	public CameraPosition cameraPosition = CameraPosition.Default;
	public const int cameraMoveTime = 1;
	public const int fadeoutTime = 2;

	Vector3 outPositionOfCamera = new Vector3 (0, 0, -10);
	Vector3 midPositionOfCamera = new Vector3 (-2.25f, -0.9f, -10);
	Vector3 inPositionOfCamera = new Vector3 (-2.59f, -0.9f, -10);

	const float outOrthographicSizeOfCamera = 3.6f;
	const float midOrthographicSizeOfCamera = 3.6f / 2;
	const float inOrthographicSizeOfCamera = 3.6f / 6;

	public enum CameraPosition
	{
		Out,
		Mid,
		In,
		Default
	};

	void Awake()
	{
		gameManagerInstance = this;
	}

	public static GameManager GetGameManagerInstance()
	{
		return gameManagerInstance;
	}

	public CameraPosition GetCameraPosition()
	{
		return cameraPosition;
	}

	// Use this for initialization
	void Start ()
	{
		InitializeCamera();
		StartCoroutine("FadeoutWhiteScreen");
	}

	public void EndTurn()
	{
		Nutrient.GainNutrientAtTurnEnd();
		Calender.PassDaysAtTurnEnd();
		ShelterStats.EatFoodAtTurnEnd();
	}

	void InitializeCamera()
	{
		cameraPosition = CameraPosition.Out;
		MainCamera.transform.position = new Vector3 (0, 0, -10);
		MainCamera.gameObject.camera.orthographicSize = outOrthographicSizeOfCamera;
		Debug.Log("Init status of camera");
	}

	IEnumerator FadeoutWhiteScreen()
	{
		this.gameObject.collider2D.enabled = false;
		UICamera.SetActive(false);
		WhiteScreen.SetActive(true);
		iTween.FadeTo(WhiteScreen, 0, fadeoutTime);
		yield return new WaitForSeconds(fadeoutTime);
		WhiteScreen.SetActive(false);
		UICamera.SetActive(true);
		WhiteScreen.SetActive(false);
		this.gameObject.collider2D.enabled = true;
	}

	IEnumerator MoveAniOfCamera(Vector3 position, float size)
	{
			isCameraMove = true;
			this.gameObject.collider2D.enabled = false;
			Debug.Log("Zoom from "+MainCamera.camera.orthographicSize+" to "+size);
			iTween.ValueTo(MainCamera.gameObject, iTween.Hash("from", MainCamera.camera.orthographicSize,
																"to", size,
															  "time", cameraMoveTime,
													"onupdatetarget", this.gameObject,
														  "onupdate", "updateOrthographicSizeOfCamera"));
			//Prevent appear background at call Move to 'Mid' View.
			if (size == 1.8f)
			{
				yield return new WaitForSeconds(cameraMoveTime);
			}
			iTween.MoveTo (MainCamera, position, cameraMoveTime);
			yield return new WaitForSeconds(cameraMoveTime);
			this.gameObject.collider2D.enabled = true;
			isCameraMove = false;
	}

	void updateOrthographicSizeOfCamera(float size)
	{
		MainCamera.camera.orthographicSize = size;
	}

	void MoveCamera (CameraPosition newCameraPosition)
	{
		cameraPosition = newCameraPosition;
		if (newCameraPosition == CameraPosition.Out)
		{
			StartCoroutine(MoveAniOfCamera(outPositionOfCamera, outOrthographicSizeOfCamera));
		}
		else if (cameraPosition == CameraPosition.Mid)
		{
			StartCoroutine(MoveAniOfCamera(midPositionOfCamera, midOrthographicSizeOfCamera));
		}
		else if (cameraPosition == CameraPosition.In)
		{
			StartCoroutine(MoveAniOfCamera(inPositionOfCamera, inOrthographicSizeOfCamera));
		}
		else
		{
			Debug.LogError("Invalid input to 'MoveCamera'");
		}
	}

	void ChangeOrthographicSizeOfCamera(float size)
	{
		MainCamera.gameObject.camera.orthographicSize = size;
	}

	bool isCameraMove = false;

	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") < 0 && isCameraMove == false)
		{
			Debug.Log("Push uparrow");
//			isCameraMove = true;
			if (cameraPosition == CameraPosition.Out)
			{
				cameraPosition = CameraPosition.Mid;
				MoveCamera(cameraPosition);
			}
			else if (cameraPosition == CameraPosition.Mid)
			{
				cameraPosition = CameraPosition.In;
				MoveCamera(cameraPosition);
			}
//			isCameraMove = false;
		}

		if (Input.GetAxis("Mouse ScrollWheel") > 0 && isCameraMove == false)
		{
			Debug.Log("Push downarrow");
//			isCameraMove = true;
			if (cameraPosition == CameraPosition.In)
			{
				cameraPosition = CameraPosition.Mid;
				MoveCamera(cameraPosition);
			}
			else if (cameraPosition == CameraPosition.Mid)
			{
				cameraPosition = CameraPosition.Out;
				MoveCamera(cameraPosition);
			}
//			isCameraMove = false;
		}
	}
}
