using TMPro;
using UnityEngine;

public class TrackClip : MonoBehaviour
{
    public AnimationClip animationClip;
    [SerializeField] private float clipAnimationStartPoint;
    [SerializeField] private float clipAnimationEndPoint;
    [HideInInspector] public float startTime;
    [HideInInspector] public float endTime;
    [HideInInspector] public float width;
    public float duration;
    private float pace = 1;
    private float durationAtPace1;
    private Track parentTrack;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;
    [SerializeField] private ClipHandle clipHandle;
    private BoxCollider2D clipHandleCollider;
    private SpriteRenderer clipHandleSpriteRenderer;

    [SerializeField] private TextMeshPro text;
    private RectTransform textRectTransform;
    private float textOffsetLeft;

    public bool applyColor = true;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        clipHandleCollider = clipHandle.GetComponent<BoxCollider2D>();
        clipHandleSpriteRenderer = clipHandle.GetComponent<SpriteRenderer>();
    }

    public void Init(AnimationClip animationClip, Color color, float duration, float pace, float clipStartTime,
        float animationStartPoint, float animationEndPoint, Track parentTrack)
    {
        this.animationClip = animationClip;
        if (applyColor)
        {
            spriteRenderer.color = color;
        }

        this.startTime = clipStartTime;
        this.parentTrack = parentTrack;
        this.duration = duration;
        this.endTime = clipStartTime + duration;
        this.pace = pace;
        durationAtPace1 = duration / pace;
        clipAnimationStartPoint = animationStartPoint;
        clipAnimationEndPoint = animationEndPoint;
        textOffsetLeft = spriteRenderer.bounds.max.x - text.transform.position.x;
        textRectTransform = text.GetComponent<RectTransform>();
        SetDurationByParameter(duration);
    }

    public void SetDurationByPosition(float xPosition)
    {
        //make the CENTER of the handle go to the click position to avoid jarring jumps
        float rightedgeX = xPosition + clipHandleSpriteRenderer.bounds.size.x / 2f; 
        // x of right edge minus x of left edge divided by length for 1 sec gives us the duration
        float newDuration = (rightedgeX - spriteRenderer.bounds.min.x) / TimeLine.Instance.trackLengthFor1Second;
        
        //first see that we're not running into the next clip, if so the duration is the maximum that won't run into the next clip
        float newEndTime = startTime + newDuration;
        if(newEndTime > parentTrack.GetNextClipStartTime(this))
        {
            newDuration = parentTrack.GetNextClipStartTime(this) - startTime;
        }
        
        //pace is how fast we're going, if pace is 2 than the duration would be half the duration on pace one, meaning it is duration*(1/pace)
        float newPace = 1 / (newDuration / durationAtPace1); 
        newPace = Mathf.Clamp(newPace, TimeLine.Instance.FastetAllowedPace, TimeLine.Instance.SlowestAllowedPace);
        pace = newPace;
        SetDurationByParameter(durationAtPace1 * 1/pace);
        TimeLine.Instance.ClipUpdated();
    }
    
    private void SetDurationByParameter(float newDuration)
    {
        duration = newDuration;
        width = duration * TimeLine.Instance.trackLengthFor1Second;
        spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
        UpdateTextAndHandle();
        UpdateCollider();
        TimeLine.Instance.ClipUpdated();
    }

    public void UpdateTextAndHandle()
    {
        clipHandle.transform.position = new Vector2(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y);
        
        text.transform.position = new Vector2(spriteRenderer.bounds.max.x - textOffsetLeft, transform.position.y);
        text.text = "X" + pace.ToString("0.##"); //show up to 2 decimal spots, don't irrelevant 0s
        text.ForceMeshUpdate();
        //only show the text if the clip is long enough
        text.enabled = (spriteRenderer.bounds.size.x > textRectTransform.rect.width + textOffsetLeft/2);
    }

    private void UpdateCollider()
    {
        float handleColliderWidth = clipHandleCollider.bounds.size.x;
        float colliderWidth = width - handleColliderWidth;
        collider.size = new Vector2(colliderWidth, collider.size.y);
        collider.offset = new Vector2(colliderWidth/2f, 0f);
    }

    public bool IsActive(float currentTime)
    {
        return currentTime >= startTime && currentTime < endTime;
    }

    public float GetAnimationSpot(float currentTime)
    {
        float clipPercentPassed = (currentTime - startTime) / duration;
        float lerpedVal = Mathf.Lerp(clipAnimationStartPoint, clipAnimationEndPoint, clipPercentPassed);
        if (lerpedVal <= 0)
        {
            return 0;
        }

        if (lerpedVal % 1f == 0f)
        {
            return 1f; //without this it goes back to the first frame after finishing the animation
        }

        return lerpedVal % 1f;
    }

    public void CutClip(float currentTime)
    {
        float firstPartDuration = currentTime - startTime;
        if (firstPartDuration < TimeLine.Instance.minDurationForClip ||
            duration - firstPartDuration < TimeLine.Instance.minDurationForClip)
        {
            Debug.Log("clip is too short to cut, you can change it under the timeline object");
            TimeLine.Instance.SelectTrackClip(null);
            return;
        }

        float secondPartAnimationStartPoint =
            clipAnimationStartPoint + (clipAnimationEndPoint - clipAnimationStartPoint) *
            (firstPartDuration / duration);
        TrackClipInitData firstPartData = new TrackClipInitData
        {
            animationClip = animationClip,
            duration = firstPartDuration,
            pace = pace,
            animationStartPoint = clipAnimationStartPoint,
            animationEndPoint = secondPartAnimationStartPoint,
            startTime = startTime,
        };
        TrackClipInitData secondPartData = new TrackClipInitData
        {
            animationClip = animationClip,
            duration = duration - firstPartDuration,
            pace = pace,
            animationStartPoint = secondPartAnimationStartPoint,
            animationEndPoint = clipAnimationEndPoint,
            startTime = startTime + firstPartDuration,
        };
        parentTrack.ReplaceCutClip(this, firstPartData, secondPartData);
    }

    public void DeleteClip()
    {
        parentTrack.DeleteSelectedClip(this);
    }

    private void OnMouseEnter()
    {
        clipHandle.ClipStartHover();
    }

    private void OnMouseExit()
    {
        clipHandle.ClipEndHover();
    }
}