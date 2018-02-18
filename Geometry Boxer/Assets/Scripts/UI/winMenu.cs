using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class winMenu : MonoBehaviour
{
    GameControllerScript gameController;
    private StandaloneInputModule gameEventSystemInputModule;
    private bool shouldAllowDPad = false;
    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
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
        LoadLevel.loader.LoadALevel(gameController.deathReloadMap);
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
