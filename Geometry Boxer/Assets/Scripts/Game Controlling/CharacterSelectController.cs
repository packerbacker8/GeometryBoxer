using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectController : MonoBehaviour
{
    public GameObject cubeChar;
    public GameObject sphereChar;
    public GameObject octahedronChar;
    public Selectable beginButton;

    private string characterSelected;
    private Vector3 cubeOrginalPos;
    private Vector3 sphereOriginalPos;
    private Vector3 octahedronOriginalPos;
    private Vector3 cubeForwardPos;
    private Vector3 sphereForwardPos;
    private Vector3 octahedronForwardPos;
    private float lerpTime;

    // Use this for initialization
    void Start()
    {
        characterSelected = "";
        beginButton.interactable = false;
        cubeChar.GetComponentInChildren<Light>().enabled = false;
        sphereChar.GetComponentInChildren<Light>().enabled = false;
        octahedronChar.GetComponentInChildren<Light>().enabled = false;
        cubeOrginalPos = new Vector3(0, 0, -4.8f);
        sphereOriginalPos = new Vector3(-3f, 0, -4.8f);
        octahedronOriginalPos = new Vector3(3f, 0, -4.8f);
        cubeForwardPos = new Vector3(0, 0, -6.8f);
        sphereForwardPos = new Vector3(-1.5f, 0, -6.8f);
        octahedronForwardPos = new Vector3(1.5f, 0, -6.8f);
        lerpTime = 1.5f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CharacterSelected(string charPicked)
    {
        beginButton.interactable = true;
        if (charPicked.Contains("Cube"))
        {
            characterSelected = "Cube";
            cubeChar.GetComponentInChildren<Light>().enabled = true;
            sphereChar.GetComponentInChildren<Light>().enabled = false;
            octahedronChar.GetComponentInChildren<Light>().enabled = false;
            if (cubeChar.transform.position != cubeForwardPos)
            {
                cubeChar.transform.position = Vector3.Lerp(cubeOrginalPos, cubeForwardPos, lerpTime);
            }
            if (sphereChar.transform.position != sphereOriginalPos)
            {
                sphereChar.transform.position = Vector3.Lerp(sphereForwardPos, sphereOriginalPos, lerpTime);
            }
            if(octahedronChar.transform.position != octahedronOriginalPos)
            {
                octahedronChar.transform.position = Vector3.Lerp(octahedronForwardPos, octahedronOriginalPos, lerpTime);
            }
        }
        else if(charPicked.Contains("Sphere"))
        {
            characterSelected = "Sphere";
            cubeChar.GetComponentInChildren<Light>().enabled = false;
            sphereChar.GetComponentInChildren<Light>().enabled = true;
            octahedronChar.GetComponentInChildren<Light>().enabled = false;
            if (cubeChar.transform.position != cubeOrginalPos)
            {
                cubeChar.transform.position = Vector3.Lerp(cubeForwardPos, cubeOrginalPos, lerpTime);
            }
            if (sphereChar.transform.position != sphereForwardPos)
            {
                sphereChar.transform.position = Vector3.Lerp(sphereOriginalPos, sphereForwardPos, lerpTime);
            }
            if (octahedronChar.transform.position != octahedronOriginalPos)
            {
                octahedronChar.transform.position = Vector3.Lerp(octahedronForwardPos, octahedronOriginalPos, lerpTime);
            }
        }
        else if(charPicked.Contains("Octahedron"))
        {
            characterSelected = "Octahedron";
            cubeChar.GetComponentInChildren<Light>().enabled = false;
            sphereChar.GetComponentInChildren<Light>().enabled = false;
            octahedronChar.GetComponentInChildren<Light>().enabled = true;
            if (cubeChar.transform.position != cubeOrginalPos)
            {
                cubeChar.transform.position = Vector3.Lerp(cubeForwardPos, cubeOrginalPos, lerpTime);
            }
            if (sphereChar.transform.position != sphereOriginalPos)
            {
                sphereChar.transform.position = Vector3.Lerp(sphereForwardPos, sphereOriginalPos, lerpTime);
            }
            if (octahedronChar.transform.position != sphereForwardPos)
            {
                octahedronChar.transform.position = Vector3.Lerp(octahedronOriginalPos, octahedronForwardPos, lerpTime);
            }
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
