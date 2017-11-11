using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
{

    public GameObject character;

    private GameObject pauseMenuCanvas;
    private GameObject saveCanvas;
    RootMotion.Demos.UserControlMelee UserControlMeleeScript;
    RootMotion.CameraController CameraControllerScript;
    PunchScript punchScript;


    private bool mouseShouldBeLocked = false;
    private bool isPaused = false;
    private float TimeSinceEsc = 0.0f;
    // Use this for initialization
    void Start()
    {
        pauseMenuCanvas = this.transform.GetChild(0).gameObject;
        saveCanvas = this.transform.GetChild(1).gameObject;
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        UserControlMeleeScript = character.GetComponentInChildren<RootMotion.Demos.UserControlMelee>();
        CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
        punchScript = character.gameObject.GetComponent<PunchScript>();
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceEsc = TimeSinceEsc += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            pauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            resumeGame();
            isPaused = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void resumeGame()
    {
        Time.timeScale = 1.0f;
        //UserControlMeleeScript.inPause = false;
        punchScript.enabled = true;
        CameraControllerScript.enabled = true;

        if (mouseShouldBeLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        Cursor.visible = false;

        pauseMenuCanvas.SetActive(false);

    }

    /// <summary>
    /// 
    /// </summary>
    public void pauseGame()
    {
        Time.timeScale = 0.0f;

        Cursor.visible = true;
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            mouseShouldBeLocked = true;
        }
        //UserControlMeleeScript.inPause = true;
        punchScript.enabled = false;
        CameraControllerScript.enabled = false;

        pauseMenuCanvas.SetActive(true);
        isPaused = true;
    }

    /// <summary>
    /// Function to open the save canvas.
    /// </summary>
    public void OpenSaveCanvas()
    {
        saveCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);
    }

    /// <summary>
    /// Function to close the save canvas.
    /// </summary>
    public void CloseSaveCanvas()
    {
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);
    }


}
