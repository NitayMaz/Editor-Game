using TMPro;
using UnityEngine;

public class TrackClip : MonoBehaviour
{
    public AnimationClip animationClip;
    [SerializeField] private float clipAnimationStartPoint;
    [SerializeField] private float clipAnimationEndPoint;
    [HideInInspector] public float startTime;
    [HideInInspector] public float width;
    public float duration;
    private float durationMultiplier = 1; //this means how much the current duration is from the original duration
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

    public void Init(AnimationClip animationClip, Color color, float duration, float durationMultiplier, float clipStartTime,
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
        this.durationMultiplier = durationMultiplier;
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
        float originalDuration = duration / durationMultiplier;
        float newDurationMultiplier = newDuration / originalDuration;
        newDurationMultiplier = Mathf.Clamp(newDurationMultiplier, TimeLine.Instance.minDurationMultiplier,
                                                                        TimeLine.Instance.maxDurationMultiplier);
        durationMultiplier = newDurationMultiplier;
        SetDurationByParameter(originalDuration * durationMultiplier);
        TimeLine.Instance.ClipUpdated();
    }
    
    private void SetDurationByParameter(float newDuration)
    {
        duration = newDuration;
        width = duration * TimeLine.Instance.trackLengthFor1Second;
        spriteRenderer.size = new Vector2(width, spriteRenderer.size.y);
        parentTrack.OrganizeClips();
        UpdateTextAndHandle();
        UpdateCollider();
    }

    public void UpdateTextAndHandle()
    {
        clipHandle.transform.position = new Vector2(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y);
        
        text.transform.position = new Vector2(spriteRenderer.bounds.max.x - textOffsetLeft, transform.position.y);
        text.text = "X" + (1f / durationMultiplier).ToString("0.##"); //show up to 2 decimal spots, don't irrelevant 0s
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
        return currentTime >= startTime && currentTime < startTime + duration;
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
            durationMultiplier = durationMultiplier,
            animationStartPoint = clipAnimationStartPoint,
            animationEndPoint = secondPartAnimationStartPoint
        };
        TrackClipInitData secondPartData = new TrackClipInitData
        {
            animationClip = animationClip,
            duration = duration - firstPartDuration,
            durationMultiplier = durationMultiplier,
            animationStartPoint = secondPartAnimationStartPoint,
            animationEndPoint = clipAnimationEndPoint
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