using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrackControlled : MonoBehaviour
{
    [SerializeField]private AnimationClip controlledAnimation;
    

    public void SetAnimationFrame(float time)
    {
        float animationSecond = controlledAnimation.length * time;
        controlledAnimation.SampleAnimation(this.gameObject, animationSecond);
    }
}
