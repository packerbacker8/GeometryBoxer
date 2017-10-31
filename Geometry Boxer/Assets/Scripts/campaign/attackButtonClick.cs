using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class attackButtonClick : MonoBehaviour {

    public UnityEngine.UI.Button attackButton;
    public int sceneIndex;

    void Start()
    {
        attackButton.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        LoadingScreenManager.LoadScene(sceneIndex);
    }
}
