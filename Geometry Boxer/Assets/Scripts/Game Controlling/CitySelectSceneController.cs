﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySelectSceneController : MonoBehaviour
{
    private string currentCityBuildName;

    // Use this for initialization
    void Start()
    {
        currentCityBuildName = "MainMenu"; //default to main menu
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(SaveAndLoadGame.saver.CheckIfWonGame())
        {
            //open canvas saying the player won, let them go back to main menu
        }
    }


    /// <summary>
    /// Set variable that indicates selected city with build index.
    /// </summary>
    /// <param name="index"></param>
    public void SetCityBuildName(string name)
    {
        currentCityBuildName = name;
    }

    /// <summary>
    /// Return index of currently selected city's build index.
    /// </summary>
    /// <returns></returns>
    public string GetCurrentCityBuildName()
    {
        return currentCityBuildName;
    }
}
