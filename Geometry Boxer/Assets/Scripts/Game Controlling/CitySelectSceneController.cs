using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySelectSceneController : MonoBehaviour
{
    private int currentCityBuildIndex;

    // Use this for initialization
    void Start()
    {
        currentCityBuildIndex = 0; //default to main menu
    }


    /// <summary>
    /// Set variable that indicates selected city with build index.
    /// </summary>
    /// <param name="index"></param>
    public void SetCityBuildIndex(int index)
    {
        currentCityBuildIndex = index;
    }

    /// <summary>
    /// Return index of currently selected city's build index.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentCityBuildIndex()
    {
        return currentCityBuildIndex;
    }
}
