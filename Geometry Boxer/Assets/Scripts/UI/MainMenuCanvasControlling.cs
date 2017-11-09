using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCanvasControlling : MonoBehaviour
{
    public GameObject hasSaveGameCanvas;
    public GameObject noSaveGameCanvas;
    public GameObject optionsMenu;

    private SaveAndLoadGame saver;
    private bool hasSavedGame;

    // Use this for initialization
    void Start()
    {
        saver = this.GetComponent<SaveAndLoadGame>();
        hasSavedGame = saver.CheckForSaveGame();
        hasSaveGameCanvas.SetActive(hasSavedGame); //only one of the canvas elements will be active at once
        noSaveGameCanvas.SetActive(!hasSavedGame);
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
}
