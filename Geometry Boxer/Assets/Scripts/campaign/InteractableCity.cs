using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InteractableCity : Interactable
{
    public GUIStyle style;
    private Rect Menu;
    public bool doWindow = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if(hit.transform.tag == "Interactable")
                {
                    doWindow = true;
                    int windowWidth = 200;
                    int windowHeight = 200;
                    int x = (Screen.width - windowWidth) / 2;
                    int y = (Screen.height - windowWidth) / 2;
                    Menu = new Rect(x, y, windowWidth, windowHeight);
                }
            }
        }


    }

    void OnGUI()
    {
        if (doWindow)
        {
            GUI.Window(0, Menu, MenuButtons, "Your forses are attacking a city");
        }
    }

    void MenuButtons(int i)
    {
        if (GUILayout.Button("Attack"))
        {
            doWindow = false;
            SceneManager.LoadScene("CombatTestEnvironment");
        }
        if (GUILayout.Button("Retreat"))
        {
            doWindow = false;
        }
    }
}
