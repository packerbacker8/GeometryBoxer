using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFormIgnore : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.root.tag.Contains("Player") && collision.gameObject.GetComponent<Collider>() != null)
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
        }
    }
}
