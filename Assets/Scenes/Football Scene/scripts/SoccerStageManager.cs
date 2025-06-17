using UnityEngine;

public class SoccerStageManager : StageManager
{
    [SerializeField] private Rigidbody2D ballRigidbody;
    [SerializeField] private SoccerKid player;

    public override void StageReset()
    {
        base.StageReset();
        // ballRigidbody.linearDamping = 0f;
    }

    public override void StageSuccess()
    {
        // ballRigidbody.linearDamping = 2f;
        player.Goal();
        base.StageSuccess();
        Invoke(nameof(ShowStageSuccessUI), 2f);
    }
}
