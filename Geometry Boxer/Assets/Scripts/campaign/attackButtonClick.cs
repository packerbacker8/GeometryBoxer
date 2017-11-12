using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class attackButtonClick : MonoBehaviour {

    public UnityEngine.UI.Button attackButton;

    private GameObject citySelectController;
    private string sceneName;

    void Start()
    {
        attackButton.onClick.AddListener(TaskOnClick);
        citySelectController = GameObject.FindGameObjectWithTag("GameController");
    }

    void TaskOnClick()
    {
        sceneName = citySelectController.GetComponent<CitySelectSceneController>().GetCurrentCityBuildName();
        LoadingScreenManager.LoadScene(sceneName);
    }
}
