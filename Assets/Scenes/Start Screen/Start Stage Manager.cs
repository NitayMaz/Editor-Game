using UnityEngine;

public class StartStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 0f;
    [SerializeField] private GameObject PlayButtonEffect;

    public override void TimeLineDone()
    {
        Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }
    
    public override void OnStagePlay()
    {
        base.OnStagePlay();
        PlayButtonEffect.SetActive(false);
    }
}