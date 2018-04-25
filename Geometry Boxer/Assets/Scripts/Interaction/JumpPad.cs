using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour {
    public float force = 100f;
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.transform.GetComponentInChildren<Rigidbody>();
        if (body != null)
        {
            body.AddForce(Vector3.up * force);
        }
    }
}
