using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour {

    // Use this for initialization
    public Transform goal;
    public GameObject player;
    NavMeshAgent agent;
    void Start()
    {
       agent = GetComponent<NavMeshAgent>();
        agent.destination = player.transform.position;
    }

    // Update is called once per frame
    void Update () {
        agent.destination = player.transform.position;
    }
}
