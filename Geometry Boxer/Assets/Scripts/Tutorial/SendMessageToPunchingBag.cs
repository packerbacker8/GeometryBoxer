using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendMessageToPunchingBag : MonoBehaviour {

    public GameObject punchingBag;
    private bool entered = false;
	
    void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.tag == "Player" && !entered)
        {
            entered = true;
            punchingBag.GetComponent<PunchingBagTrigger>().setJabText();
        }
    }
}
