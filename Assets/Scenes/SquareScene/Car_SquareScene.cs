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
        StartInteraction();
        //can hit both duck and other rider
        other.GetComponent<Car_SquareScene>()?.StartInteraction();
        other.GetComponent<Duck_DuckScene>()?.StartInteraction();
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
}
