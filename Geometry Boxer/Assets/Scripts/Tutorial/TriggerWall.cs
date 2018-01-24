using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWall : MonoBehaviour {
    public GameObject wall;
    public GameObject punchingBag;
    bool playedSound = false;
    
	void OnTriggerEnter ()
    {
	    wall.BroadcastMessage("CloseWall", SendMessageOptions.RequireReceiver);
        if(!playedSound)
        {
            punchingBag.SetActive(false);
            GetComponent<AudioSource>().Play();
            playedSound = true;
        }
    }
}
