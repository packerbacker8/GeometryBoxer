using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionReceived : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        this.transform.gameObject.SendMessageUpwards("ImpactReceived", collision);
    }
}
