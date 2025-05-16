using System;
using UnityEngine;

public enum buttonType
{
    PlayPause,
    Cut,
    Reset
}

public class MyButton : MonoBehaviour
{
    public buttonType buttonType;
    [SerializeField] private Animator animator;

    public void OnMouseDown()
    {
        MyCursor.Instance?.ButtonClicked();
        switch (buttonType)
        {
            case buttonType.PlayPause:
                UIManager.Instance.PlayPauseButtonClicked();
                break;
            case buttonType.Cut:
                UIManager.Instance.CutButtonClicked();
                animator.SetTrigger("IsPressed");
                break;
            case buttonType.Reset:
                UIManager.Instance.ResetClicked();
                animator.SetTrigger("IsPressed");
                break;
            default:
                Debug.LogError("Button type not recognized");
                break;
        }
    }
}