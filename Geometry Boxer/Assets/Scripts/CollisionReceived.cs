using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class CollisionReceived : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        this.transform.gameObject.SendMessageUpwards("ImpactReceived", collision, SendMessageOptions.DontRequireReceiver);
        this.transform.gameObject.SendMessageUpwards("SendImpactSound", collision, SendMessageOptions.DontRequireReceiver);
    }
}
