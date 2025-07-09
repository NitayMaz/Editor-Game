using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStageButton : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private bool lastStage = false;

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
        if (lastStage)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StageManager.Instance.MoveToNextStage();
        }
    }
}
