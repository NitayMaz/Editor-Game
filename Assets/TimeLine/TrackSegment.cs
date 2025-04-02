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

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        Init(spriteRenderer.color, duration, startTime, endTime);
    }

    public void Init(Color color, float duration, float startTime, float endTime)
    {
        spriteRenderer.color = color;
        SetDuration(duration);
        this.startTime = startTime;
        this.endTime = endTime;
    }
    
    private void SetDuration(float newDuration)
    {
        duration = newDuration;
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float currentWidth = spriteWidth * (transform.lossyScale.x/transform.localScale.x);
        float newWidth = duration * TimeLine.Instance.trackLengthFor1Second;
        Vector2 scale = transform.localScale;
        scale.x = newWidth / currentWidth;
        transform.localScale = scale;
        
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
}
