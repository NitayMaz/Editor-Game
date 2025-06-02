using System;
using UnityEngine;

public class Car_SquareScene : TrackControlled
{
    [SerializeField] ParticleSystem waiterCollisionEffect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        waiterCollisionEffect.transform.position = transform.position;
        waiterCollisionEffect.Play(); //particle!
        StageManager.Instance?.StageFailed();
    }
    

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
