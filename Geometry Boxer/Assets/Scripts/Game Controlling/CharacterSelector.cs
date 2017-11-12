using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    private GameObject controller;

    // Use this for initialization
    void Start()
    {
        controller = GameObject.FindGameObjectWithTag("GameController");
    }

    // Update is called once per frame
    void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            controller.SendMessage("CharacterSelected", this.gameObject.name,SendMessageOptions.DontRequireReceiver);
        }
    }
}
