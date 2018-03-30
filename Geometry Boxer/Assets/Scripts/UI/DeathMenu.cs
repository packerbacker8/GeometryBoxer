using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class DeathMenu : MonoBehaviour
{

    private string reloadLevelString;
    private bool shouldAllowDPad = false;
    private bool ps4Mode;
    private float TimeSinceScreen = 0.0f;
    private bool startTimer = false;
    private GameObject canvasPart;
    private StandaloneInputModule gameEventSystemInputModule;
    // Use this for initialization
    void Start()
    {
        gameEventSystemInputModule = GameObject.FindGameObjectWithTag("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
    }

    // Update is called once per frame
    void Update()
    {

        if (shouldAllowDPad)
        {
            string[] inputNames = Input.GetJoystickNames();
            if(inputNames.Length > 0)
            {
                if (inputNames[0].Length == 19)
                {
                    ps4Mode = true;
                }
                else
                {
                    ps4Mode = false;
                }
            }
            else
            {
                ps4Mode = false;
            }

            if (startTimer)
            {
                TimeSinceScreen += Time.deltaTime;
                if (TimeSinceScreen > 1.2f)
                {
                    EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                    startTimer = false;
                }
            }



            if (ps4Mode)
            {
                gameEventSystemInputModule.submitButton = "SubmitPS4";
            }
            else
            {
                gameEventSystemInputModule.submitButton = "Submit";
            }


            if (ps4Mode && Input.GetAxis("DPadYPS4") != 0)
            {
                gameEventSystemInputModule.verticalAxis = "DPadYPS4";
            }
            else if (Input.GetAxis("DPadY") != 0)
            {
                gameEventSystemInputModule.verticalAxis = "DPadY";
            }
            else
            {
                gameEventSystemInputModule.verticalAxis = "Vertical";
            }
        }
    }

    public void retryButton()
    {
        LoadLevel.loader.ReloadScene();
    }

    public void SetReloadString(string s)
    {
        reloadLevelString = s;
    }

    public void setButtonActive()
    {
        //for controller
        //EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        shouldAllowDPad = true;

        //check this frame if a controller is plugged in or not, and change modes accordingly.
        string[] inputNames = Input.GetJoystickNames();
        if(inputNames.Length > 0)
        {       //Length == 33 is Xbox One Controller... Length == 19 is PS4 Controller
            if (inputNames[0].Length == 33 || inputNames[0].Length == 19)
            {
                if (inputNames[0].Length == 19)
                {
                    ps4Mode = true;
                    gameEventSystemInputModule.submitButton = "SubmitPS4";
                }
                else
                {
                    ps4Mode = false;
                }
            }
        }

        startTimer = true;
    }

    public void setMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
