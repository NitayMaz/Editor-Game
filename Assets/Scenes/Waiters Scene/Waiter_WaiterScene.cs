using System;
using UnityEngine;

public class Waiter_WaiterScene : TrackControlled
{
    [SerializeField] ParticleSystem waiterCollisionEffect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        waiterCollisionEffect.transform.position = transform.position;
        waiterCollisionEffect.Play(); //particle!
        StartInteraction();
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
