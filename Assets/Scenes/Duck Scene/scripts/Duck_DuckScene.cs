using System;
using UnityEngine;

public class Duck_DuckScene : TrackControlled
{
    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] private bool isDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        if (other.gameObject.CompareTag("WinArea"))
        {
            // StageManager.Instance.StageSuccess();
            StartInteraction();
            transform.GetComponent<Animator>().SetBool("PassedRoad", true);
            SoundManager.Instance.PlayAudio(AudioClips.DuckQuack);
            StageManager.Instance.StageSuccess();
            return;
        }
        if (isDead) // to avoid second collision when moving transform
            return;
        
        
        //collision with car/bike
        duckExplodes.transform.position = transform.position;

        duckExplodes.Play(); //particle!
        isDead = true;
        StartInteraction();
    }

    public override void StartInteraction()
    {
        if(isDead)
            StageManager.Instance.StageFailed();
        Vector3 lastPosition = transform.position;
        base.StartInteraction(); // turns on animator, resets position to start
        transform.position = lastPosition; // recover last position
    }
    

    public override void StopInteraction()
    {
        base.StopInteraction();
        isDead = false;
        transform.GetComponent<Animator>().SetBool("PassedRoad", false);
    }
}