using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Logo : TrackControlled
{
    private void Start()
    {
        controlledByTimeLine = false;
    }

    public override void SetAnimationFrame(AnimationClip animationClip, float time)
    {
        if (time != 0 && time != 1)
        {
            controlledByTimeLine = true;
            animator.enabled = false;
        }
        base.SetAnimationFrame(animationClip, time);
    }
    
    public override void StopInteraction()
    {
        return;
    }

    
    
    /*
     * public virtual void SetAnimationFrame(AnimationClip animationClip, float time)
    {
        if (!controlledByTimeLine)
            return;
        float animationSecond = animationClip.length * time;
        animationClip.SampleAnimation(gameObject, animationSecond);
    }

    public virtual void StartInteraction()
    {
        if (!controlledByTimeLine)
            return;
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
     */
}
