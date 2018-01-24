using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWall : MonoBehaviour {
    public GameObject wall;
    public GameObject punchingBag;
    public GameObject InvisibleContainer;
    bool playedSound = false;
    
    void Start()
    {
    }

	void OnTriggerEnter ()
    {
	    wall.BroadcastMessage("CloseWall", SendMessageOptions.RequireReceiver);
        if(!playedSound)
        {
            InvisibleContainer.SetActive(true);
            punchingBag.SetActive(false);
            GetComponent<AudioSource>().Play();
            playedSound = true;
        }
    }
}
