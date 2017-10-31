using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour {

	private GameObject pauseMenuCanvas;
	public GameObject character;

	RootMotion.Demos.UserControlMelee UserControlMeleeScript;
	RootMotion.CameraController CameraControllerScript;
	PunchScript punchScript;


	private bool mouseShouldBeLocked = false;
	private bool isPaused = false;
	private float TimeSinceEsc = 0.0f;
	// Use this for initialization
	void Start () {
		pauseMenuCanvas = this.transform.GetChild (0).gameObject;
		pauseMenuCanvas.SetActive (false);
		UserControlMeleeScript = character.GetComponentInChildren<RootMotion.Demos.UserControlMelee> ();
		CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController> ();
		punchScript = character.gameObject.GetComponent<PunchScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		TimeSinceEsc = TimeSinceEsc += Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.Escape) && !isPaused) {
			Time.timeScale = 0.0f;

			Cursor.visible = true;
			if (Cursor.lockState == CursorLockMode.Locked) {
				Cursor.lockState = CursorLockMode.None;
				mouseShouldBeLocked = true;
			}
			//UserControlMeleeScript.inPause = true;
			punchScript.enabled = false;
			CameraControllerScript.enabled = false;

			pauseMenuCanvas.SetActive (true);
			isPaused = true;

		}

		else if (Input.GetKeyDown(KeyCode.Escape) && isPaused) {
			resumeGame ();
			isPaused = false;
		}
			

	}
		

	public void resumeGame()
	{
		Time.timeScale = 1.0f;
		//UserControlMeleeScript.inPause = false;
		punchScript.enabled = true;
		CameraControllerScript.enabled = true;

		if (mouseShouldBeLocked) {
			Cursor.lockState = CursorLockMode.Locked;
		}
		Cursor.visible = false;

		pauseMenuCanvas.SetActive (false);

	}


}
