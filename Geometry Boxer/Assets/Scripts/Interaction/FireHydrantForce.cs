using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;
using UTJ.Alembic;

public class FireHydrantForce : MonoBehaviour
{
    private ParticleSystem particles;
    private Collider capCollider;
    private float activateTime;
    private float activateCount;
    //private float abcTime;
    //private float abcTimeThreshold;
    //private GameObject abcObj;
    //private GameObject abcObjLoop;

    private void Start()
    {
        particles = this.GetComponent<ParticleSystem>();
        capCollider = this.GetComponent<Collider>();
        capCollider.enabled = false;
        activateTime = 1f;
        activateCount = 0f;
        //abcTime = 0f;
        //abcTimeThreshold = 2.3f;
        //abcObj = this.transform.GetChild(0).gameObject;
        //abcObjLoop = this.transform.GetChild(1).gameObject;
        //abcObj.SetActive(false);
        //abcObjLoop.SetActive(false);
    }

    public void LateUpdate()
    {
        if(!this.transform.parent && !particles.isEmitting)
        {
            activateCount += Time.deltaTime;
            if(activateCount > activateTime)
            {
                particles.Play();
                capCollider.enabled = true;
                //abcObj.SetActive(true);
            }
        }
        /*
        if(abcObj.activeInHierarchy)
        {
            abcTime += Time.deltaTime;
            if(abcTime > abcTimeThreshold)
            {
                abcObj.SetActive(false);
                abcObjLoop.SetActive(true);
            }
        }*/
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag.Contains("Player"))
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
        else if(other.transform.tag.Contains("Enemy"))
        {
            GameObject findingRoot = other.gameObject;
            while (findingRoot.tag != "EnemyRoot")
            {
                findingRoot = findingRoot.transform.parent.gameObject;
            }
            BehaviourPuppet behavePup = findingRoot.GetComponentInChildren<BehaviourPuppet>();
            behavePup.SetState(BehaviourPuppet.State.Unpinned);
            behavePup.dropProps = false;
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
        if (other.transform.tag.Contains("Player"))
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
        else if (other.transform.tag.Contains("Enemy"))
        {
            GameObject findingRoot = other.gameObject;
            while (findingRoot.tag != "EnemyRoot")
            {
                findingRoot = findingRoot.transform.parent.gameObject;
            }
            BehaviourPuppet behavePup = findingRoot.GetComponentInChildren<BehaviourPuppet>();
            behavePup.dropProps = true;
            other.transform.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 100);
        }
        }
}
