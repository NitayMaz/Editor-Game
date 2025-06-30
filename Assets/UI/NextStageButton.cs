using System;
using UnityEngine;

public class NextStageButton : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void OnMouseOver()
    {
        animator.SetBool("IsHovering", true);
    }
    private void OnMouseExit()
    {
        animator.SetBool("IsHovering", false);
    }

    private void OnMouseDown()
    {
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
        StageManager.Instance.MoveToNextStage();
    }
}
