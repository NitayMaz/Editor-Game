using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ClipHandle : MonoBehaviour
{
    [FormerlySerializedAs("parentSegment")] [SerializeField] private TrackClip parentClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool segmentHovered = false;
    private bool handleHovered = false;
    private bool handleClicked = false;

    private void Update()
    {
        if((segmentHovered || handleHovered || handleClicked) && !TimeLine.Instance.isPlaying)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
            spriteRenderer.enabled = false;
        }
    }
    
    private void OnMouseDrag()
    {
        if (TimeLine.Instance.isPlaying)
            return;
        float mousePositionX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        parentClip.SetDurationByPosition(mousePositionX);
    }

    //corresponds to hover method on the segment
    public void SegmentStartHover()
    {
        segmentHovered = true;
    }
    
    public void SegmentEndHover()
    {
        segmentHovered = false;
    }

    private void OnMouseDown()
    {
        handleClicked = true;
    }
    
    private void OnMouseUp()
    {
        handleClicked = false;
    }
    
    private void OnMouseEnter()
    {
        handleHovered = true;
    }
    private void OnMouseExit()
    {
        handleHovered = false;
    }
    
}
