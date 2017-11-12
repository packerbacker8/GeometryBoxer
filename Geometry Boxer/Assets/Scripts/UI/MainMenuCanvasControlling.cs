using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCanvasControlling : MonoBehaviour
{
    public GameObject hasSaveGameCanvas;
    public GameObject noSaveGameCanvas;
    public GameObject optionsMenu;
    public GameObject loadFileCanvas;

    private bool hasSavedGame;
    private InputField loadFileInput;
    private string fileToLoad;

    // Use this for initialization
    void Start()
    {
        hasSavedGame = SaveAndLoadGame.saver.CheckForSaveGame();
        hasSaveGameCanvas.SetActive(hasSavedGame); //only one of the canvas elements will be active at once
        noSaveGameCanvas.SetActive(!hasSavedGame);
        loadFileInput = loadFileCanvas.GetComponentInChildren<InputField>();
        loadFileCanvas.SetActive(false);
        fileToLoad = "";
    }

    /// <summary>
    /// Function to show or hide options for the game.
    /// </summary>
    /// <param name="show">If true, options menu will be shown, hidden if false.</param>
    public void ShowOrHideOptions(bool show)
    {
        optionsMenu.SetActive(show);
        if (show)
        {
            hasSaveGameCanvas.SetActive(false);
            noSaveGameCanvas.SetActive(false);
        }
        else
        {
            hasSaveGameCanvas.SetActive(hasSavedGame);
            noSaveGameCanvas.SetActive(!hasSavedGame);
        }
    }

    /// <summary>
    /// Function to show the load file canvas.
    /// </summary>
    public void ShowLoadCanvas()
    {
        loadFileCanvas.SetActive(true);
        FillInSaveFileInfo();
        hasSaveGameCanvas.SetActive(false);
        noSaveGameCanvas.SetActive(false);
    }

    private void FillInSaveFileInfo()
    {
        string[] files = SaveAndLoadGame.saver.GetAllSaveFiles();
        foreach(string file in files)
        {
            //make prefab button and add to scrollview
            //Debug.Log(Application.persistentDataPath.Length);
            //Debug.Log(file.Length);
            Debug.Log(file.Substring(Application.persistentDataPath.Length+1, file.Length - Application.persistentDataPath.Length - 5));
        }
    }

    /// <summary>
    /// Function to hide the load file canvas and show the others.
    /// </summary>
    public void HideLoadCanvas()
    {
        loadFileCanvas.SetActive(false);
        hasSavedGame = SaveAndLoadGame.saver.CheckForSaveGame();
        hasSaveGameCanvas.SetActive(hasSavedGame);
        noSaveGameCanvas.SetActive(!hasSavedGame);
    }
    /// <summary>
    /// Function to set string that represents file we want to load.
    /// </summary>
    public void SetFileToLoadString()
    {
        fileToLoad = loadFileInput.text;
    }

    /// <summary>
    /// Load the data file matched to the string of the input field.
    /// </summary>
    public void LoadThisFile()
    {
        loadFileInput.text = "";
        SaveAndLoadGame.saver.LoadGame(fileToLoad);
        LoadLevel.loader.LoadALevel("SelectCityMap");
    }
}
