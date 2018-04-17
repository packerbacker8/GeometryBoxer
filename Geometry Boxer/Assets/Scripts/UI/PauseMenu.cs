using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject scrollViewContent;
    public GameObject fileButtonPrefab;
    public GameObject pauseMenuCanvas { get; private set; }
    public bool notInDeathOrWinScreen = true;
    public bool saveCanvasTextInputMode = false;
    public bool isPaused = false;

    private GameObject character;
    private GameObject control;
    private GameObject saveCanvas;
    private InputField saveInputField;

    private RootMotion.Demos.UserControlMelee UserControlMeleeScript;
    private RootMotion.CameraController CameraControllerScript;
    private PunchScript punchScript;

    private string saveFileName;
    private string startButton;
    private string rightStickButton;
    private string submit;
    private string dPadY;
    private string dPadX;
    private string vertical;
    private string horizontalLeft;
    private string verticalLeft;

    private bool mouseShouldBeLocked = false;
    private bool isCombatScene = false;
    private bool isPlayer2;
    private bool controllerMode = false;
    private bool ps4Mode = false;

    private float TimeSinceEsc = 0.0f;

    private List<GameObject> saveFileButtons;
    private StandaloneInputModule gameEventSystemInputModule;

    // Use this for initialization
    void Start()
    {
        startButton = "StartButton";
        rightStickButton = "RightStickButton";
        submit = "Submit";
        dPadY = "DPadY";
        dPadX = "DPadX";
        vertical = "Vertical";
        horizontalLeft = "HorizontalLeft";
        verticalLeft = "VerticalLeft";

        control = GameObject.FindGameObjectWithTag("GameController");
        pauseMenuCanvas = this.transform.GetChild(0).gameObject;
        saveCanvas = this.transform.GetChild(1).gameObject;
        saveInputField = saveCanvas.GetComponentInChildren<InputField>();
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        gameEventSystemInputModule = GameObject.FindGameObjectWithTag("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
        saveFileName = "";
        saveFileButtons = new List<GameObject>();
        //if game controller found is game controller for combat levels, grab player info
        isCombatScene = control.name.Equals("GameController") || control.name.Equals("ArenaGameController");
        if (isCombatScene)
        {
            character = control.GetComponent<GameControllerScript>().GetActivePlayer();
            CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
            punchScript = character.gameObject.GetComponent<PunchScript>();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
        {
            isCombatScene = true;
            character = control.GetComponent<GameControllerScriptTutorial>().GetActivePlayer();
            if (character != null)
            {
                CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
                punchScript = character.gameObject.GetComponent<PunchScript>();
            }
        }
        isPlayer2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceEsc = TimeSinceEsc += Time.deltaTime;
        isPaused = pauseMenuCanvas.activeInHierarchy;
        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(startButton) || Input.GetButtonDown(rightStickButton) || Input.GetButtonDown(startButton + "_2") || Input.GetButtonDown(rightStickButton + "_2")) && !isPaused && notInDeathOrWinScreen)
        {
            if (Input.GetButtonDown(startButton + "_2") || Input.GetButtonDown(rightStickButton + "_2"))
            {
                isPlayer2 = true;
            }
            else
            {
                isPlayer2 = false;
            }
            //RightStickButton axis is the same as start button for the PS4 controller.
            ps4Mode = checkPS4Mode();
            if (((!isPlayer2 && Input.GetButtonDown(rightStickButton)) || (isPlayer2 && Input.GetButtonDown(rightStickButton + "_2"))) && !ps4Mode)
            {
                return;
            }
            //if its player 1 (or 2) and right trigger is pressed and its playstation 4 controller, then dont pause
            if (((!isPlayer2 && Input.GetButtonDown(startButton)) || (isPlayer2 && Input.GetButtonDown(startButton + "_2"))) && ps4Mode)
            {
                return;
            }
            if (Input.GetButtonDown(startButton) || Input.GetButtonDown(rightStickButton) || isPlayer2)
            {
                controllerMode = true;
            }
            else
            {
                controllerMode = false;
            }

            pauseGameHelper();
        }
        else if (!isPlayer2 && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(startButton) || Input.GetButtonDown(rightStickButton)) && isPaused)
        {
            if (Input.GetButtonDown(rightStickButton) && !ps4Mode)
            {
                return;
            }
            resumeGameHelper();
        }
        else if(isPlayer2 && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown(startButton) || Input.GetButtonDown(rightStickButton)) && isPaused)
        {
            if (Input.GetButtonDown(rightStickButton + "_2") && !ps4Mode)
            {
                return;
            }
            resumeGameHelper();
        }


        if (controllerMode && isPaused)
        {
            ps4Mode = checkPS4Mode();
            if (ps4Mode)
            {
                gameEventSystemInputModule.submitButton = submit + "PS4";
            }
            else
            {
                gameEventSystemInputModule.submitButton = submit;
            }
            if(isPlayer2)
            {
                gameEventSystemInputModule.submitButton += "_2";
            }

            if (ps4Mode && (Input.GetAxis(dPadY + "PS4") != 0 || Input.GetAxis(dPadY + "PS4_2") != 0))
            {
                gameEventSystemInputModule.verticalAxis = dPadY + "PS4";
            }
            else if (Input.GetAxis(dPadY) != 0 || Input.GetAxis(dPadY + "_2") != 0)
            {
                gameEventSystemInputModule.verticalAxis = dPadY;
            }
            else
            {
                gameEventSystemInputModule.verticalAxis = vertical;
            }
            if(isPlayer2)
            {
                gameEventSystemInputModule.verticalAxis += "_2";
            }


            if (saveCanvas.activeInHierarchy)
            {
                if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject.name == "InputField")
                {
                    //Debug.Log("Text mode");
                    saveCanvasTextInputMode = true;
                }
                else if (EventSystem.current.currentSelectedGameObject.name == "InputField" && !saveCanvasTextInputMode)
                {
                    EventSystem.current.SetSelectedGameObject(saveCanvas.transform.Find("SaveFileButton").gameObject);
                }

                if (!isPlayer2 && saveCanvasTextInputMode && (Input.GetAxis(horizontalLeft) != 0 || Input.GetAxis(verticalLeft) != 0))
                {
                    saveCanvasTextInputMode = false;
                }
                else if (isPlayer2 && saveCanvasTextInputMode && (Input.GetAxis(horizontalLeft + "_2") != 0 || Input.GetAxis(verticalLeft + "_2") != 0))
                {
                    saveCanvasTextInputMode = false;
                }

            }
        }
    }

    public bool checkPS4Mode()
    {
        bool ps4True = false;
        string[] inputNames = Input.GetJoystickNames();
        if(inputNames.Length > 0 && !isPlayer2)
        {
            if (inputNames[0].Length == 19)
            {
                ps4True = true;
            }
        }
        else if(inputNames.Length > 0 && isPlayer2)
        {
            if(inputNames.Length > 1)
            {
                if (inputNames[1].Length == 19)
                {
                    ps4True = true;
                }
            }
            else
            {
                if (inputNames[0].Length == 19)
                {
                    ps4True = true;
                }
            }

        }
        else
        {
            ps4True = false;
        }

        return ps4True;
    }


    /// <summary>
    /// Function to pick which pause to use.
    /// </summary>
    public void pauseGameHelper()
    {
        if (saveCanvas.activeInHierarchy)
        {
            CloseSaveCanvas();
        }
        else
        {
            if (isCombatScene)
            {
                pauseGame();
            }
            else
            {
                pauseGameNonCombat();
            }
        }

    }

    /// <summary>
    /// Function to pick which resume to use.
    /// </summary>
    public void resumeGameHelper()
    {
        if (isCombatScene)
        {
            resumeGame();
        }
        else
        {
            resumeGameNonCombat();
        }
    }

    /// <summary>
    /// Function to resume gameplay when playing in combat scenes.
    /// </summary>
    public void resumeGame()
    {
        Time.timeScale = 1.0f;
        //UserControlMeleeScript.inPause = false;
        punchScript.enabled = true;
        CameraControllerScript.enabled = true;


        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        pauseMenuCanvas.SetActive(false);
        isPaused = false;
    }

    /// <summary>
    /// Function to pause gameplay when in combat scenes.
    /// </summary>
    public void pauseGame()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0.0f;

        mouseShouldBeLocked = true;

        //UserControlMeleeScript.inPause = true;
        punchScript.enabled = false;
        CameraControllerScript.enabled = false;

        pauseMenuCanvas.SetActive(true);
        if (controllerMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameObject obj = pauseMenuCanvas.transform.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(obj);
            obj.GetComponent<UnityEngine.UI.Button>().OnSelect(null);
            //controllerMode = false;
        }

        isPaused = true;
    }

    /// <summary>
    /// Function to resume gameplay when playing in other scenes.
    /// </summary>
    public void resumeGameNonCombat()
    {
        Time.timeScale = 1.0f;

        pauseMenuCanvas.SetActive(false);
        isPaused = false;
    }

    /// <summary>
    /// Function to pause gameplay when in other scenes.
    /// </summary>
    public void pauseGameNonCombat()
    {
        Time.timeScale = 0.0f;

        mouseShouldBeLocked = true;
        pauseMenuCanvas.SetActive(true);
        if (controllerMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameObject obj = pauseMenuCanvas.transform.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(obj);
            obj.GetComponent<UnityEngine.UI.Button>().OnSelect(null);
            //controllerMode = false;
        }
        isPaused = true;
    }

    /// <summary>
    /// Function to open the save canvas.
    /// </summary>
    public void OpenSaveCanvas()
    {
        saveCanvas.SetActive(true);
        FillInSaveFileInfo();
        pauseMenuCanvas.SetActive(false);

        for (int i = 0; i < saveFileButtons.Count; i++)
        {
            UnityEngine.UI.Button button = saveFileButtons[i].GetComponent<UnityEngine.UI.Button>();
            //change color of all buttons when highlighted to some shade of red
            ColorBlock colorsOfButton = button.colors;
            Color highlightColor = colorsOfButton.highlightedColor;
            colorsOfButton.highlightedColor = new Color(highlightColor.r + 50, highlightColor.g, highlightColor.b, highlightColor.a);
            button.colors = colorsOfButton;
        }
        if (saveFileButtons.Count > 0 && controllerMode)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            EventSystem.current.SetSelectedGameObject(saveFileButtons[saveFileButtons.Count - 1]);
        }
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
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSaveFileName(button.GetComponentInChildren<Text>().text); });

            saveFileButtons.Add(button);
        }
    }

    /// <summary>
    /// Function to close the save canvas.
    /// </summary>
    public void CloseSaveCanvas()
    {
        foreach(GameObject butt in saveFileButtons)
        {
            butt.transform.parent = null;
            Destroy(butt);
        }
        saveFileButtons.Clear();
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);

        if (controllerMode)
        {
            Cursor.lockState = CursorLockMode.Locked;
            GameObject obj = pauseMenuCanvas.transform.GetChild(0).gameObject;
            EventSystem.current.SetSelectedGameObject(obj);
        }
    }

    public string GetSaveFileName()
    {
        return saveFileName;
    }

    public void SetSaveFileName()
    {
        saveFileName = saveInputField.text;
    }

    public void SetSaveFileName(string buttonText)
    {
        saveInputField.text = buttonText;
        SetSaveFileName();
    }

    public void SaveTheGame()
    {
        if(isCombatScene)
        {
            GameControllerScript gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
            HashSet<int> allyI = gameController.hasAllies ? gameController.AllyAliveIndicies() : new HashSet<int>();
            SaveAndLoadGame.saver.SetFightSceneSaveValues(gameController.EnemyAliveIndicies(), gameController.hasAllies, allyI);
            if(SaveAndLoadGame.saver.GetCharacterType().Contains("Cube"))
            {
                SaveAndLoadGame.saver.SetPlayerCurrentHealth(gameController.GetActivePlayer().GetComponent<CubeSpecialStats>().GetPlayerHealth());
            }
            else
            {
                SaveAndLoadGame.saver.SetPlayerCurrentHealth(gameController.GetActivePlayer().GetComponent<OctahedronStats>().GetPlayerHealth());
            }
            if(gameController.GetPlayer2() != null)
            {
                SaveAndLoadGame.saver.SetPlayer2CurrentHealth(gameController.GetPlayer2().GetComponent<PlayerStatsBaseClass>().GetPlayerHealth());
            }
        }
        else
        {
            SaveAndLoadGame.saver.SetFightSceneSaveValues(new HashSet<int>(), false, new HashSet<int>());
        }
        SaveAndLoadGame.saver.SetCurrentScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        SaveAndLoadGame.saver.SaveGame(saveFileName);
        saveInputField.text = "";
        CloseSaveCanvas();
    }

    public void GoToMainMenu()
    {
        LoadLevel.loader.LoadMainMenu();
    }

    public void QuitGame()
    {
        LoadLevel.loader.ExitGame();
    }
}
