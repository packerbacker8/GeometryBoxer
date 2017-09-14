using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchScript : MonoBehaviour
{

    public bool useController = true;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && this.transform.name == "LowerLeftArm")
        {
            this.GetComponent<Rigidbody>().AddForce(this.transform.forward * 100f);

        }
        if (Input.GetKeyDown(KeyCode.E) && this.transform.name == "LowerRightArm")
        {
            this.GetComponent<Rigidbody>().AddForce(this.transform.forward * 10000f);

        }
    }
}
