using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWall : MonoBehaviour {
    public GameObject wall;
    bool playedSound = false;
    
	void OnTriggerEnter ()
    {
	    wall.BroadcastMessage("CloseWall", SendMessageOptions.RequireReceiver);
        if(!playedSound)
        {
            GetComponent<AudioSource>().Play();
            playedSound = true;
        }
    }
}
