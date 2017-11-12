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
        
        return GetAllSaveFiles().Length > 0;
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
        string[] files = GetAllSaveFiles();
        if(files.Length == 0)
        {
            return;
        }
        FileInfo mostRecent = new FileInfo(files[0]);
        int mostRecentIndex = 0;
        FileInfo current;
        for(int i =0; i<files.Length; i++)
        {
            current = new FileInfo(files[i]); 
            if(DateTime.Compare(current.LastWriteTime,mostRecent.LastWriteTime) > 0)
            {
                mostRecent = current;
                mostRecentIndex = i;
            }
        }
        LoadGame(files[mostRecentIndex].Substring(Application.persistentDataPath.Length + 1, files[mostRecentIndex].Length - Application.persistentDataPath.Length - 5));
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
        LoadGame();
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
        LoadLevel.loader.LoadALevel("CharacterSelect");
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

    /// <summary>
    /// Method to grab all files found on disk that were saved by this game.
    /// </summary>
    /// <returns>Returns files found that match app persistent data path and saved as .dat</returns>
    public string[] GetAllSaveFiles()
    {
        string[] filesFound = Directory.GetFiles(Application.persistentDataPath + "/", "*.dat", SearchOption.TopDirectoryOnly);
        return filesFound;
    }

    /// <summary>
    /// Method to grab all files found on disk that were saved by this game.
    /// </summary>
    /// <param name="searchPath">A string representing address to search for pattern on disk.</param>
    /// <returns>Returns files found that match given search path and saved as .dat</returns>
    public string[] GetAllSaveFiles(string searchPath)
    {
        string[] filesFound = Directory.GetFiles(searchPath, "*.dat", SearchOption.TopDirectoryOnly);
        return filesFound;
    }

    /// <summary>
    /// Method to grab all files found on disk that were saved by this game.
    /// </summary>
    /// <param name="searchPath">A string representing address to search for pattern on disk.</param>
    /// <param name="searchPattern">String to represent the pattern to search for.</param>
    /// <returns>Returns files found that match given search path and matches the search pattern</returns>
    public string[] GetAllSaveFiles(string searchPath, string searchPattern)
    {
        string[] filesFound = Directory.GetFiles(searchPath, searchPattern, SearchOption.TopDirectoryOnly);
        return filesFound;
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


