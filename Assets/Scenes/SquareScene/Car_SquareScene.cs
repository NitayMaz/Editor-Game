using System;
using UnityEngine;

public class Car_SquareScene : TrackControlled
{
    [SerializeField] ParticleSystem waiterCollisionEffect;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        Car_SquareScene otherCar = other.GetComponent<Car_SquareScene>();
        waiterCollisionEffect.transform.position = transform.position;
        waiterCollisionEffect?.Play(); //particle!
        SoundManager.Instance.PlayAudio(AudioClips.BikeAccident);
        StartInteraction();
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
