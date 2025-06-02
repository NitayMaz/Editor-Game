using UnityEngine;

public class SquareStageManager : StageManager
{
    private bool stageFailed = false;

    public override void StageReset()
    {
        stageFailed = false;
    }

    public override void StageFailed()
    {
        stageFailed = true;
        TimeLine.Instance.StopPlaying();
    }

    public override void TimeLineDone()
    {
        if(!stageFailed)
            ShowStageSuccessUI();
    }
    
    

}
