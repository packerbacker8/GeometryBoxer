using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class attackButtonClick : MonoBehaviour {

    public UnityEngine.UI.Button attackButton;

    private GameObject citySelectController;
    private int sceneIndex;

    void Start()
    {
        attackButton.onClick.AddListener(TaskOnClick);
        citySelectController = GameObject.FindGameObjectWithTag("GameController");
    }

    void TaskOnClick()
    {
        sceneIndex = citySelectController.GetComponent<CitySelectSceneController>().GetCurrentCityBuildIndex();
        LoadingScreenManager.LoadScene(sceneIndex);
    }
}
