using UnityEngine;

public class ClipHandle : MonoBehaviour
{
    [SerializeField] private TrackClip parentClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float startDragXPosition;
    
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
        startDragXPosition = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition).x;
    }
    
    private void OnMouseUp()
    {
        if(MyCursor.Instance != null)
            MyCursor.Instance.SwitchToNormalCursor();
        //push undo action for stretching
        float oldStartDragXPosition = startDragXPosition;
        UndoManager.Push(() =>
        {
            Debug.Log("undoing stretch");
            parentClip.SetDurationByPosition(oldStartDragXPosition);
        });
    }
    
}
