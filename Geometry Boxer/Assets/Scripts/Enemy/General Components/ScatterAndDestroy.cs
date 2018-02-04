using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterAndDestroy : MonoBehaviour
{

    // Use this for initialization
    public void BeginDestruction(float destroyTime)
    {
        int num = transform.childCount;
        for(int i = 0; i < num; i++)
        {
            transform.GetChild(0).gameObject.AddComponent<DestroyPiece>();
            transform.GetChild(0).GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1f,1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 1000f);
            transform.GetChild(0).gameObject.GetComponent<DestroyPiece>().DestroyThisPiece(destroyTime);
            transform.GetChild(0).parent = null; 
        }
        
    }

}
