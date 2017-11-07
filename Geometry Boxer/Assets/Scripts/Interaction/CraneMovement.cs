using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneMovement : MonoBehaviour
{
    public float travelDistance = 10f;
    Vector3 pointA;
    Vector3 pointB;

    void Start()
    {
        pointA = transform.position;
        pointB = new Vector3(transform.position.x,transform.position.y,transform.position.z + travelDistance);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(pointA, pointB, Mathf.PingPong(Time.time/2, 1));
    }
}
