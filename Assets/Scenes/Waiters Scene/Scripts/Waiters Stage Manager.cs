using UnityEngine;

public class WaitersStageManager : StageManager
{
    [SerializeField] private Waiter_WaiterScene[] waiters;
    private bool stageFailed = false;

    public override void StageReset()
    {
        stageFailed = false;
        foreach (var waiter in waiters)
        {
            waiter.ResetAnimator();
        }
    }

    public override void StageFailed()
    {
        stageFailed = true;
    }

    public override void TimeLineDone()
    {
        if (!stageFailed)
        {
            ShowStageSuccessUI();
            foreach (var waiter in waiters)
            {
                waiter.StageSuccess();
            }
        }
    }
    
    

}
