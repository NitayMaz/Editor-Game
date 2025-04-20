using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TrackClip : MonoBehaviour
{
    [HideInInspector] public float startTime;
    [HideInInspector] public float width;
    public float duration;
    private float originalDuration;
    [SerializeField] private float segmentAnimationStartPoint;
    [SerializeField] private float segmentAnimationEndPoint;
    private Track parentTrack;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("segmentHandle")] [SerializeField] private ClipHandle clipHandle;
    private Vector2 handleOriginalScale;

    public bool applyColor = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        handleOriginalScale = clipHandle.transform.lossyScale;
    }

    public void Init(Color color, float initialDuration, float segmentStartTime,
        float animationStartPoint, float animationEndPoint, float segmentHeight, Track parentTrack)
    {
        if (applyColor)
        {
            color.a = Random.Range(0.5f, 1f); //temporary solution to tell segments apart
            spriteRenderer.color = color;
        }

        this.startTime = segmentStartTime;
        this.parentTrack = parentTrack;
        this.originalDuration = initialDuration;
        this.duration = initialDuration;
        segmentAnimationStartPoint = animationStartPoint;
        segmentAnimationEndPoint = animationEndPoint;
        SetHeight(segmentHeight);
        SetDurationByParameter(duration);
    }

    private void SetHeight(float newHeight)
    {
        Vector2 scale = transform.localScale;
        scale.y = newHeight / spriteRenderer.sprite.bounds.size.y;
        transform.localScale = scale;
        Vector2 handleScale = clipHandle.transform.localScale;
        handleScale.y = scale.y;
    }

    private void SetDurationByParameter(float newDuration)
    {
        duration = newDuration;
        float spriteWidth = spriteRenderer.sprite.bounds.size.x;
        float currentWidth = spriteWidth * (transform.lossyScale.x / transform.localScale.x);
        width = duration * TimeLine.Instance.trackLengthFor1Second;
        Vector2 scale = transform.localScale;
        scale.x = width / currentWidth;
        transform.localScale = scale;
        parentTrack.OrganizeSegments();
        PositionHandle();
    }

    public void SetDurationByPosition(float xPosition)
    {
        // x of right edge minus x of left edge divided by length for 1 sec gives us the duration
        float newDuration = (xPosition - spriteRenderer.bounds.min.x) / TimeLine.Instance.trackLengthFor1Second;
        if (newDuration < TimeLine.Instance.minDurationMultiplier * originalDuration)
            newDuration = TimeLine.Instance.minDurationMultiplier * originalDuration;
        if (newDuration > TimeLine.Instance.maxDurationMultiplier * originalDuration)
            newDuration = TimeLine.Instance.maxDurationMultiplier * originalDuration;
        SetDurationByParameter(newDuration);
        TimeLine.Instance.SegmentStretched();
    }

    private void PositionHandle()
    {
        clipHandle.transform.position = new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y,
            spriteRenderer.transform.position.z);
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

    public void CutSegment(float currentTime)
    {
        float firstPartDuration = currentTime - startTime;
        if (firstPartDuration < TimeLine.Instance.minDurationForSegment ||
            duration - firstPartDuration < TimeLine.Instance.minDurationForSegment)
        {
            Debug.Log("Segment is too short to cut, you can change it under the timeline object");
            TimeLine.Instance.SelectTrackSegment(null);
            return;
        }

        float secondPartAnimationStartPoint =
            segmentAnimationStartPoint + (segmentAnimationEndPoint - segmentAnimationStartPoint) *
            (firstPartDuration / duration);
        TrackSegmentInitData firstPartData = new TrackSegmentInitData
        {
            duration = firstPartDuration,
            animationStartPoint = segmentAnimationStartPoint,
            animationEndPoint = secondPartAnimationStartPoint
        };
        TrackSegmentInitData secondPartData = new TrackSegmentInitData
        {
            duration = duration - firstPartDuration,
            animationStartPoint = secondPartAnimationStartPoint,
            animationEndPoint = segmentAnimationEndPoint
        };
        parentTrack.ReplaceCutSegment(this, firstPartData, secondPartData);
    }

    public void DeleteSegment()
    {
        parentTrack.DeleteSelectedSegment(this);
    }

    private void OnMouseEnter()
    {
        clipHandle.SegmentStartHover();
    }

    private void OnMouseExit()
    {
        clipHandle.SegmentEndHover();
    }
}