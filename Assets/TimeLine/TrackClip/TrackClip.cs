using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TrackClip : MonoBehaviour
{
    [HideInInspector] public float startTime;
    [HideInInspector] public float width;
    public float duration;
    private float durationMultiplier = 1; //this means how much the current duration is from the original duration
    [SerializeField] private float segmentAnimationStartPoint;
    [SerializeField] private float segmentAnimationEndPoint;
    private Track parentTrack;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [FormerlySerializedAs("segmentHandle")] [SerializeField] private ClipHandle clipHandle;

    [SerializeField] private TextMeshPro text;
    [SerializeField] [Range(0, 1)] private float textHeight;
    private float originaltextWidth;

    public bool applyColor = true;
    private float segmentHeight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(Color color, float duration, float durationMultiplier, float segmentStartTime,
        float animationStartPoint, float animationEndPoint, float segmentHeight, Track parentTrack)
    {
        if (applyColor)
        {
            color.a = Random.Range(0.5f, 1f); //temporary solution to tell segments apart
            spriteRenderer.color = color;
        }

        this.startTime = segmentStartTime;
        this.parentTrack = parentTrack;
        this.duration = duration;
        this.durationMultiplier = durationMultiplier;
        this.segmentHeight = segmentHeight;
        segmentAnimationStartPoint = animationStartPoint;
        segmentAnimationEndPoint = animationEndPoint;
        SetHeight(segmentHeight);
        InitalizeTextHeight();
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
        UpdateTextAndHandle();
    }

    public void SetDurationByPosition(float xPosition)
    {
        // x of right edge minus x of left edge divided by length for 1 sec gives us the duration
        float newDuration = (xPosition - spriteRenderer.bounds.min.x) / TimeLine.Instance.trackLengthFor1Second;
        float originalDuration = duration / durationMultiplier;
        float newDurationMultiplier = newDuration / originalDuration;
        newDurationMultiplier = Mathf.Clamp(newDurationMultiplier, TimeLine.Instance.minDurationMultiplier,
                                                                        TimeLine.Instance.maxDurationMultiplier);
        durationMultiplier = newDurationMultiplier;
        SetDurationByParameter(originalDuration * durationMultiplier);
        TimeLine.Instance.SegmentStretched();
    }

    public void UpdateTextAndHandle()
    {
        clipHandle.transform.position = new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y,
            spriteRenderer.transform.position.z);
        text.ForceMeshUpdate();
        text.transform.position = spriteRenderer.bounds.center;
        text.text = "X" + durationMultiplier.ToString("0.##"); //show up to 2 decimal spots, don't irrelevant 0s
        //only show the text if the segment is long enough
        text.enabled = spriteRenderer.bounds.size.x > text.bounds.size.x;
    }

    private void InitalizeTextHeight()
    {
        text.ForceMeshUpdate();
        float targetHeight = textHeight * segmentHeight;
        float currentHeight = text.bounds.size.y;
        text.transform.localScale *= targetHeight / currentHeight;
        text.ForceMeshUpdate(); 
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
            durationMultiplier = durationMultiplier,
            animationStartPoint = segmentAnimationStartPoint,
            animationEndPoint = secondPartAnimationStartPoint
        };
        TrackSegmentInitData secondPartData = new TrackSegmentInitData
        {
            duration = duration - firstPartDuration,
            durationMultiplier = durationMultiplier,
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