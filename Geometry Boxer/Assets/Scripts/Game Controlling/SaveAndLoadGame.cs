using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoadGame : MonoBehaviour
{
    private LoadLevel loader;
    private bool hasSavedGame;

    // Use this for initialization
    void Start()
    {
        loader = this.GetComponent<LoadLevel>();

    }

    /// <summary>
    /// Function to check if there is a saved game already.
    /// </summary>
    public bool CheckForSaveGame()
    {
        return false;
    }

    /// <summary>
    /// Function to save game and the player's play session
    /// information.
    /// </summary>
    /// <returns>Returns true if success in saving, false otherwise.</returns>
    public bool SaveGame()
    {
        return true;
    }

    /// <summary>
    /// Function to load game state information.
    /// </summary>
    public void LoadGame()
    {

    }

    /// <summary>
    /// Function to load game with the most recent save game file on record.
    /// </summary>
    public void ContinueGame()
    {
        LoadGame();
    }


    /// <summary>
    /// Function to start new game save information and 
    /// load into the character select scene.
    /// </summary>
    public void StartNewGame()
    {
        //start save game information here
        
        loader.LoadALevel("CitySelectMap");
    }


}
