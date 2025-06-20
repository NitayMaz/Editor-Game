using UnityEngine;

public class RiderJunctionDuck : TrackControlled
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying)
            return;
        other.GetComponent<Duck_DuckScene>()?.StartInteraction();
        StageManager.Instance?.StageFailed();
    }
    
    public override void StartInteraction()
    {
        Vector3 pos = transform.position;
        base.StartInteraction();
        transform.position = pos;
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
