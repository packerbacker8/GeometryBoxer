using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinX : MonoBehaviour {

    public float speed = 10f;
	// Update is called once per frame
	void Update ()
    {
        transform.RotateAround(transform.position, transform.up, Time.deltaTime * speed * 90f);
    }
}
