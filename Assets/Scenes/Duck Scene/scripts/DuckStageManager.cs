using UnityEngine;

public class DuckStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;

    public override void OnStagePlay()
    {
        base.OnStagePlay();
        SoundManager.Instance.PlayAudio(AudioClips.CarHonk);
    }

    public override void StageSuccess()
    {
        base.StageSuccess();
        Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }
}
