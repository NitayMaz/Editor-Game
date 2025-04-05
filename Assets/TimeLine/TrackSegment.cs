using System;
using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    [HideInInspector]public float startTime;
    [HideInInspector]public float endTime;
    public float duration = 1f;
    public float segmentAnimationStartPoint = 0f;
    public float segmentAnimationEndPoint = 1f;
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SegmentHandle segmentHandle;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Color color, float duration, float startTime, float endTime)
    {
        spriteRenderer.color = color;
        SetDurationByParameter(duration);
        this.startTime = startTime;
        this.endTime = endTime;
    }
    
    private void SetDurationByParameter(float newDuration)
    {
        duration = newDuration;
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float currentWidth = spriteWidth * (transform.lossyScale.x/transform.localScale.x);
        float newWidth = duration * TimeLine.Instance.trackLengthFor1Second;
        Vector2 scale = transform.localScale;
        scale.x = newWidth / currentWidth;
        transform.localScale = scale;
    }

    public void SetDurationByPosition(float xPosition)
    {
        // x of right edge minus x of left edge divided by length for 1 sec gives us the duration
        float newDuration = (xPosition - spriteRenderer.bounds.min.x) / TimeLine.Instance.trackLengthFor1Second;
        float newPace = CalculatePace(newDuration);
        if(newPace < TimeLine.Instance.minSegmentPace || newPace > TimeLine.Instance.maxSegmentPace)
        {
            return;
        }
        SetDurationByParameter(newDuration);
        TimeLine.Instance.SegmentStretched();
    }
    
    public bool IsActive(float currentTime)
    {
        return currentTime >= startTime && currentTime < startTime + duration;
    }
    
    public float GetAnimationSpot(float currentTime)
    {
        float segmentPercentPassed = (currentTime - startTime) / duration;
        return Mathf.Lerp(segmentAnimationStartPoint, segmentAnimationEndPoint, segmentPercentPassed);
    }

    private float CalculatePace(float segmentDuration)
    {
        return (segmentAnimationEndPoint - segmentAnimationStartPoint) / segmentDuration;
    }
    
    private void OnMouseEnter()
    {
        segmentHandle.SegmentStartHover();
    }
    
    private void OnMouseExit()
    {
        segmentHandle.SegmentEndHover();
    }
    
}
