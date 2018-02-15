using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAndReset : MonoBehaviour
{
    private Rigidbody rigid;
    private SpecialOctahedronAttackAI special;

    private float launchedTimer;
    private float timeInAir;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="force"></param>
    /// <param name="maxTime"></param>
    public void Launch(float force, float maxTime)
    {
        launchedTimer = 0;
        timeInAir = maxTime;
        rigid = this.GetComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.AddForce(Vector3.forward * force);
        special = this.transform.parent.GetComponentInChildren<SpecialOctahedronAttackAI>();
    }

    private void Update()
    {
        launchedTimer += Time.deltaTime;
        if(launchedTimer > timeInAir)
        {
            ResetProjectile();
        }
    }

    private void ResetProjectile()
    {
        special.ProjectileWasReset();
        rigid.velocity = Vector3.zero;
        this.transform.localPosition = Vector3.forward;
        this.gameObject.SetActive(false);
    }
}
