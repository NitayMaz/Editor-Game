using UnityEngine;

public class DuckStageManager : StageManager
{
    public override void StageSuccess()
    {
        base.StageSuccess();
        Invoke(nameof(ShowStageSuccessUI), 0.5f);
    }
}
