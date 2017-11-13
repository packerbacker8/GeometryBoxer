using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pauseMenu : MonoBehaviour
{

    public GameObject character;
    public GameObject scrollView;
    public GameObject fileButtonPrefab;

    private GameObject pauseMenuCanvas;
    private GameObject saveCanvas;
    private InputField saveInputField;

    RootMotion.Demos.UserControlMelee UserControlMeleeScript;
    RootMotion.CameraController CameraControllerScript;
    PunchScript punchScript;
    private string saveFileName;

    private bool mouseShouldBeLocked = false;
    private bool isPaused = false;
    private float TimeSinceEsc = 0.0f;
    private List<GameObject> saveFileButtons;

    // Use this for initialization
    void Start()
    {
        pauseMenuCanvas = this.transform.GetChild(0).gameObject;
        saveCanvas = this.transform.GetChild(1).gameObject;
        saveInputField = saveCanvas.GetComponentInChildren<InputField>();
        saveCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        UserControlMeleeScript = character.GetComponentInChildren<RootMotion.Demos.UserControlMelee>();
        CameraControllerScript = character.GetComponentInChildren<RootMotion.CameraController>();
        punchScript = character.gameObject.GetComponent<PunchScript>();
        saveFileName = "";
        saveFileButtons = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        TimeSinceEsc = TimeSinceEsc += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape) && !isPaused)
        {
            pauseGame();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            resumeGame();
        }
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    public void pauseGame()
    {
        Time.timeScale = 0.0f;

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;
        mouseShouldBeLocked = true;

        //UserControlMeleeScript.inPause = true;
        punchScript.enabled = false;
        CameraControllerScript.enabled = false;

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
            button.transform.SetParent(scrollView.transform, false);
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200f - (30f * i));
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
