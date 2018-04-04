using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectAllObjectsOfName : ScriptableWizard
{

    public string desiredName = "Your name here";
    [MenuItem("Geometry Boxer Tools/Select All Objects of Name")]

    static void SelectAllOfTagWizard()
    {
        ScriptableWizard.DisplayWizard<SelectAllObjectsOfName>("Select All Objects of Name", "Select All", "Select Root Objects Only");
    }

    void OnWizardCreate()
    {
        GameObject[] gameobjs = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> nameobjs = new List<GameObject>();
        foreach (GameObject obj in gameobjs)
        {
            if (obj.name == desiredName)
            {
                nameobjs.Add(obj);
            }
        }
        Selection.objects = nameobjs.ToArray();
    }

    private void OnWizardOtherButton()
    {
        GameObject[] gameobjects = GameObject.FindObjectsOfType<GameObject>();
        List<GameObject> rootObjects = new List<GameObject>();
        foreach (GameObject obj in gameobjects)
        {
            if (obj.name == desiredName && obj.transform.root == obj.transform)
            {
                rootObjects.Add(obj);
            }
        }
        Selection.objects = rootObjects.ToArray();
    }
}

