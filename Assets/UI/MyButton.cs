using System;
using UnityEngine;

public enum buttonType
{
    PlayPause,
    Cut,
    Undo,
    Reset,
    OpenPauseMenu
}

public class MyButton : MonoBehaviour
{
    public buttonType buttonType;
    [SerializeField] private Animator animator;

    public void OnMouseDown()
    {
        switch (buttonType)
        {
            case buttonType.PlayPause:
                MyCursor.Instance?.ButtonClicked();
                TimelineUIManager.Instance.PlayPauseButtonClicked();
                break;
            case buttonType.Cut:
                TimelineUIManager.Instance.CutButtonClicked();
                animator.SetTrigger("IsPressed");
                break;
            case buttonType.Undo:
                MyCursor.Instance?.ButtonClicked();
                TimelineUIManager.Instance.UndoButtonClicked();
                animator.SetTrigger("IsPressed");
                break;
            case buttonType.Reset:
                MyCursor.Instance?.ButtonClicked();
                TimelineUIManager.Instance.ResetClicked();
                animator.SetTrigger("IsPressed");
                break;
            case buttonType.OpenPauseMenu:
                MenuUiCordinator.Instance.OpenPauseMenu();
                break;
            default:
                Debug.LogError("Button type not recognized");
                break;
        }

        SoundManager.Instance?.PlayAudio(AudioClips.MouseClick);
    }
}