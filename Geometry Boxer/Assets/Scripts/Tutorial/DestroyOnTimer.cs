using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnTimer : MonoBehaviour {

    public float time = 5f;
    public GameObject punchingBag;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            Destroy(punchingBag.GetComponent<PunchingBagTrigger>());
            Destroy(this.gameObject, time);
        }
    }
}
