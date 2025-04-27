using UnityEngine;


public class SetTriggerAnimator : MonoBehaviour
{

    public Animator anim;
    public bool isButtonStopped;
    
    public void OnMouseDown()
    {
        if (!isButtonStopped)
        {
            anim.SetBool("IsStopped", true);
            isButtonStopped = true;
            UIManager.Instance.StopButtonClicked();
            
        }
        else //(isButtonStopped == true)
        {
            anim.SetBool("IsStopped", false);
            isButtonStopped = false;
            UIManager.Instance.PlayClicked();
        }
    }

    // set bool isStopped = true ->  stopped is played
    //change the bool isButtonStopped to true (it is in pause now)
    //if pressed again -> change the bool isButtonStopped to false and set bool isStopped to false
}
