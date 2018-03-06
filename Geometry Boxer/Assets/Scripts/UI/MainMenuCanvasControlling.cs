using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using RootMotion.Demos;

public class MainMenuCanvasControlling : MonoBehaviour
{
    public GameObject hasSaveGameCanvas;
    public GameObject noSaveGameCanvas;
    public GameObject optionsMenu;
    public GameObject loadFileCanvas;
    public GameObject fileButtonPrefab;
    public GameObject scrollView;
    public GameObject scrollViewContent;
    public GameObject playerUI;

    private bool hasSavedGame;
    private InputField loadFileInput;
    private string fileToLoad;
    private List<GameObject> loadFileButtons;

    private bool controllerMode;
    private bool ps4Mode = false;
    //private bool mouseMode = true;
    private bool menuActive = false;
    private bool allowNavigation = false;
    private float timeSinceDPAD = 0.0f;
    private StandaloneInputModule EventSystemInputModule;

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

        controllerMode = false;
        string[] inputNames = Input.GetJoystickNames();
        for (int i = 0; i < inputNames.Length; i++)
        {       //Length == 33 is Xbox One Controller... Length == 19 is PS4 Controller
            if (inputNames[i].Length == 33 || inputNames[i].Length == 19)
            {
                controllerMode = true;
                if (inputNames[i].Length == 19)
                    ps4Mode = true;
            }
        }

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
    }

    void disablePlayerForController()
    {
        if (hasSaveGameCanvas.activeSelf)
        {
            //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
            EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        }
        else if (noSaveGameCanvas.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(noSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
        }


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
        EventSystem.current.SetSelectedGameObject(null);
        //mouseMode = true;
    }


    void Update()
    {
        //check this frame if a controller is plugged in or not, and change modes accordingly.
        string[] inputNames = Input.GetJoystickNames();
        for (int i = 0; i < inputNames.Length; i++)
        {       //Length == 33 is Xbox One Controller... Length == 19 is PS4 Controller
            if (inputNames[i].Length == 33 || inputNames[i].Length == 19)
            {
                //if (mouseMode)
               // {
                    //disablePlayerForController();
                //    mouseMode = false;
                //}
                controllerMode = true;
                if (inputNames[i].Length == 19)
                {
                    ps4Mode = true;
                }
                else
                {
                    ps4Mode = false;
                }
            }
            else
            {
                controllerMode = false;
            }
        }

        //Debug.Log(menuActive);

        if (controllerMode)
        {
                if(ps4Mode)
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
                if(!allowNavigation && timeSinceDPAD >= 0.1f)
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
                        
                        if (hasSaveGameCanvas.activeSelf)
                        {
                            //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
                            EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                        }
                        else if (noSaveGameCanvas.activeSelf)
                        {
                            EventSystem.current.SetSelectedGameObject(noSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);  
                        }
                        
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
                    menuActive = false;
                    enablePlayerForMouse();
                }
            //EventSystemInputModule.verticalAxis = "DPadY";
        }
        else
        {
            enablePlayerForMouse();
            EventSystemInputModule.verticalAxis = "Vertical";
        }
        

        if (loadFileCanvas.activeSelf == true && controllerMode && EventSystem.current.currentSelectedGameObject.name == "LoadInputField")
        {
            EventSystem.current.SetSelectedGameObject(loadFileCanvas.transform.Find("LoadFileButton").gameObject);
        }
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

            if (controllerMode)
            {
                EventSystem.current.SetSelectedGameObject(optionsMenu.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            }

        }
        else
        {
            hasSaveGameCanvas.SetActive(hasSavedGame);
            noSaveGameCanvas.SetActive(!hasSavedGame);
            if(controllerMode)
            {
                if (hasSaveGameCanvas.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                }
                else if (noSaveGameCanvas.activeSelf)
                {
                    EventSystem.current.SetSelectedGameObject(noSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
                }
            }
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
            button.transform.SetParent(scrollViewContent.transform,false);
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
        int buttonsCount = scrollViewContent.transform.childCount;
        for (int i = 0; i < buttonsCount; i++)
        {
            DestroyImmediate(scrollViewContent.transform.GetChild(0).gameObject);
        }
        loadFileCanvas.SetActive(false);
        hasSavedGame = SaveAndLoadGame.saver.CheckForSaveGame();
        hasSaveGameCanvas.SetActive(hasSavedGame);
        noSaveGameCanvas.SetActive(!hasSavedGame);

        if (controllerMode)
        {
            if (hasSaveGameCanvas.activeSelf)
            {
                //UnityEngine.UI.Button[] a = hasSaveGameCanvas.GetComponentsInChildren<UnityEngine.UI.Button>();
                EventSystem.current.SetSelectedGameObject(hasSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            }
            else if (noSaveGameCanvas.activeSelf)
            {
                EventSystem.current.SetSelectedGameObject(noSaveGameCanvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            }
        }
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
