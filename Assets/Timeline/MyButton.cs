using System;
using UnityEngine;

public enum buttonType
{
Play,
Stop,
Cut,
Reset
}

public class MyButton : MonoBehaviour
{
    public buttonType buttonType;
    [SerializeField] private Animator animator;
    
    public void OnMouseDown()
    {
        switch (buttonType)
        {
            case buttonType.Play:
                
              UIManager.Instance.PlayClicked();
                break; 
            case buttonType.Stop:
                UIManager.Instance.StopButtonClicked();
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
                UIManager.Instance.PlayClicked();
                break;
        }
    }
}

