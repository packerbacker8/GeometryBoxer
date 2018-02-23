using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class DeathMenu : MonoBehaviour
{

    private string reloadLevelString;
    private bool shouldAllowDPad = false;
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

    public void retryButton()
    {
        LoadLevel.loader.LoadALevel(reloadLevelString);
    }

    public void SetReloadString(string s)
    {
        reloadLevelString = s;
    }

    public void setButtonActive()
    {
        //for controller
        EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        shouldAllowDPad = true;
    }

    public void setMouse()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
