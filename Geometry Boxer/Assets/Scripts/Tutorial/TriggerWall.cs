using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWall : MonoBehaviour {
    public GameObject wall;
    
	void OnTriggerEnter ()
    {
	    wall.BroadcastMessage("CloseWall", SendMessageOptions.RequireReceiver);
    }
}
