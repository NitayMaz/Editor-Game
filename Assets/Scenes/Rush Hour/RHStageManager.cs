using UnityEngine;

public class RHStageManager : StageManager
{
    public override void StageFailed()
    {
        base.StageFailed();
        TimeLine.Instance.StopPlaying();
    }
    
    public override void StageSuccess()
    {
        base.StageSuccess();
        ShowStageSuccessUI();
    }
}
