using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchAndReset : MonoBehaviour
{
    private Rigidbody rigid;
    private SpecialOctahedronAttackAI special;

    private float launchedTimer;
    private float timeInAir;
    private float launchForce;
    private float startShrinkMult = 0.4f;
    private float shrinkAmountX;
    private float shrinkAmountYZ;

    private Vector3 launchDir;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="force"></param>
    /// <param name="maxTime"></param>
    public void Launch(float force, float maxTime, Vector3 dir)
    {
        launchedTimer = 0;
        timeInAir = maxTime;
        launchDir = dir;
        launchForce = force;
        shrinkAmountX = this.transform.localScale.x / (timeInAir * startShrinkMult);
        shrinkAmountYZ = this.transform.localScale.y / (timeInAir * startShrinkMult);
        rigid = this.GetComponent<Rigidbody>();
        rigid.useGravity = false;
        rigid.AddForce(launchDir * launchForce);
        special = this.transform.parent.GetComponentInChildren<SpecialOctahedronAttackAI>();
    }

    private void Update()
    {
        launchedTimer += Time.deltaTime;
        if(launchedTimer > timeInAir)
        {
            ResetProjectile();
        }
        if (!rigid.useGravity && launchedTimer > timeInAir * startShrinkMult)
        {
            rigid.useGravity = true;
        }
        else if(rigid.useGravity)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x - (shrinkAmountX * Time.deltaTime), this.transform.localScale.y - (shrinkAmountYZ * Time.deltaTime), this.transform.localScale.z - (shrinkAmountYZ * Time.deltaTime));
            if(this.transform.localScale.x < 0 || this.transform.localScale.y < 0 || this.transform.localScale.z < 0)
            {
                this.transform.localScale = Vector3.one * 0.001f;
            }
        }
    }

    private void ResetProjectile()
    {
        special.ProjectileWasReset();
        rigid.velocity = Vector3.zero;
        //this.transform.position = special.transform.position + Vector3.forward;
        this.gameObject.SetActive(false);
    }
}
