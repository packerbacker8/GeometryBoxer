using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {
    
    public List<GameObject> targets;
    private float activateThreshold = 10f;
    void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > activateThreshold)
        {
            for(int i = 0; i < targets.Count; i++)
            {
                targets[i].SendMessage("Activate");
            }
            Debug.Log("Button punched");
        }
    }

}
