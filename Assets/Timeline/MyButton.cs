using System;
using UnityEngine;

public enum buttonType
{
Play,
Stop,
Cut,
Rerun
}

public class MyButton : MonoBehaviour
{
    public buttonType buttonType;
    
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
                break;
            case buttonType.Rerun:
                UIManager.Instance.ResetClicked();
                break;
            default:
                UIManager.Instance.PlayClicked();
                break;
        }
    }
}

