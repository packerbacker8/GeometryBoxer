using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerWall : MonoBehaviour {
    public GameObject wall;
    public GameObject InvisibleContainer;
    bool playedSound = false;

	void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            wall.BroadcastMessage("CloseWall", SendMessageOptions.RequireReceiver);
            if (!playedSound)
            {
                InvisibleContainer.SetActive(true);
                GetComponent<AudioSource>().Play();
                playedSound = true;
            }
        }
    }
}
