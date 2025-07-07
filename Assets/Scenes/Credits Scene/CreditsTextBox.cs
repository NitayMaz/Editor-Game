using UnityEngine;

public class CreditsTextBox : TrackControlled
{
    public bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        if (other.CompareTag("TextOverlapArea"))
        {
            isActive = true; 
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying || other.CompareTag("TextOverlapArea"))
            return;
        if(!isActive || !other.GetComponent<CreditsTextBox>().isActive)
            return;
        StageManager.Instance.StageFailed();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        if (other.CompareTag("TextOverlapArea"))
        {
            isActive = false;
        }
    }


    public override void StopInteraction()
    {
        base.StopInteraction();
        isActive = false;
    }
}
