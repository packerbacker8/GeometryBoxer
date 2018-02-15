using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class winMenu : MonoBehaviour
{
    GameControllerScript gameController;
    // Use this for initialization
    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setButtonActive()
    {
        //for controller
        EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
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
