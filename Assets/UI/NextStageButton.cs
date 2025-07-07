using System;
using System.Collections;
using UnityEngine;

public class NextStageButton : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float fadeOutDuration = 0.5f;

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
        animator.SetTrigger("FadeOut");
        StartCoroutine(MoveToNextStage());
    }
    
    private IEnumerator MoveToNextStage()
    {
        yield return new WaitForSeconds(fadeOutDuration);
        StageManager.Instance.MoveToNextStage();
    }
}
