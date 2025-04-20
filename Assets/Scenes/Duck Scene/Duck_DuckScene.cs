using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Duck_DuckScene : TrackControlled
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        //particle!
        StartInteraction();
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        gameObject.SetActive(false);
    }
    
    public override void StopInteraction()
    {
        base.StopInteraction();
        gameObject.SetActive(true);
    }
}
