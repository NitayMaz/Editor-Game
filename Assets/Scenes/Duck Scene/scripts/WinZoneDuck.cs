using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        StageManager.Instance.StageSuccess();
    }
}
