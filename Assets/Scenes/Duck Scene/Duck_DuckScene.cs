using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Duck_DuckScene : TrackControlled
{
    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] Vector2 duckExplodesOffset;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name.ToString());
        if(!TimeLine.Instance.isPlaying)
            return;
        duckExplodes.transform.position = transform.position + (Vector3)duckExplodesOffset;

        duckExplodes.Play(); //particle!
       
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
