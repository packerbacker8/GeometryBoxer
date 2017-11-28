using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FireHydrantForce : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Contains("Player") || other.transform.tag.Contains("Enemy"))
        {
            //other.transform.root.GetComponentInChildren<PuppetMaster>().pinWeight = 0;
            //other.transform.root.GetComponentInChildren<PuppetMaster>().muscleWeight = 0;
            other.transform.root.GetComponentInChildren<BehaviourPuppet>().SetState(BehaviourPuppet.State.Unpinned);
            other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag.Contains("Player")|| other.transform.tag.Contains("Enemy"))
        {
            other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Contains("Player") || other.transform.tag.Contains("Enemy"))
        {
            other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
            //other.transform.root.GetComponentInChildren<PuppetMaster>().muscleWeight = 1;
        }
    }
}
