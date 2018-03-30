using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;

public class CharacterSelectController : MonoBehaviour
{
    public GameObject cubeChar;
    public GameObject octahedronChar;
    public Selectable beginButton;
    public Text textElement;
    public Text cubemanText;
    public Text octahedronText;
    public float speed = 1.0f;

    private string characterSelected;
    private string[] inputNames;

    private Vector3 cubeOrginalPos;
    private Vector3 octahedronOriginalPos;
    private Vector3 cubeForwardPos;
    private Vector3 octahedronForwardPos;

    private bool cubeSelected = false;
    private bool octaSelected = false;

    private bool controllerMode = false;
    private bool ps4Mode = false;
    private StandaloneInputModule gameEventSystemInputModule;
    // Use this for initialization
    void Start()
    {
        characterSelected = "";
        beginButton.GetComponent<Image>().color = Color.clear;
        beginButton.interactable = false;
        cubeChar.GetComponentInChildren<Light>().enabled = false;
        octahedronChar.GetComponentInChildren<Light>().enabled = false;
        cubeOrginalPos = cubeChar.transform.position;
        octahedronOriginalPos = octahedronChar.transform.position;
        cubeForwardPos = new Vector3(cubeChar.transform.position.x, cubeChar.transform.position.y, cubeChar.transform.position.z - 0.5f);
        octahedronForwardPos = new Vector3(octahedronChar.transform.position.x, octahedronChar.transform.position.y, octahedronChar.transform.position.z - 0.5f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        textElement.text = "Pick Your Faction";
        cubemanText.enabled = false;
        octahedronText.enabled = false;

        controllerMode = false;
        CheckControllerModeAndType();
        gameEventSystemInputModule = GameObject.FindGameObjectWithTag("EventSystem").gameObject.GetComponent<StandaloneInputModule>();
    }

    void Update()
    {
        CheckControllerModeAndType();
        if (controllerMode)
        {
            if (ps4Mode)
            {
                gameEventSystemInputModule.submitButton = "SubmitPS4";
            }
            else
            {
                gameEventSystemInputModule.submitButton = "Submit";
            }


            //Debug.Log(Input.GetAxis("DPadY"));
            if (Input.GetAxis("HorizontalLeft") < -0.5f || (!ps4Mode && Input.GetAxis("DPadY") < -0.5f) || Input.GetAxis("DPadX") < -0.5f || Input.GetAxis("DPadYPS4") < -0.5f)
            {
                CharacterSelected("Cube");
            }
            else if (Input.GetAxis("HorizontalLeft") > 0.5f || (!ps4Mode && Input.GetAxis("DPadY") > 0.5f) || Input.GetAxis("DPadX") > 0.5f || Input.GetAxis("DPadYPS4") > 0.5f)
            {
                CharacterSelected("Octahedron");
            }
        }
        if(!cubeSelected && !octaSelected)
        {
            beginButton.enabled = false;
        }
        if(cubeSelected && !octaSelected)
        {
            cubeChar.transform.position = Vector3.MoveTowards(cubeForwardPos, cubeOrginalPos, speed * Time.deltaTime);
            octahedronChar.transform.position = Vector3.MoveTowards(octahedronOriginalPos, octahedronForwardPos, speed * Time.deltaTime);
            beginButton.enabled = true;
            textElement.text = "Fight as a Cubeman";
            cubemanText.enabled = true;
            octahedronText.enabled = false;
            beginButton.GetComponent<Image>().color = Color.grey;

            EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
        else if(octaSelected && !cubeSelected)
        {
            cubeChar.transform.position = Vector3.MoveTowards(cubeOrginalPos, cubeForwardPos, speed * Time.deltaTime);
            octahedronChar.transform.position = Vector3.MoveTowards(octahedronForwardPos, octahedronOriginalPos, speed * Time.deltaTime);
            beginButton.enabled = true;
            textElement.text = "Fight as an Octahedron";
            cubemanText.enabled = false;
            octahedronText.enabled = true;
            beginButton.GetComponent<Image>().color = Color.grey;

            EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
    }

    public void CharacterSelected(string charPicked)
    {
        beginButton.interactable = true;
        if (charPicked.Contains("Cube"))
        {
            characterSelected = "Cube";
            cubeChar.GetComponentInChildren<Light>().enabled = true;
            octahedronChar.GetComponentInChildren<Light>().enabled = false;
            cubeSelected = true;
            octaSelected = false;
        }
        else if(charPicked.Contains("Octahedron"))
        {
            characterSelected = "Octahedron";
            cubeChar.GetComponentInChildren<Light>().enabled = false;
            octahedronChar.GetComponentInChildren<Light>().enabled = true;
            octaSelected = true;
            cubeSelected = false;
        }
        else
        {
            /*
            characterSelected = "";
            beginButton.interactable = false;
            cubeChar.GetComponentInChildren<Light>().enabled = false;
            sphereChar.GetComponentInChildren<Light>().enabled = false;
            octahedronChar.GetComponentInChildren<Light>().enabled = false;
            if (cubeChar.transform.position != cubeOrginalPos)
            {
                cubeChar.transform.position = Vector3.Lerp(cubeForwardPos, cubeOrginalPos, lerpTime);
            }
            if (sphereChar.transform.position != sphereOriginalPos)
            {
                sphereChar.transform.position = Vector3.Lerp(sphereForwardPos, sphereOriginalPos, lerpTime);
            }
            if (octahedronChar.transform.position != octahedronOriginalPos)
            {
                octahedronChar.transform.position = Vector3.Lerp(octahedronForwardPos, octahedronOriginalPos, lerpTime);
            }
            */
        }
        
    }

    public void BeginDominationClicked()
    {
        SaveAndLoadGame.saver.SetCharType(characterSelected);
        string yourCity = SaveAndLoadGame.saver.GetCityName(characterSelected);
        if(yourCity == "nothing")
        {
            Debug.Log("problem, city not found");
        }
        //This was the original line
        //LoadLevel.loader.LoadALevel(yourCity);
        if (cubeSelected)
        {
            LoadLevel.loader.LoadALevel("DefendShippingYard");
        }
        else
        {
            //Replace this with the octahedrons cutscene level
            LoadLevel.loader.LoadALevel("DefendNightclub");
        }
        
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
}
