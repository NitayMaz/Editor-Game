using UnityEngine;

public class DuckStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;
    public override void StageSuccess()
    {
        base.StageSuccess();
        Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }
}
