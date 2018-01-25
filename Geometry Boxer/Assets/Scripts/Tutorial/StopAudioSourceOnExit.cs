using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAudioSourceOnExit : MonoBehaviour {

    private AudioSource source;
    // Use this for initialization
    void Start()
    {
        source = this.GetComponent<AudioSource>();
    }
    void OnTriggerExit(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            source.Stop();
        }
    }
}
