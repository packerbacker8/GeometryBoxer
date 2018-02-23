using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class WinMenu : MonoBehaviour
{
    GameControllerScript gameController;
    private StandaloneInputModule gameEventSystemInputModule;
    private bool shouldAllowDPad = false;
    private bool tutorialScene;
    // Use this for initialization
    void Start()
    {
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
        {
            gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        }

        gameEventSystemInputModule = GameObject.FindGameObjectWithTag("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldAllowDPad)
        {
            if (Input.GetAxis("DPadY") != 0)
            {
                gameEventSystemInputModule.verticalAxis = "DPadY";
            }
            else
            {
                gameEventSystemInputModule.verticalAxis = "Vertical";
            }
        }
    }

    public void setButtonActive()
    {
        //for controller
        EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        shouldAllowDPad = true;
    }

    public void retryButton()
    {
        LoadLevel.loader.ReloadScene();
    }


    public void continueButton()
    {
        LoadLevel.loader.LoadALevel(gameController.dominationMap);
    }

    public void setMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }



}
