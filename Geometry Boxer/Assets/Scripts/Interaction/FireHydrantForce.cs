using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

public class FireHydrantForce : MonoBehaviour
{
    private ParticleSystem particles;
    private Collider capCollider;
    private float activateTime;
    private float activateCount;

    private void Start()
    {
        particles = this.GetComponent<ParticleSystem>();
        capCollider = this.GetComponent<Collider>();
        capCollider.enabled = false;
        activateTime = 3f;
        activateCount = 0f;
    }

    public void LateUpdate()
    {
        if(!this.transform.parent)
        {
            activateCount += Time.deltaTime;
            if(activateCount > activateTime)
            {
                particles.Play();
                capCollider.enabled = particles.isEmitting;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Contains("Player") || other.transform.tag.Contains("Enemy"))
        {
            //other.transform.root.GetComponentInChildren<PuppetMaster>().pinWeight = 0;
            //other.transform.root.GetComponentInChildren<PuppetMaster>().muscleWeight = 0;
            if(other.transform.root.GetComponentInChildren<BehaviourPuppet>() == null)
            {
                other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
            }
            else
            {
                other.transform.root.GetComponentInChildren<BehaviourPuppet>().dropProps = false;
                other.transform.root.GetComponentInChildren<BehaviourPuppet>().SetState(BehaviourPuppet.State.Unpinned);
                other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
            }
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
            if (other.transform.root.GetComponentInChildren<BehaviourPuppet>() == null)
            {
                other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
            }
            else
            {
                other.transform.root.GetComponentInChildren<BehaviourPuppet>().dropProps = true;
                other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
            }
        }
    }
}
