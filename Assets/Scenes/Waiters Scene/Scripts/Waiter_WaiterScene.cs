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
        Debug.Log(other.name.ToString());
        StartInteraction();
        StageManager.Instance?.StageFailed();
    }


    public override void StartInteraction()
    {
        Vector3 pos = transform.position;
        base.StartInteraction();
        animator.enabled = false;
        transform.position = pos;
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
