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
    }


    public override void StartInteraction()
    {
        Vector3 pos = transform.position;
        base.StartInteraction();
        animator.enabled = false;
        transform.position = pos;
        Debug.Log(pos);
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
