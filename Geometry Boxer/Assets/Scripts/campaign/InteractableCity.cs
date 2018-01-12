using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InteractableCity : Interactable
{
    public Light statusLight;
    public GUIStyle style;
    public GameObject Player;
    public GameObject MapCamera;
    public GameObject Canvas;
    public Image cityImageLink;
    public Sprite citySprite;
    public SpriteRenderer AttackIconSpriteRenderer;
    public string sceneName;

    private WorldInteraction worldInit;
    private RTSCam cam;
    private GameObject citySelectController;
    private bool exitedTrigger = false;

    private const float pulseRange = 10.0f;
    private const float pulseSpeed = 10.0f;
    private const float pulseMin = 0.0f;

    private void Awake()
    {
        worldInit = Player.GetComponent<WorldInteraction>();
        cam = MapCamera.GetComponent<RTSCam>();
        Canvas.SetActive(false);
        citySelectController = GameObject.FindGameObjectWithTag("GameController");

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
    }

    //Bring up the canvas for the city if player enters the trigger zone
    void OnTriggerEnter(Collider col)
    {
        exitedTrigger = false;
        if (col.transform.root.tag == "Player" && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "owned" && SaveAndLoadGame.saver.GetCityStatus(sceneName) != "conquered" && !exitedTrigger)
        {
            worldInit.freeze = true;
            cam.freeze = true;
            Canvas.SetActive(true);
            citySelectController.GetComponent<CitySelectSceneController>().SetCityBuildName(sceneName);
            cityImageLink.sprite = citySprite;
        }
    }
    void OnTriggerExit(Collider col)
    {
        exitedTrigger = true;
        worldInit.freeze = false;
        cam.freeze = false;
        Canvas.SetActive(false);
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
            
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Vector3 clickedPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //RaycastHit hit;
            //if (Physics.Raycast(ray, out hit, 100))
            //{
            //if(hit.transform.tag == "Interactable")
            //{

            //}
            //}
        }
        
    }
    void Update()
    {
        //Make enemy city lights pulsate red
        /*
        if (statusLight.color == Color.red)
        {
            statusLight.intensity = pulseMin + Mathf.PingPong(Time.time * pulseSpeed, pulseRange - pulseMin);
        }
        */
    }
}
