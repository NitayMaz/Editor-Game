using System;
using UnityEngine;

public class Duck_DuckScene : TrackControlled
{
    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] private Vector2 lastPosition;
    [SerializeField] private bool isDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        if (other.gameObject.CompareTag("WinArea"))
            return;
        if (isDead) // to avoid second collision when moving transform
            return;
        duckExplodes.transform.position = transform.position;
        lastPosition = transform.position;

        duckExplodes.Play(); //particle!
        isDead = true;
        StartInteraction();
    }

    public override void StartInteraction()
    {
        base.StartInteraction(); // turns on animator, resets position to start
        transform.position = lastPosition; // recover last position
    }
    

    public override void StopInteraction()
    {
        base.StopInteraction();
        isDead = false;
    }
}