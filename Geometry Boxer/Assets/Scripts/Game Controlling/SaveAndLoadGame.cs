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
    [Tooltip("If set to true, the main menu scene will not be forced.")]
    public bool debugMode = false;

    private static InitializeData initData;
    private static GameData saveData;
    private bool hasSavedGame;
    private bool forceTutorial;
    private string currentSavePath;

    //use player information and store it in a singleton game object

    // Use this for initialization
    void Awake()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/SetUp/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/SetUp/");
        }
        string[] firstSaveFound = Directory.GetFiles(Application.persistentDataPath + "/SetUp/", "Initialize.dat", SearchOption.TopDirectoryOnly);
        
        forceTutorial = !(firstSaveFound.Length > 0);
        
        if(forceTutorial)
        {
            initData = new InitializeData(DateTime.Today);
            BinaryFormatter binaryForm = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SetUp/Initialize.dat", FileMode.OpenOrCreate);

            binaryForm.Serialize(file, initData);
            file.Close();
        }
        else
        {
            if (File.Exists(Application.persistentDataPath + "/SetUp/Initialize.dat"))
            {
                BinaryFormatter binaryForm = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/SetUp/Initialize.dat", FileMode.Open);
                initData = (InitializeData)binaryForm.Deserialize(file);
                file.Close();
            }
            else
            {
                foreach(string filePath in firstSaveFound)
                {
                    Debug.Log("Oops, actually at: " + filePath);
                }
            }
        }

        if (saver == null)
        {
            //might want to not destroy on load
            DontDestroyOnLoad(this.gameObject);
            saver = this;
            saveData = new GameData();
            currentSavePath = "";
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
        if(!debugMode)
        {
            if (forceTutorial)
            {
                forceTutorial = false;
                saveData = new GameData { characterType = "Cube" };
                LoadLevel.loader.LoadALevel("Tutorial");
            }
            else
            {
                saveData = new GameData { characterType = "Cube" };
                LoadLevel.loader.LoadMainMenu();
            }
        }
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
    /// Function to quickly save the player's game.
    /// </summary>
    public void QuickSave()
    {
        SaveGame(currentSavePath);
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
        currentSavePath = name;
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
        currentSavePath = loadFileName;
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
        SetLoadedFightScene(true);
        LoadLevel.loader.LoadALevel(saveData.sceneCurrentlyOn);
    }


    /// <summary>
    /// Function to start new game save information and 
    /// load into the character select scene.
    /// </summary>
    public void StartNewGame()
    {
        //start save game information here
        saveData = new GameData();
        SetCityNamesAndStatus();
        LoadLevel.loader.LoadALevel("CharacterSelect");
    }

    /// <summary>
    /// Function to set character type.
    /// </summary>
    /// <param name="type">The type of the character (cube, sphere, octa).</param>
    public void SetCharType(string type)
    {
        saveData.characterType = type;
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
        saveData.gameStatus = "Started";
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
    /// Function to get status of the requested city.
    /// </summary>
    /// <param name="cityToGet">Name of the city of which status you want.</param>
    /// <returns>Return the string of the status of the requested city.</returns>
    public string GetCityStatus(string cityToGet)
    {
        for (int i = 0; i < saveData.cityNames.Count; i++)
        {
            if (cityToGet == saveData.cityNames[i])
            {
                return saveData.cityStatuses[i];
            }
        }
        return "unknown";
    }

    /// <summary>
    /// Function to return the status of each city as currently saved.
    /// </summary>
    /// <returns>List of strings of all city statuses.</returns>
    public List<string> GetCityStatuses()
    {
        return saveData.cityStatuses;
    }

    /// <summary>
    /// Function to get the city name of the type that is passed in.
    /// </summary>
    /// <param name="typeToFind">What type to search for (Cube,Octahedron,Sphere).</param>
    /// <returns></returns>
    public string GetCityName(string typeToFind)
    {
        foreach(string city in cityNames)
        {
            if(city.Contains(typeToFind))
            {
                return city;
            }
        }
        return "nothing";
    }

    /// <summary>
    /// Go through the city statuses and check if all conquered.
    /// </summary>
    /// <returns>Returns true if all cities are conquered, else false.</returns>
    public bool CheckIfWonGame()
    {
        for (int i = 0; i < saveData.cityStatuses.Count; i++)
        {
            if (!saveData.cityNames[i].Contains(saveData.characterType) &&  saveData.cityStatuses[i] != "conquered")
            {
                saveData.wonGame = false;
                saveData.gameStatus = "In Progress";
                return false;
            }
        }
        saveData.wonGame = true;
        saveData.gameStatus = "Victory";
        SaveGame(currentSavePath);
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

    /// <summary>
    /// Function to obtain the saved player's character type.
    /// </summary>
    /// <returns>Returns the string which represents the player's type (Cube, Sphere, Octahedron).</returns>
    public string GetCharacterType()
    {
        return saveData.characterType;
    }

    /// <summary>
    /// Function to obtain the saved game's status.
    /// </summary>
    /// <returns>Returns status of current save data with Character Select, In Progress, or Victory.</returns>
    public string GetGameStatus()
    {
        return saveData.gameStatus;
    }


    /// <summary>
    /// Function to set the current save's game status.
    /// </summary>
    /// <param name="status">Set the saved game's status to some string that indicates status.</param>
    public void SetGameStatus(string status)
    {
        saveData.gameStatus = status;
    }

    /// <summary>
    /// Function to set values in save data of fighting information.
    /// Should be called before each save. On non fighting scenes these values can be
    /// defaulted to empty new list, false, and empty new list respectively.
    /// </summary>
    /// <param name="eIndicies">Indicies of enemies still alive. Destroy others.</param>
    /// <param name="allies">Does the scene have allies? Yes if bool is true.</param>
    /// <param name="aIndicies">Indicies of allies still alive, others destroyed.</param>
    public void SetFightSceneSaveValues(HashSet<int> eIndicies, bool allies, HashSet<int> aIndicies)
    {
        saveData.enemyIndicies = eIndicies;
        saveData.currentSceneHasAllies = allies;
        saveData.allyIndicies = aIndicies;
    }

    /// <summary>
    /// Function to grab current living enemies in scene.
    /// </summary>
    /// <returns>Returns the index of enemies in current scene, which should be living and which should not.</returns>
    public HashSet<int> GetFightSceneEnemyIndicies()
    {
        return saveData.enemyIndicies;
    }

    /// <summary>
    /// Function to grab current living allies in scene.
    /// </summary>
    /// <returns>Returns the index of allies in current scene, which should be living and which should not.</returns>
    public HashSet<int> GetFightSceneAllyIndicies()
    {
        return saveData.allyIndicies;
    }

    /// <summary>
    /// Function to find out if scene has allies or not.
    /// </summary>
    /// <returns>True if allies present, false otherwise.</returns>
    public bool GetFightSceneHasAllies()
    {
        return saveData.currentSceneHasAllies;
    }

    /// <summary>
    /// Function to save which scene the player is on.
    /// </summary>
    /// <param name="scene">String name needs to be scene in build order to work.</param>
    public void SetCurrentScene(string scene)
    {
        saveData.sceneCurrentlyOn = scene;
    }

    /// <summary>
    /// Function to get which scene was saved to this save data.
    /// </summary>
    /// <returns>Returns string name saved as the current scene.</returns>
    public string GetSceneNameCurrentlyOn()
    {
        return saveData.sceneCurrentlyOn;
    }

    /// <summary>
    /// Set whether or not this save data is loading into a fight scene.
    /// Important only if checking the game controller to destroy enemies or not.
    /// </summary>
    /// <param name="loaded">If true, the game controller will check which enemies to destroy on load.</param>
    public void SetLoadedFightScene(bool loaded)
    {
        saveData.loadedThisFightScene = loaded;
    }

    /// <summary>
    /// Function to return if this scene was loaded into by a save file
    /// or if it was gotten to by the normal game flow.
    /// </summary>
    /// <returns>If true, this game came from a loaded file.</returns>
    public bool GetLoadedFightScene()
    {
        return saveData.loadedThisFightScene;
    }

    /// <summary>
    /// Set what the player's health will be when loaded back in.
    /// </summary>
    /// <param name="amount">Set the players health to this amount.</param>
    public void SetPlayerCurrentHealth(float amount)
    {
        saveData.playerCurrentHealth = amount;
    }

    /// <summary>
    /// Get what the player's health was at the time of saving.
    /// </summary>
    /// <returns>Returns what player's health was.</returns>
    public float GetPlayerCurrentHealth()
    {
        return saveData.playerCurrentHealth;
    }

    [Serializable]
    private class InitializeData
    {
        public bool tutorialCompleted = false;
        public DateTime dateLaunched;
        public InitializeData(DateTime today)
        {
            dateLaunched = today;
        }
    }

    [Serializable]
    private class GameData
    {
        public string gameStatus = "Character Select";
        public string sceneCurrentlyOn = "MainMenu";
        public string characterType = "Octahedron";
        public float playerCurrentHealth = 15000f;
        public List<string> cityNames = new List<string>();
        public List<string> cityStatuses = new List<string>();
        public bool wonGame = false;
        public HashSet<int> enemyIndicies = new HashSet<int>();
        public bool currentSceneHasAllies = false;
        public HashSet<int> allyIndicies = new HashSet<int>();
        public bool loadedThisFightScene = false;
        //public int enemiesLeftInScene = -1; can calculate this number based on number of indicies passed
        //public int alliesLeftInScene = -1;
    }
}


