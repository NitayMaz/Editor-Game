using UnityEngine;

public class SquareStageManager : StageManager
{
    private bool stageFailed = false;
    [SerializeField] private float timeToShowSuccessUI = 0f;

    public override void StageReset()
    {
        stageFailed = false;
    }

    public override void StageFailed()
    {
        stageFailed = true;
    }

    public override void TimeLineDone()
    {
        if(!stageFailed)
            Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }
    
    

}
