using System;
using UnityEngine;

public class ClickToAdvanceTutorial : MonoBehaviour
{
    private void OnMouseDown()
    {
        if(Tutorial.Instance == null)
        {
            Debug.LogError("How the hell is this on without a tutorial?");
            return;
        }
        Tutorial.Instance.Next();
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
    }
}
