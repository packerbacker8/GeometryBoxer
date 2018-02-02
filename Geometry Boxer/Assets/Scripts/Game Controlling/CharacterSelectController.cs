using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectController : MonoBehaviour
{
    public GameObject cubeChar;
    public GameObject octahedronChar;
    public Selectable beginButton;
    public float speed = 1.0f;

    private string characterSelected;
    private Vector3 cubeOrginalPos;
    private Vector3 octahedronOriginalPos;
    private Vector3 cubeForwardPos;
    private Vector3 octahedronForwardPos;

    private bool cubeSelected = false;
    private bool octaSelected = false;

    // Use this for initialization
    void Start()
    {
        characterSelected = "";
        beginButton.interactable = false;
        cubeChar.GetComponentInChildren<Light>().enabled = false;
        octahedronChar.GetComponentInChildren<Light>().enabled = false;
        cubeOrginalPos = cubeChar.transform.position;
        octahedronOriginalPos = octahedronChar.transform.position;
        cubeForwardPos = new Vector3(cubeChar.transform.position.x, cubeChar.transform.position.y, cubeChar.transform.position.z + 5.5f);
        octahedronForwardPos = new Vector3(octahedronChar.transform.position.x, octahedronChar.transform.position.y, octahedronChar.transform.position.z + 5.5f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if(cubeSelected && !octaSelected)
        {
            cubeChar.transform.position = Vector3.MoveTowards(cubeOrginalPos, cubeForwardPos, speed * Time.deltaTime);
            octahedronChar.transform.position = Vector3.MoveTowards(octahedronForwardPos, octahedronOriginalPos, speed * Time.deltaTime);
        }
        else if(octaSelected && !cubeSelected)
        {
            cubeChar.transform.position = Vector3.MoveTowards(cubeForwardPos, cubeOrginalPos, speed * Time.deltaTime);
            octahedronChar.transform.position = Vector3.MoveTowards(octahedronOriginalPos, octahedronForwardPos, speed * Time.deltaTime);
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
        LoadLevel.loader.LoadALevel(yourCity);
    }
}
