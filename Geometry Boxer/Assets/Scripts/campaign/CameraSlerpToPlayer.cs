using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSlerpToPlayer : MonoBehaviour {
    
    private GameObject target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("GameController").GetComponent<CitySelectSceneController>().GetActivePlayer().transform.GetChild(2).gameObject;
        Debug.Log(target.name);
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.LookAt(target.transform);
    }
}
