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
    public GameObject fileButtonPrefab;
    public GameObject scrollView;

    private bool hasSavedGame;
    private InputField loadFileInput;
    private string fileToLoad;
    private List<GameObject> loadFileButtons;

    // Use this for initialization
    void Start()
    {
        hasSavedGame = SaveAndLoadGame.saver.CheckForSaveGame();
        hasSaveGameCanvas.SetActive(hasSavedGame); //only one of the canvas elements will be active at once
        noSaveGameCanvas.SetActive(!hasSavedGame);
        loadFileInput = loadFileCanvas.GetComponentInChildren<InputField>();
        loadFileCanvas.SetActive(false);
        fileToLoad = "";
        loadFileButtons = new List<GameObject>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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

    /// <summary>
    /// Function to fill in buttons of scrollview of the found saved game files.
    /// </summary>
    private void FillInSaveFileInfo()
    {
        string[] files = SaveAndLoadGame.saver.GetAllSaveFiles();
        for(int i = 0; i<files.Length;i++)
        {
            files[i] = files[i].Substring(Application.persistentDataPath.Length+1, files[i].Length - Application.persistentDataPath.Length - 5);
            GameObject button = Instantiate(fileButtonPrefab) as GameObject;
            button.GetComponentInChildren<Text>().text = files[i];
            button.transform.SetParent(scrollView.transform,false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200f - (30f * i));
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetFileToLoadString(button.GetComponentInChildren<Text>().text); });
            loadFileButtons.Add(button);
        }
    }

    /// <summary>
    /// Function to hide the load file canvas and show the others.
    /// </summary>
    public void HideLoadCanvas()
    {
        loadFileButtons.Clear();
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
    /// Function to set string that represents file we want to load.
    /// </summary>
    /// <param name="buttonText">The text on the button that is the desired file name.</param>
    public void SetFileToLoadString(string buttonText)
    {
        loadFileInput.text = buttonText;
        SetFileToLoadString();
    }

    /// <summary>
    /// Load the data file matched to the string of the input field.
    /// </summary>
    public void LoadThisFile()
    {
        SaveAndLoadGame.saver.LoadGame(fileToLoad);
        loadFileInput.text = "";
        LoadLevel.loader.LoadALevel("CitySelectMap");
    }

    /// <summary>
    /// Helper function to continue most recent save game.
    /// </summary>
    public void ContinueGame()
    {
        SaveAndLoadGame.saver.ContinueGame();
    }

    /// <summary>
    /// Helper function to start a new save game.
    /// </summary>
    public void BeginNewGame()
    {
        SaveAndLoadGame.saver.StartNewGame();
    }

    /// <summary>
    /// Helper function to quit the game.
    /// </summary>
    public void QuitGame()
    {
        LoadLevel.loader.ExitGame();
    }
    /// <summary>
    /// Helper function to start tutorial.
    /// </summary>
    public void Tutorial()
    {
        SaveAndLoadGame.saver.SetCharType("Cube");
        LoadLevel.loader.LoadALevel("Tutorial");
    }

}
