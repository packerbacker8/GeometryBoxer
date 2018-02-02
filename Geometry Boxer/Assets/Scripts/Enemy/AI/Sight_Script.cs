using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;
public class Sight_Script : MonoBehaviour
{

    private bool inZone;
    private bool playerFound;
    private MovementBase movement;
    // Use this for initialization
    void Start()
    {
        inZone = false;
        playerFound = false;
        movement = GetComponentInParent<MovementBase>();
    }

    // Update is called once per frame
    void Update()
    {
        //if()
        //RaycastHit hit;
        //Vector3 p1 = transform.position + Vector3.up;
        //if(!inZone)
        //{
        //    if (Physics.SphereCast(p1, 1, transform.forward, out hit, 1))
        //    {
        //        if (hit.transform.root.tag == "Player")
        //        {
        //            //SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
        //            movement.playerFound();
        //        }
        //        //else
        //        //{
        //        //    //SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
                    
        //        //}
        //    }
        //    else
        //    {
        //        movement.playerLost();
        //        //SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
                
        //    }
        //}
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered started: " + other.gameObject.tag);
        if (other.transform.root.tag == "Player")
        {
            //SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
            inZone = true;
            movement.playerFound();
            //playerFound = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            //this.SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
            inZone = false;
            movement.playerLost();
            //playerFound = false;
        }
    }
}
