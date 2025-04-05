using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PianoSceneControlled : TrackControlled
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        if (other.CompareTag("Player"))
        {
            TurnAnimatorOn();
        }
        if (other.CompareTag("Piano"))
        {
            SceneManager.LoadScene(0);
        }
    }
}
