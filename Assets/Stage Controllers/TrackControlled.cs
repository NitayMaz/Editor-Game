using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrackControlled : MonoBehaviour
{
    protected Animator animator;
    public bool controlledByTimeLine = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void SetAnimationFrame(AnimationClip animationClip, float time)
    {
        if (!controlledByTimeLine)
            return;
        float animationSecond = animationClip.length * time;
        animationClip.SampleAnimation(gameObject, animationSecond);
    }

    public virtual void StartInteraction()
    {
        controlledByTimeLine = false;
        animator.enabled = true;
        animator.Rebind();
        animator.Update(0f);
    }
    
    public virtual void StopInteraction()
    {
        controlledByTimeLine = true;
        animator.enabled = false;
    }
}
