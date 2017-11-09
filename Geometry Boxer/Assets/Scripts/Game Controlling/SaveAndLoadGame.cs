using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveAndLoadGame : MonoBehaviour
{
    public static SaveAndLoadGame saver;
    private LoadLevel loader;
    private bool hasSavedGame;

    //use player information and store it in a singleton game object

    // Use this for initialization
    void Awake()
    {
        if(saver == null)
        {
            //might want to not destroy on load
            saver = this;
        }
        else if(saver != this)
        {
            //may want to destroy to avoid duplicates
        }
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
        BinaryFormatter binaryForm = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/saveGame_" + DateTime.Today + ".dat", FileMode.Open);

        GameData data = new GameData();

        binaryForm.Serialize(file, data);
        file.Close();
        return true;
    }
    /// <summary>
    /// Function to save game and the player's play session
    /// information.
    /// </summary>
    /// <param name="saveFileName">String that says where to write this file.</param>
    /// <returns>Returns true if success in saving, false otherwise.</returns>
    public bool SaveGame(string saveFileName)
    {
        BinaryFormatter binaryForm = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + saveFileName + ".dat", FileMode.OpenOrCreate);
        return true;
    }

    /// <summary>
    /// Function to load game state information.
    /// </summary>
    public void LoadGame()
    {

    }

    /// <summary>
    /// Function to load game state information.
    /// </summary>
    public void LoadGame(string loadFileName)
    {
        if (File.Exists(Application.persistentDataPath + "/" + loadFileName + ".dat" ))
        {
            BinaryFormatter binaryForm = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + loadFileName + ".dat", FileMode.Open);
            GameData data = (GameData)binaryForm.Deserialize(file);
            file.Close();

            //set values from data received
        }

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

    [Serializable]
    private class GameData
    {

    }
}


