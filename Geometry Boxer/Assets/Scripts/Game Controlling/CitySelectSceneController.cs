using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySelectSceneController : MonoBehaviour
{
    public GameObject wonGameCanvas;

    private string currentCityBuildName;

    // Use this for initialization
    void Start()
    {
        currentCityBuildName = "MainMenu"; //default to main menu
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        wonGameCanvas.SetActive(SaveAndLoadGame.saver.CheckIfWonGame());
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

    /// <summary>
    /// Funciton for canvas button to quit game.
    /// </summary>
    public void QuitGameButton()
    {
        LoadLevel.loader.ExitGame();
    }

    /// <summary>
    /// Function for canvas button to go to main menu.
    /// </summary>
    public void GoToMainMenuButton()
    {
        LoadLevel.loader.LoadMainMenu();
    }
}
