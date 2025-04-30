using UnityEngine;

public class ClipHandle : MonoBehaviour
{
    [SerializeField] private TrackClip parentClip;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool clipHovered = false;
    private bool handleHovered = false;
    private bool handleClicked = false;

    private void Update()
    {
        if((clipHovered || handleHovered || handleClicked) && !TimeLine.Instance.isPlaying)
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

    //corresponds to hover method on the clip
    public void ClipStartHover()
    {
        clipHovered = true;
    }
    
    public void ClipEndHover()
    {
        clipHovered = false;
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
