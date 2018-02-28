using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class WinMenu : MonoBehaviour
{
    GameControllerScript gameController;
    private StandaloneInputModule gameEventSystemInputModule;
    private bool shouldAllowDPad = false;
    private bool ps4Mode = false;
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
            string[] inputNames = Input.GetJoystickNames();
            for (int i = 0; i < inputNames.Length; i++)
            {       //Length == 33 is Xbox One Controller... Length == 19 is PS4 Controller
                if (inputNames[i].Length == 33 || inputNames[i].Length == 19)
                {
                    if (inputNames[i].Length == 19)
                    {
                        ps4Mode = true;
                    }
                    else
                    {
                        ps4Mode = false;
                    }
                }
            }

            if(ps4Mode)
            {
                gameEventSystemInputModule.submitButton = "SubmitPS4";
            }
            else
            {
                gameEventSystemInputModule.submitButton = "Submit";
            }


            if (Input.GetAxis("DPadY") != 0)
            {
                gameEventSystemInputModule.verticalAxis = "DPadY";
            }
            else if (Input.GetAxis("DPadYPS4") != 0)
            {
                gameEventSystemInputModule.verticalAxis = "DPadYPS4";
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
