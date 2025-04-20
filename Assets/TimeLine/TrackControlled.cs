using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrackControlled : MonoBehaviour
{
    [SerializeField]private AnimationClip controlledAnimation;
    private Animator animator;
    public bool controlledByTimeLine = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (controlledAnimation == null)
        {
            Debug.LogError("No Animation Clip assigned to the controlled object");
        }
    }

    public void SetAnimationFrame(float time)
    {
        if (!controlledByTimeLine)
            return;
        float animationSecond = controlledAnimation.length * time;
        controlledAnimation.SampleAnimation(this.gameObject, animationSecond);
    }

    public virtual void StartInteraction()
    {
        controlledByTimeLine = false;
        animator.enabled = true;
    }
    
    public virtual void StopInteraction()
    {
        controlledByTimeLine = true;
        animator.enabled = false;
    }
}
