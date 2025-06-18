using System;
using UnityEngine;

public class Waiter_WaiterScene : TrackControlled
{
    [SerializeField] ParticleSystem waiterCollisionEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        waiterCollisionEffect.transform.position = transform.position;
        waiterCollisionEffect.Play(); //particle!
        StartInteraction();
        other.GetComponent<Waiter_WaiterScene>().StartInteraction();
        StageManager.Instance?.StageFailed();
    }


    public override void StartInteraction()
    {
        Vector3 pos = transform.position;
        base.StartInteraction();
        transform.position = pos;
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
    
    public void ResetAnimator()
    {
        animator.SetBool("Success", false);
    }
    
    public void StageSuccess()
    {
        StartInteraction();
        animator.SetBool("Success", true);
    }
}
