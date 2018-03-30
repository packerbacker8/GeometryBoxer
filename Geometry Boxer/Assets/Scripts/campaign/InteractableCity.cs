using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class InteractableCity : Interactable
{
    public Text cityTitleText;
    public Text descriptionText;
    public Light statusLight;
    public GUIStyle style;
    public GameObject Player;
    public GameObject MapCamera;
    public GameObject Canvas;
    public Image cityImageLink;
    public Sprite citySprite;
    public SpriteRenderer AttackIconSpriteRenderer;
    public string sceneName;
    public string CityTitle;

    [TextArea]
    public string description;

    private string dPadX = "DPadX";
    private string dPadXPS4 = "DPadXPS4";
    private WorldInteraction worldInit;
    private RTSCam cam;
    private GameObject citySelectController;
    private SphereCollider sphere;
    private GameObject eventSystem;
    private PauseMenu pauseMenuScript;
    private bool exitedTrigger = false;
    private float sphereOriginalRad;

    private const float pulseRange = 10.0f;
    private const float pulseSpeed = 10.0f;
    private const float pulseMin = 0.0f;

    private void Awake()
    {
        worldInit = Player.GetComponent<WorldInteraction>();
        cam = MapCamera.GetComponent<RTSCam>();
        Canvas.SetActive(false);
        citySelectController = GameObject.FindGameObjectWithTag("GameController");
        Cursor.visible = false;

        statusLight.intensity = pulseRange;
        //If city is owned, switch light from red to green. What status goes here?
        if(SaveAndLoadGame.saver.GetCityStatus(sceneName) == "owned" || SaveAndLoadGame.saver.GetCityStatus(sceneName) == "conquered")
        {
            //statusLight.color = Color.green;
            statusLight.enabled = false;
            AttackIconSpriteRenderer.enabled = false;
        }
        else
        {
            AttackIconSpriteRenderer.enabled = true;
            statusLight.color = Color.red;
        }
        sphere = this.GetComponent<SphereCollider>();
        sphereOriginalRad = sphere.radius;

        //Set the Dpad X axis as the way to select buttons on the 
        eventSystem = GameObject.FindGameObjectWithTag("EventSystem").gameObject;
        pauseMenuScript = GameObject.FindGameObjectWithTag("PauseMenu").gameObject.GetComponent<PauseMenu>();

        //If the pauseMenu's checkPS4Mode believe PS4 controller is at helm, use that axis
        if (pauseMenuScript.checkPS4Mode())
        {
            eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis = dPadXPS4;
        }
        else //otherwise use XBox's
        {
            eventSystem.GetComponent<StandaloneInputModule>().horizontalAxis = dPadX;
        }
    }

    //Bring up the canvas for the city if player enters the trigger zone
    void OnTriggerEnter(Collider col)
    {
        exitedTrigger = false;
        if (col.transform.root.tag == "Player" && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "owned" && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "conquered" && !exitedTrigger)
        {
            //If no joystick plugged in, turn mouse on
            if(Input.GetJoystickNames().Length == 0)
            {
                Cursor.visible = true;
            }
            worldInit.freeze = true;
            cam.freeze = true;
            Canvas.SetActive(true);
            EventSystem.current.SetSelectedGameObject(Canvas.transform.GetChild(3).gameObject);
            citySelectController.GetComponent<CitySelectSceneController>().SetCityBuildName(sceneName);
            cityImageLink.sprite = citySprite;
            cityTitleText.text = CityTitle;
            descriptionText.text = description;
            //for controller
            EventSystem.current.SetSelectedGameObject(Canvas.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
            sphere.radius = sphere.radius * 1.05f;
        }
        
    }
    void OnTriggerExit(Collider col)
    {
        exitedTrigger = true;
        worldInit.freeze = false;
        cam.freeze = false;
        Canvas.SetActive(false);
        sphere.radius = sphereOriginalRad;
        Cursor.visible = false;
    }

    //Bring up the canvas for the city if clicked on
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "owned" && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "conquered") //CHANGE TO BRING UP A DIFFERENT CANVAS IN THE FUTURE
        {
            worldInit.freeze = true;
            cam.freeze = true;
            Canvas.SetActive(true);
            citySelectController.GetComponent<CitySelectSceneController>().SetCityBuildName(sceneName);
            cityImageLink.sprite = citySprite;
        }
    }
}
