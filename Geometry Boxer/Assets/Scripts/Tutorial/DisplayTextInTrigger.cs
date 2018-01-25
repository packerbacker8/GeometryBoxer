using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTextInTrigger : MonoBehaviour {
    public Canvas canvas;
    [TextArea]
    public string textKeyboard;
    [TextArea]
    public string textController;
    public AudioClip ping;

    [TextArea]
    private string text;
    private Text textComponent;
    private bool pingPlayed;
    private AudioSource pinger;

    // Use this for initialization
	void Start ()
    {
        pingPlayed = false;
        pinger = this.gameObject.AddComponent<AudioSource>();
        pinger.clip = ping;
		if(canvas != null)
        {
            textComponent = canvas.transform.GetChild(0).GetComponent<Text>();
        }
        if(Input.GetJoystickNames().Length > 0)
        {
            text = textController;
        }
        else
        {
            text = textKeyboard;
        }
	}
    void OnTriggerEnter(Collider col)
    {
        if(col.transform.root.tag == "Player" && !pingPlayed && ping != null)
        {
            pinger.PlayOneShot(ping, 0.5f);
            pingPlayed = true;
        }
    }
    void OnTriggerStay(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            textComponent.text = text;
        }
        
    }
    void OnTriggerExit(Collider col)
    {
        if (col.transform.root.tag == "Player")
        {
            textComponent.text = "";
        }
    }
}
