using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pauseMenu : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject scrollViewContent;
    public GameObject fileButtonPrefab;

    private GameObject character;
    private GameObject control;
    private GameObject pauseMenuCanvas;
    private GameObject saveCanvas;
    private InputField saveInputField;

    RootMotion.Demos.UserControlMelee UserControlMeleeScript;
    RootMotion.CameraController CameraControllerScript;
    PunchScript punchScript;
    private string saveFileName;

    private bool mouseShouldBeLocked = false;
    private bool isPaused = false;
    private bool isCombatScene = false;
    private float TimeSinceEsc = 0.0f;
    private List<GameObject> saveFileButtons;

    private bool controllerMode = false;

    // Use this for initialization
    void Start()
    {
        control = GameObject.FindGameObjectWithTag("GameController");
        pauseMenuCanvas = this.transform.GetChild(0).gameObject;
        saveCanvas = this.transform.GetChild(1).gameObject;
        saveInputField = saveCanvas.GetComponentInChildren<InputField>();
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        saveFileName = "";
        saveFileButtons = new List<GameObject>();
        //if game controller found is game controller for combat levels, grab player info
        isCombatScene = control.name.Equals("GameController");
        if (isCombatScene)
        {
            character = control.GetComponent<GameControllerScript>().GetActivePlayer();
            CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
            punchScript = character.gameObject.GetComponent<PunchScript>();
        }
        else if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Tutorial"))
        {
            isCombatScene = true;
            character = control.GetComponent<GameControllerScriptTutorial>().GetActivePlayer();
            if(character != null)
            {
                CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
                punchScript = character.gameObject.GetComponent<PunchScript>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceEsc = TimeSinceEsc += Time.deltaTime;

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("StartButton")) && !isPaused)
        {
            if (Input.GetButtonDown("StartButton"))
            {
                controllerMode = true;
            }
            pauseGameHelper();
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("StartButton")) && isPaused)
        {
            resumeGameHelper();
        }

    }

    /// <summary>
    /// Function to pick which pause to use.
    /// </summary>
    public void pauseGameHelper()
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
            controllerMode = false;
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
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((scrollView.GetComponent<RectTransform>().rect.size.y * 0.85f) * 0.5f - 10f) - (30f * i));
            button.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { SetSaveFileName(button.GetComponentInChildren<Text>().text); });
            saveFileButtons.Add(button);
        }
    }

    /// <summary>
    /// Function to close the save canvas.
    /// </summary>
    public void CloseSaveCanvas()
    {
        saveFileButtons.Clear();
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);
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
