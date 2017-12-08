using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FireHydrantAlbemic : MonoBehaviour
{
    private float activateTime;
    private float activateCount;
    
    // Use this for initialization
    void Start()
    {
        activateTime = 1f;
        activateCount = 0f;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!this.transform.parent)
        {
            activateCount += Time.deltaTime;
            if (activateCount > activateTime)
            {
            }
        }
    }
}
