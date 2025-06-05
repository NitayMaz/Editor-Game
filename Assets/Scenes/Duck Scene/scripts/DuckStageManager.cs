using UnityEngine;

public class DuckStageManager : StageManager
{
    public override void StageSuccess()
    {
        base.StageSuccess();
        ShowStageSuccessUI();
    }
}
