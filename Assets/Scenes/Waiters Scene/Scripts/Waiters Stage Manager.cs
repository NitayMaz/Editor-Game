using UnityEngine;

public class WaitersStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;
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
        base.StageFailed();
        stageFailed = true;
    }

    public override void TimeLineDone()
    {
        if (!stageFailed)
        {
            Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
            foreach (var waiter in waiters)
            {
                waiter.StageSuccess();
            }
        }
    }
    
    

}
