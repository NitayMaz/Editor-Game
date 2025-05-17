using UnityEngine;

public class ClipHandle : MonoBehaviour
{
    [SerializeField] private TrackClip parentClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    
    private void OnMouseDrag()
    {
        if (TimeLine.Instance.isPlaying)
            return;
        float mousePositionX = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition).x;
        parentClip.SetDurationByPosition(mousePositionX);
    }

    private void OnMouseDown()
    {
        if(MyCursor.Instance!=null)
            MyCursor.Instance.SwitchToHoldingCursor();
    }
    
    private void OnMouseUp()
    {
        if(MyCursor.Instance != null)
            MyCursor.Instance.SwitchToNormalCursor();
    }
    
}
