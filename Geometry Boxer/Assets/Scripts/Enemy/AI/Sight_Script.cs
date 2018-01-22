using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight_Script : MonoBehaviour {

    bool playerNear;
	// Use this for initialization
	void Start () {
        playerNear = false;
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        Vector3 p1 = transform.position + Vector3.up;
        //Debug.DrawRay(p1, transform.forward + Vector3.up, Color.black, 2);
        if(Physics.SphereCast(p1, 1, transform.forward, out hit, 1))
        {
            if(hit.transform.root.tag == "Player")
            {
                Debug.Log("Player Detected");
                playerNear = true;
                SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
            }
            
        }
        else if(!playerNear)
        {
            Debug.Log("Player Lost");
            SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Triggered started: " + other.gameObject.tag);
        if (other.transform.root.tag == "Player")
        {
            playerNear = true;
            SendMessageUpwards("playerFound", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            playerNear = false;
            this.SendMessageUpwards("playerLost", SendMessageOptions.DontRequireReceiver);
        }
    }
}
