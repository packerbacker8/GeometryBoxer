using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight_Script : MonoBehaviour
{

    private bool inZone;
    // Use this for initialization
    void Start()
    {
        inZone = false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector3 p1 = transform.position + Vector3.up;
        Debug.DrawRay(p1, transform.forward + Vector3.up, Color.black, 2);
        if (Physics.SphereCast(p1, 1, transform.forward, out hit, 1))
        {
            if (hit.transform.root.tag == "Player")
            {
                Debug.Log("Player Found1");
                SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (!inZone)
        {
            SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Sight Radius OnTriggerEnter, root = " + other.transform.name);
        //Debug.Log("Triggered started: " + other.gameObject.tag);
        if (other.transform.root.tag == "Player")
        {
            inZone = true;
            Debug.Log("Player found2");
            SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            this.SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
            inZone = false;
        }
    }
}
