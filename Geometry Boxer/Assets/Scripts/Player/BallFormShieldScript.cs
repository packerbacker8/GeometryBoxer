using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallFormShieldScript : MonoBehaviour
{
    private Collider ball;
    private Collider shield;

    private void Start()
    {
        ball = this.transform.root.GetChild(5).GetComponent<Collider>();
        shield = this.GetComponent<Collider>();
        Physics.IgnoreCollision(ball, shield, true);
    }

    private void Update()
    {
        //rotate to where camera is looking
    }
}
