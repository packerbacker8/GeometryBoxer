using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBounce : MonoBehaviour
{
    public float travelDistance = 10f;
    Vector3 pointA;
    Vector3 pointB;

    void Start()
    {
        pointA = transform.position;
        pointB = new Vector3(transform.position.x,transform.position.y + travelDistance, transform.position.z);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(pointA, pointB, Mathf.PingPong(Time.time, 1));
    }
}
