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
    
    private string text;
    private Text textComponent;
    private bool pingPlayed;
    private AudioSource pinger;
    private GameObject panel;

    // Use this for initialization
	void Start ()
    {
        pingPlayed = false;
        pinger = this.gameObject.AddComponent<AudioSource>();
        pinger.clip = ping;
		if(canvas != null)
        {
            panel = canvas.transform.GetChild(0).gameObject;
            textComponent = canvas.transform.GetChild(1).GetComponent<Text>();
            panel.SetActive(false);
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
            pinger.PlayOneShot(ping, 1f);
            pingPlayed = true;
            panel.SetActive(true);
        }
    }
    void OnTriggerStay(Collider col)
    {
        if(col.transform.root.tag == "Player")
        {
            textComponent.text = text;
            panel.SetActive(true);
        }
        
    }
    void OnTriggerExit(Collider col)
    {
        if (col.transform.root.tag == "Player")
        {
            panel.SetActive(false);
            textComponent.text = "";
        }
    }
}
