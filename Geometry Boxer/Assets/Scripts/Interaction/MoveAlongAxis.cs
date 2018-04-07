using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongAxis : MonoBehaviour
{
    public float speed = 10f;
    public float travelDistance = 10f;
    public bool XAxis;
    public bool YAxis;
    public bool ZAxis;
    Vector3 pointA;
    Vector3 pointB;

    void Start()
    {
        pointA = transform.position;
        if(XAxis)
        {
            pointB = new Vector3(transform.position.x + travelDistance, transform.position.y, transform.position.z);
        }
        else if (YAxis)
        {
            pointB = new Vector3(transform.position.x, transform.position.y + travelDistance, transform.position.z);
        }
        else if(ZAxis)
        {
            pointB = new Vector3(transform.position.x, transform.position.y, transform.position.z + travelDistance);
        }
        
    }
    void Update()
    {
        transform.position = Vector3.Lerp(pointA, pointB, Mathf.PingPong(Time.time/2, 1));
    }
}
