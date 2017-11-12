using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveAndLoadGame : MonoBehaviour
{
    public static SaveAndLoadGame saver;
    public string[] cityNames;
    public string[] cityStatuses;

    private static GameData saveData;
    private bool hasSavedGame;

    //use player information and store it in a singleton game object

    // Use this for initialization
    void Awake()
    {
        if(saver == null)
        {
            //might want to not destroy on load
            DontDestroyOnLoad(this.gameObject);
            saver = this;
            saveData = new GameData();
        }
        else if(saver != this)
        {
            //may want to destroy to avoid duplicates
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        SetCityNamesAndStatus();
    }

    private void OnApplicationQuit()
    {
        //SaveGame();
    }

    /// <summary>
    /// Function to check if there is a saved game already.
    /// </summary>
    public bool CheckForSaveGame()
    {
        if(hasSavedGame)
        {
            return true;
        }
        //otherwise do checking on files to set the value of has saved game
        return false;
    }

    /// <summary>
    /// Function to save game and the player's play session
    /// information.
    /// </summary>
    public void SaveGame()
    {
        BinaryFormatter binaryForm = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/ASF_" + DateTime.Today.Month + "_" + DateTime.Today.Day + "_" + DateTime.Today.Year + "_" + DateTime.Today.ToFileTime() + ".dat", FileMode.OpenOrCreate);

        binaryForm.Serialize(file, saveData);
        file.Close();
    }
    /// <summary>
    /// Function to save game and the player's play session
    /// information.
    /// </summary>
    /// <param name="fileToSave">String that says where to write this file.</param>
    public void SaveGame(string name)
    {
        if(name == "")
        {
            name = "unnamedSave_" + DateTime.Today.Second;
        }
        BinaryFormatter binaryForm = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/" + name + ".dat", FileMode.OpenOrCreate);

        binaryForm.Serialize(file, saveData);
        file.Close();
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
            saveData = (GameData)binaryForm.Deserialize(file);
            file.Close();
            hasSavedGame = true;
            //set values from data received
        }

    }

    /// <summary>
    /// Function to load game with the most recent save game file on record.
    /// </summary>
    public void ContinueGame()
    {
        //LoadGame();
        LoadLevel.loader.LoadALevel("CitySelectMap");
    }


    /// <summary>
    /// Function to start new game save information and 
    /// load into the character select scene.
    /// </summary>
    public void StartNewGame()
    {
        //start save game information here
        saveData = new GameData();
        LoadLevel.loader.LoadALevel("CitySelectMap");
    }

    /// <summary>
    /// Function to set character type.
    /// </summary>
    /// <param name="type">The type of the character (cube, sphere, octa).</param>
    public void SetCharType(string type)
    {
        saveData.characterType = type;
        for (int i = 0; i < cityNames.Length; i++)
        {
            if(cityNames[i].Contains(type))
            {
                SetCityStatus(cityNames[i], "ours");
            }
        }
    }

    /// <summary>
    /// Function to add the city names and statuses that are in the lists. 
    /// </summary>
    public void SetCityNamesAndStatus()
    {
        saveData.cityNames.Clear();
        saveData.cityStatuses.Clear();
        for (int i = 0; i < cityNames.Length; i++)
        {
            saveData.cityNames.Add(cityNames[i]);
            saveData.cityStatuses.Add(cityStatuses[i]);
        }
    }

    /// <summary>
    /// Function to set the status of a city given its name.
    /// </summary>
    /// <param name="cityName">The name of the city to change its status.</param>
    /// <param name="newStatus">The new status to the city.</param>
    /// <returns>Returns true if status set, false otherwise.</returns>
    public bool SetCityStatus(string cityName, string newStatus)
    {
        for (int i = 0; i < saveData.cityNames.Count; i++)
        {
            if(cityName == saveData.cityNames[i])
            {
                saveData.cityStatuses[i] = newStatus;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Go through the city statuses and check if all conquered.
    /// </summary>
    /// <returns>Returns true if all cities are conquered, else false.</returns>
    public bool CheckIfWonGame()
    {
        for (int i = 0; i < saveData.cityStatuses.Count; i++)
        {
            if (!saveData.cityNames[i].Contains(saveData.characterType) &&  saveData.cityStatuses[i] != "Conquered")
            {
                saveData.wonGame = false;
                return false;
            }
        }
        saveData.wonGame = true;
        return true;
    }

    [Serializable]
    private class GameData
    {   
        public string characterType = "Cube";
        public List<string> cityNames = new List<string>();
        public List<string> cityStatuses = new List<string>();
        public bool wonGame = false;
    }
}


