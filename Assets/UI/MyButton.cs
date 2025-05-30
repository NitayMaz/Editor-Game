using System;
using UnityEngine;

public enum buttonType
{
    PlayPause,
    Cut,
    Undo
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
            default:
                Debug.LogError("Button type not recognized");
                break;
        }
    }
}