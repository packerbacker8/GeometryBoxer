using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SelectAllObjectOfTag : ScriptableWizard
{
    public string desiredTag = "Your tag here";
    [MenuItem("Geometry Boxer Tools/Select All Objects of Tag")]
    
    static void SelectAllOfTagWizard()
    {
        ScriptableWizard.DisplayWizard<SelectAllObjectOfTag>("Select All Objects of Tag", "Select All", "Select Root Objects Only");
    }

    void OnWizardCreate()
    {
        Selection.objects = GameObject.FindGameObjectsWithTag(desiredTag);
    }

    private void OnWizardOtherButton()
    {
        GameObject[] gameobjects = GameObject.FindGameObjectsWithTag(desiredTag);
        List<GameObject> rootObjects = new List<GameObject>();
        foreach(GameObject obj in gameobjects)
        {
            if(obj.transform.root == obj.transform)
            {
                rootObjects.Add(obj);
            }
        }
        Selection.objects = rootObjects.ToArray();
    }
}
