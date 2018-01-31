using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionTextInTrigger : MonoBehaviour {
    public Canvas canvas;
    public AudioClip ping;

    [TextArea]
    public string prompt;

    private string text;
    private string key = "/button";
    private bool pingPlayed;
    private AudioSource pinger;
    private Text textComponent;

    private KeyCode jump;
    private KeyCode walk;
    private KeyCode zoomin;
    private KeyCode zoomout;

    // Use this for initialization
    void Start () {
        pingPlayed = false;
        pinger = this.gameObject.AddComponent<AudioSource>();
        pinger.clip = ping;
        if (canvas != null)
        {
            textComponent = canvas.transform.GetChild(0).GetComponent<Text>();
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
