using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class deathMenu : MonoBehaviour {

    private string reloadLevelString;

	// Use this for initialization
	void Start () {


        

	}
	
	// Update is called once per frame
	void Update () {
		

	}

    public void retryButton ()
    {
        LoadLevel.loader.LoadALevel(reloadLevelString);
    }

    public void SetReloadString (string s)
    {
        reloadLevelString = s;
    }

    public void setButtonActive()
    {
        //for controller
        EventSystem.current.SetSelectedGameObject(this.gameObject.GetComponentInChildren<UnityEngine.UI.Button>().gameObject);
    }

}
