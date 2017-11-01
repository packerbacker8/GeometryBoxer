using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion;
using RootMotion.Dynamics;

public class BehaviourFreeArms : BehaviourBase
{
    /// <summary>
    /// Animation State to crosfade to when this behaviour is activated.
    /// </summary>
    public string stateName = ""; //idle maybe
    /// <summary>
    /// The duration of crossfading to "State Name". Value is in seconds.
    /// </summary>
    public float transitionDuration = 0.4f;
    /// <summary>
    /// Layer index containing the destination state. If no layer is specified or layer is -1, 
    /// the first state that is found with the given name or hash will be played.
    /// </summary>
    public int layer;
    /// <summary>
    /// Start time of the current destination state. Value is in seconds. If no explicit fixedTime 
    /// is specified or fixedTime value is float.NegativeInfinity, the state will either be played
    /// from the start if it's not already playing, or will continue playing from its current time
    /// and no transition will happen.
    /// </summary>
    public float fixedTime;
    /// <summary>
    /// The layers that will be raycasted against to find colliding objects.
    /// </summary>
    public LayerMask raycastLayers;
    /// <summary>
    /// The parameter in the Animator that blends between catch fall and writhe animations.
    /// </summary>
    public string blendParameter = "";
    /// <summary>
    /// The height of the pelvis from the ground at which will blend to writhe animation.
    /// </summary>
    public float writheHeight = 4f;
    /// <summary>
    /// The vertical velocity of the pelvis at which will blend to writhe animation.
    /// </summary>
    public float writheYVelocity = 1f;
    /// <summary>
    /// The speed of blending between the two falling animations.
    /// </summary>
    public float blendSpeed = 3f;
    /// <summary>
    /// The speed of blending in mapping on activation.
    /// </summary>
    public float blendMappingSpeed = 1f;
    /// <summary>
    /// If false, this behaviour will never end.
    /// </summary>
    public bool canEnd = true;
    /// <summary>
    /// The minimum time since this behaviour activated before it can end.
    /// </summary>
    public float minTime = 1.5f;
    /// <summary>
    /// If the velocity of the pelvis falls below this value, can end the behaviour.
    /// </summary>
    public float maxEndVelocity = 0.5f;
    /// <summary>
    /// Event triggered when all end conditions are met.
    /// </summary>
    public PuppetEvent onEnd;

    private float timer;
    private bool endTriggered;

    protected override void OnActivate()
    {
        forceActive = true;
        StopAllCoroutines();
        StartCoroutine(SmoothActivate());
    }

    protected override void OnDeactivate()
    {
        forceActive = false;
    }

    public override void OnReactivate()
    {
        timer = 0f;
        endTriggered = false;
    }

    // Making sure all params are smoothly blended, not jumping simultaneously
    private IEnumerator SmoothActivate()
    {
        timer = 0f;
        endTriggered = false;
        //puppetMaster.targetAnimator.CrossFadeInFixedTime(stateName, transitionDuration, layer, fixedTime);

        foreach (Muscle m in puppetMaster.muscles)
        {
            m.state.pinWeightMlp = 0f;
        }

        float fader = 0f;

        while (fader < 1f)
        {
            fader += Time.deltaTime;

            foreach (Muscle m in puppetMaster.muscles)
            {
                m.state.pinWeightMlp -= Time.deltaTime;
                //m.state.muscleWeightMlp += Time.deltaTime;
                m.state.mappingWeightMlp += Time.deltaTime * blendMappingSpeed;
            }

            yield return null;
        }
    }

    protected override void OnFixedUpdate()
    {
        if (raycastLayers == -1) Debug.LogWarning("BehaviourFall has no layers to raycast to.", transform);

        // Blending between catch fall and writhe animations
        //float blendTarget = GetBlendTarget(GetGroundHeight());
        //float blend = Mathf.MoveTowards(puppetMaster.targetAnimator.GetFloat(blendParameter), blendTarget, Time.deltaTime * blendSpeed);

        //puppetMaster.targetAnimator.SetFloat(blendParameter, blend);

        // Ending conditions
        timer += Time.deltaTime;

        if (!endTriggered && canEnd && timer >= minTime && !puppetMaster.isBlending && puppetMaster.muscles[0].rigidbody.velocity.magnitude < maxEndVelocity)
        {
            endTriggered = true;
            onEnd.Trigger(puppetMaster);
            return;
        }
    }

    protected override void OnLateUpdate()
    {
        puppetMaster.targetRoot.position += puppetMaster.muscles[0].transform.position - puppetMaster.muscles[0].target.position;
        GroundTarget(raycastLayers);
    }

    public override void Resurrect()
    {
        foreach (Muscle m in puppetMaster.muscles)
        {
            m.state.pinWeightMlp = 0f;
        }
    }

    // 1 is writhe animation, 0 is catch fall
    private float GetBlendTarget(float groundHeight)
    {
        if (groundHeight > writheHeight) return 1f;

        Vector3 verticalVelocity = V3Tools.ExtractVertical(puppetMaster.muscles[0].rigidbody.velocity, puppetMaster.targetRoot.up, 1f);
        float velocityY = verticalVelocity.magnitude;
        if (Vector3.Dot(verticalVelocity, puppetMaster.targetRoot.up) < 0f) velocityY = -velocityY;

        if (velocityY > writheYVelocity) return 1f;

        //if (puppetMaster.muscles[0].rigidbody.velocity.y > writheYVelocity) return 1f;
        return 0f;
    }

    // Returns the height of the first muscle from the ground
    private float GetGroundHeight()
    {
        RaycastHit hit = new RaycastHit();

        if (Physics.Raycast(puppetMaster.muscles[0].rigidbody.position, -puppetMaster.targetRoot.up, out hit, 100f, raycastLayers))
        {
            return hit.distance;
        }

        return Mathf.Infinity;
    }
}
