using UnityEngine;

public class SoccerStageManager : StageManager
{
    [SerializeField] private Rigidbody2D ballRigidbody;
    
    public override void StageSuccess()
    {
        Debug.Log("Stage Success");
        ballRigidbody.linearDamping = 2f;
        base.StageSuccess();
        ShowStageSuccessUI();
    }
}
