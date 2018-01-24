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

    [TextArea]
    private string text;
    private Text textComponent;
	
    // Use this for initialization
	void Start () {
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
