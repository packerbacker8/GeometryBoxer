using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using RootMotion.Demos;

public class MainMenuCanvasControlling : MonoBehaviour
{
    //Toggles for Survival Canvas
    public Text survivalCurrentMapSelection;
    public Dropdown survivalFactionDropdown;
    public Dropdown survivalModeDropdown;
    public GameObject survivalBackButton;
    public GameObject survivalStartButton;

    public GameObject survivalCanvas;
    public GameObject CampaignCanvas;
    public GameObject StartMenuCanvas;
    public GameObject creditsCanvas;
    public GameObject loadFileCanvas;

    public UnityEngine.UI.Button continueButton;
    public UnityEngine.UI.Button loadGameButton;
    public UnityEngine.UI.Button coopButton;

    public GameObject fileButtonPrefab;
    public GameObject scrollView;
    public GameObject scrollViewContent;
    public GameObject playerUI;
    
    [Header("Names of UI elements that need to be found.")]
    public string tutorialButtonName = "TextButtonTutorial";
    public string loadFileButtonName = "LoadFileButton";
    public string loadInputFieldName = "LoadInputField";
    string[] inputNames;

    
    private InputField loadFileInput;
    private string fileToLoad;
    private string survivalCurrentArenaSelection;
    private List<GameObject> loadFileButtons;

    private bool controllerMode;
    private bool ps4Mode = false;
    private bool menuActive = false;
    private bool allowNavigation = false;
    private bool inInputField;
    private bool loadCanvasEnabled = false;
    private float timeSinceDPAD = 0.0f;
    private StandaloneInputModule EventSystemInputModule;

    // Use this for initialization
    void Start()
    {
        SaveAndLoadGame.saver.SetWaveOn(-1);
        checkCoop();
        checkSaves();
        //hasSaveGameCanvas.SetActive(hasSavedGame); //only one of the canvas elements will be active at once
        StartMenuCanvas.SetActive(true);
        CampaignCanvas.SetActive(false);
       
        //All things related to Survival Canvas
        survivalCanvas.SetActive(false);
        survivalCurrentMapSelection.text = "Arena0";
        survivalCurrentArenaSelection = "Arena0";

        loadFileInput = loadFileCanvas.GetComponentInChildren<InputField>();
        loadFileCanvas.SetActive(false);
        fileToLoad = "";
        loadFileButtons = new List<GameObject>();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        controllerMode = false;
        inInputField = false;
        CheckControllerModeAndType();

        EventSystemInputModule = GameObject.FindGameObjectWithTag("EventSystem").gameObject.GetComponent<StandaloneInputModule>();

        //if (controllerMode)
        //{
        //disablePlayerForController();     
        //EventSystemInputModule.verticalAxis = "DPadY";
        //}

        for (int i = 0; i < playerUI.transform.childCount; i++)
        {
            playerUI.transform.GetChild(i).gameObject.SetActive(false);
        }

        if (controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(GameObject.Find(tutorialButtonName));
            menuActive = true;
        }
        
        
    }

    void checkCoop()
    {
        if (Input.GetJoystickNames().Length > 1)
        { 
            coopButton.interactable = true;
            coopButton.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            coopButton.interactable = false;
            coopButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    void checkSaves()
    {
        if (SaveAndLoadGame.saver.CheckForSaveGame())
        {
            continueButton.interactable = true;
            continueButton.GetComponentInChildren<Text>().color = Color.white;
            loadGameButton.interactable = true;
            loadGameButton.GetComponentInChildren<Text>().color = Color.white;
        }
        else
        {
            continueButton.interactable = false;
            continueButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            loadGameButton.interactable = false;
            loadGameButton.GetComponentInChildren<Text>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }

    }
    void disablePlayerForController()
    {
        //if (StartMenuCanvas.activeSelf)
        //{
        //    //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
        //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        //}
        //else if (StartMenuCanvas.activeSelf)
        //{
        //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        //}

        EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        GameObject CharacterController = null;
        GameObject[] listOfPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listOfPlayerObjects.Length; i++)
        {
            if (listOfPlayerObjects[i].name == "Character Controller")
            {
                CharacterController = listOfPlayerObjects[i];
                break;
            }
        }

        //disable UserControlMelee on CubeMan
        CharacterController.GetComponentInChildren<UserControlMelee>().enabled = false;
        //mouseMode = false;
    }

    void enablePlayerForMouse()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        GameObject CharacterController = null;
        GameObject[] listOfPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listOfPlayerObjects.Length; i++)
        {
            if (listOfPlayerObjects[i].name == "Character Controller")
            {
                CharacterController = listOfPlayerObjects[i];
                break;
            }
        }

        //enable UserControlMelee on CubeMan
        CharacterController.GetComponentInChildren<UserControlMelee>().enabled = true;
        //mouseMode = true;
    }

    void Update()
    {
        CheckControllerModeAndType();
        checkCoop();
        if (controllerMode)
        {
            if (ps4Mode)
            {
                EventSystemInputModule.submitButton = "SubmitPS4";
            }
            else
            {
                EventSystemInputModule.submitButton = "Submit";
            }

            //Debug.Log(timeSinceDPAD);
            if (menuActive && timeSinceDPAD < 0.1f)
            {
                timeSinceDPAD += Time.deltaTime;
            }
            if (!allowNavigation && timeSinceDPAD >= 0.1f)
            {
                EventSystem.current.sendNavigationEvents = true;
                allowNavigation = true;
            }

            if (Input.GetAxis("DPadY") != 0 || Input.GetAxis("DPadYPS4") != 0)
            {
                if (ps4Mode)
                {
                    EventSystemInputModule.verticalAxis = "DPadYPS4";
                }
                else
                {
                    EventSystemInputModule.verticalAxis = "DPadY";
                }

                //set canvas active for input manager if not active
                if (!menuActive)
                {

                    //if (hasSaveGameCanvas.activeInHierarchy)
                    //{
                    //    //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
                    //    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                    //}
                    //else if (StartMenuCanvas.activeInHierarchy)
                    //{
                    //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                    //}

                    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);

                    menuActive = true;
                    allowNavigation = false;
                    timeSinceDPAD = 0.0f;
                    EventSystem.current.sendNavigationEvents = false;
                    disablePlayerForController();
                }

            }
            else if (Input.GetAxis("VerticalLeft") != 0)
            {

                EventSystemInputModule.verticalAxis = "Vertical";
                if (menuActive)
                {
                    menuActive = false;
                    //if player is in a submenu, exit that submenu.

                    //exit any options menu
                    creditsCanvas.SetActive(false);

                    //exit any save menu
                    foreach (GameObject butt in loadFileButtons)
                    {
                        butt.transform.parent = null;
                        Destroy(butt);
                    }
                    loadFileButtons.Clear();
                    loadFileCanvas.SetActive(false);

                    CampaignCanvas.SetActive(false);

                    //exit any survival menu
                    if (survivalCanvas.activeSelf)
                    {
                        survivalCanvas.SetActive(false);
                    }

                    StartMenuCanvas.SetActive(true);
                    enablePlayerForMouse();

                }
                EventSystem.current.SetSelectedGameObject(null);
            }
            if(Input.GetAxis("DPadY") == 0 && Input.GetAxis("DPadYPS4") == 0 && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.name == loadInputFieldName)
            {
                inInputField = true;
            }
            //EventSystemInputModule.verticalAxis = "DPadY";
        }
        else
        {
            enablePlayerForMouse();
            EventSystemInputModule.verticalAxis = "Vertical";
        }

        if (loadFileCanvas.activeInHierarchy && controllerMode && inInputField && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.name == loadInputFieldName && (Input.GetAxis("DPadY") != 0 || Input.GetAxis("DPadYPS4") != 0))
        {
            EventSystem.current.SetSelectedGameObject(loadFileCanvas.transform.Find(loadFileButtonName).gameObject);
            inInputField = false;
        }

        //Cases for Survival Canvas
        if (survivalCanvas.activeSelf && controllerMode)
        {
            GameObject scrollViewGameObj = survivalCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject;
            GameObject curObj = EventSystem.current.currentSelectedGameObject;

            //We are on scrollview hitting right, so move to faction dropdown
            if (Input.GetAxis("DPadX") == 1 && curObj == scrollViewGameObj)
            {
                EventSystem.current.SetSelectedGameObject(survivalFactionDropdown.gameObject);
            }
            //We are on scrollView hitting left, so move to back button
            else if (Input.GetAxis("DPadX") == -1 && curObj == scrollViewGameObj)
            {
                EventSystem.current.SetSelectedGameObject(survivalBackButton.gameObject);
            }
            //We are on either dropdown and hit left, move us to scrollview
            else if ((curObj.Equals(survivalFactionDropdown.gameObject) || curObj.Equals(survivalModeDropdown.gameObject)) && (Input.GetAxis("DPadX") == -1))
            {
                EventSystem.current.SetSelectedGameObject(scrollViewGameObj);
            }
        }
    }

    public void showCampaign()
    {
        CampaignCanvas.SetActive(true);
        StartMenuCanvas.SetActive(false);
        checkSaves();
        if (controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(CampaignCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);

        }
    }

    public void hideCampaign()
    {
        StartMenuCanvas.SetActive(true);
        CampaignCanvas.SetActive(false);

        if (controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);

            //if (hasSaveGameCanvas.activeSelf)
            //{
            //    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
            //else if (StartMenuCanvas.activeSelf)
            //{
            //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
        }
    }
    /// <summary>
    /// Function to show or hide options for the game.
    /// </summary>
    /// <param name="show">If true, options menu will be shown, hidden if false.</param>
    public void ShowOrHideOptions(bool show)
    {
        creditsCanvas.SetActive(show);
        if (show)
        {
            
            StartMenuCanvas.SetActive(false);
            CampaignCanvas.SetActive(false);
            survivalCanvas.SetActive(false);

            if (controllerMode)
            {
                EventSystem.current.SetSelectedGameObject(creditsCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            }

        }
        else
        {
            //hasSaveGameCanvas.SetActive(hasSavedGame);
            //StartMenuCanvas.SetActive(!hasSavedGame);
            StartMenuCanvas.SetActive(true);
            if (controllerMode)
            {         
                EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                
                //if (hasSaveGameCanvas.activeSelf)
                //{
                //    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                //}
                //else if (StartMenuCanvas.activeSelf)
                //{
                //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                //}
            }
        }
    }

    /// <summary>
    /// Function to show the survival mode canvas.
    /// </summary>
    public void ShowSurvivalModeCanvas()
    {
        survivalCanvas.SetActive(true);
        loadFileCanvas.SetActive(false);
        CampaignCanvas.SetActive(false);
        StartMenuCanvas.SetActive(false);
        creditsCanvas.SetActive(false);

        EventSystem.current.SetSelectedGameObject(survivalCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        ArenaMapSelection();
    }


    /// <summary>
    /// Function to show the load file canvas.
    /// </summary>
    public void ShowLoadCanvas()
    {
        loadFileCanvas.SetActive(true);
        FillInSaveFileInfo();
        CampaignCanvas.SetActive(false);
        StartMenuCanvas.SetActive(false);

        for (int i = 0; i < loadFileButtons.Count; i++)
        {
            UnityEngine.UI.Button button = loadFileButtons[i].GetComponent<UnityEngine.UI.Button>();
            //change color of all buttons when highlighted to some shade of red
            ColorBlock colorsOfButton = button.colors;
            Color highlightColor = colorsOfButton.highlightedColor;
            colorsOfButton.highlightedColor = new Color(highlightColor.r + 50, highlightColor.g, highlightColor.b, highlightColor.a);
            button.colors = colorsOfButton;
        }

        if (loadFileButtons.Count > 0 && controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(loadFileButtons[loadFileButtons.Count - 1]);
        }

        loadCanvasEnabled = true;
    }

    /// <summary>
    /// Function to fill in buttons of scrollview of the found saved game files.
    /// </summary>
    private void FillInSaveFileInfo()
    {
        string[] files = SaveAndLoadGame.saver.GetAllSaveFiles();
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Substring(Application.persistentDataPath.Length + 1, files[i].Length - Application.persistentDataPath.Length - 5);
            GameObject button = Instantiate(fileButtonPrefab) as GameObject;
            button.GetComponentInChildren<Text>().text = files[i];
            button.transform.SetParent(scrollViewContent.transform, false);
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetFileToLoadString(button.GetComponentInChildren<Text>().text); });
            loadFileButtons.Add(button);
        }
    }

    /// <summary>
    /// Function to hide the load file canvas and show the others.
    /// </summary>
    public void HideLoadCanvas()
    {
        foreach (GameObject butt in loadFileButtons)
        {
            butt.transform.parent = null;
            Destroy(butt);
        }
        loadFileButtons.Clear();
        loadFileCanvas.SetActive(false);
       
        CampaignCanvas.SetActive(true);
        checkSaves();
        if (controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //if (hasSaveGameCanvas.activeInHierarchy)
            //{
            //    //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
            //    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
            //else if (StartMenuCanvas.activeInHierarchy)
            //{
            //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
        }

        loadCanvasEnabled = false;
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
        SaveAndLoadGame.saver.SetLoadedFightScene(true);
        loadFileInput.text = "";
        LoadLevel.loader.LoadALevel(SaveAndLoadGame.saver.GetSceneNameCurrentlyOn());
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
    /// Helper function to start a new coop save game.
    /// </summary>
    public void BeginNewCoopGame()
    {
        SaveAndLoadGame.saver.StartNewCoopGame();
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

    /// <summary>
    /// Check if there is a controller plugged in and if so checks its type (PS4 vs Xbox)
    /// and then sets the appropriate variables. Only checks the first controller slot.
    /// </summary>
    private void CheckControllerModeAndType()
    {
        inputNames = Input.GetJoystickNames();
        if (inputNames.Length > 0)
        {
            if (inputNames[0].Length == 33 || inputNames[0].Length == 19)
            {
                controllerMode = true;
                if (inputNames[0].Length == 19)
                {
                    ps4Mode = true;
                }
            }
        }
    }
    /// <summary>
    /// Function for back button on SurvivalCanvas
    /// </summary>
    public void HideSurvivalCanvas()
    {
        survivalCanvas.SetActive(false);
        StartMenuCanvas.SetActive(true);

        if (controllerMode)
        {
            EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //if (hasSaveGameCanvas.activeSelf)
            //{
            //    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
            //else if (StartMenuCanvas.activeSelf)
            //{
            //    EventSystem.current.SetSelectedGameObject(StartMenuCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            //}
        }
    }
    /// <summary>
    /// Function for the start button on the SurvivalCanvas. Evaluates chosen map, mode, and faction and starts appropriate game
    /// </summary>
    public void StartSurvivalGame()
    {
        string level = survivalCurrentArenaSelection;
        string faction = "";

        //Faction is Cubemen
        if(survivalFactionDropdown.value == 0)
        {
            faction = "Cube";
            
        }
        //Faction is Octahdedron
        else if(survivalFactionDropdown.value == 1)
        {
            faction = "Octahedron";
        }


        //Mode is single player
        if (survivalModeDropdown.value == 0)
        {
            SaveAndLoadGame.saver.StartNewArenaGame(level,faction);
        }
        //Mode is coop
        else if(survivalModeDropdown.value == 1)
        {
            SaveAndLoadGame.saver.StartNewArenaCoopGame(level,faction);
        }
    }
    //Set current level selection to name of map buutton. MAP BUTTON MUST BE SAME AS SCENE NAME, but not the text ON the button.
    public void ArenaMapSelection()
    {
        if(survivalCanvas.activeInHierarchy)
        {
            survivalCurrentArenaSelection = EventSystem.current.currentSelectedGameObject.name;
            survivalCurrentMapSelection.text = EventSystem.current.currentSelectedGameObject.gameObject.GetComponentInChildren<Text>().text;
        }
    }
}
