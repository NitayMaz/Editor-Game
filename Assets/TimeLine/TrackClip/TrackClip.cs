using System;
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
    private float pace = 1;
    private float durationAtPace1;
    private Track parentTrack;

    [SerializeField] private Transform entireClipTransform;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D collider;
    [SerializeField] private ClipHandle clipHandle;
    private BoxCollider2D clipHandleCollider;
    private SpriteRenderer clipHandleSpriteRenderer;

    [SerializeField] private TextMeshPro text;
    private RectTransform textRectTransform;
    private float textOffsetLeft;
    
    [SerializeField] private bool snapPace = true;
    [SerializeField] private float paceJumps = 0.1f;

    [SerializeField] private float minMovementToDrag = 0.1f;
    private float startDragMouseXPosition;
    private float lastMouseXPosition; //for dragging
    private bool isDragging = false;
    private bool clicked = false;
    
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
        spriteRenderer.color = color;
        

        this.startTime = clipStartTime;
        this.parentTrack = parentTrack;
        this.duration = duration;
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
        //quick bug fix when pulling the handle all the way to the other side:
        if(xPosition < spriteRenderer.bounds.min.x)
        {
            xPosition = spriteRenderer.bounds.min.x;
        }
        
        //make the CENTER of the handle go to the click position to avoid jarring jumps
        float rightedgeX = xPosition + clipHandleSpriteRenderer.bounds.size.x / 2f;
        // x of right edge minus x of left edge divided by length for 1 sec gives us the duration
        float newDuration = (rightedgeX - spriteRenderer.bounds.min.x) / TimeLine.Instance.trackLengthFor1Second;

        //first see that we're not running into the next clip, if so the duration is the maximum that won't run into the next clip
        float newEndTime = startTime + newDuration;
        if (newEndTime > parentTrack.GetNextClipStartTime(this))
        {
            newDuration = parentTrack.GetNextClipStartTime(this) - startTime;
        }

        //pace is how fast we're going, if pace is 2 than the duration would be half the duration on pace one, meaning it is duration*(1/pace)
        float newPace = 1 / (newDuration / durationAtPace1);
        //snap pace by pace jumps
        if(snapPace)
            newPace = Mathf.Ceil(newPace / paceJumps) * paceJumps;
        newPace = Mathf.Clamp(newPace, TimeLine.Instance.FastetAllowedPace, TimeLine.Instance.SlowestAllowedPace);
        pace = newPace;
        SetDurationByParameter(durationAtPace1 * 1 / pace);
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

    private void UpdateTextAndHandle()
    {
        clipHandle.transform.position = new Vector2(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y);

        text.transform.position = new Vector2(spriteRenderer.bounds.max.x - textOffsetLeft, transform.position.y);
        text.text = "X" + pace.ToString("0.##"); //show up to 2 decimal spots, don't irrelevant 0s
        text.ForceMeshUpdate();
        //only show the text if the clip is long enough
        text.enabled = (spriteRenderer.bounds.size.x > textRectTransform.rect.width + textOffsetLeft / 2);
    }

    private void UpdateCollider()
    {
        float handleColliderWidth = clipHandleCollider.bounds.size.x;
        float colliderWidth = width - handleColliderWidth;
        collider.size = new Vector2(colliderWidth, collider.size.y);
        collider.offset = new Vector2(colliderWidth / 2f, 0f);
    }

    private void ClipDragged(float deltaX)
    {
        //first clamp so we're not overrunning neighboring clips/out of the timeline
        float newStartTime = startTime + deltaX / TimeLine.Instance.trackLengthFor1Second;
        if (newStartTime < parentTrack.GetPreviousClipEndTime(this))
        {
            deltaX = -1 * (startTime - parentTrack.GetPreviousClipEndTime(this)) *
                     TimeLine.Instance.trackLengthFor1Second;
        }

        float newEndTime = GetEndTime() + deltaX / TimeLine.Instance.trackLengthFor1Second;
        if (newEndTime > parentTrack.GetNextClipStartTime(this))
        {
            deltaX = (parentTrack.GetNextClipStartTime(this) - GetEndTime()) * TimeLine.Instance.trackLengthFor1Second;
        }

        entireClipTransform.position += new Vector3(deltaX, 0f, 0f);
        startTime += deltaX / TimeLine.Instance.trackLengthFor1Second;
    }

    public bool IsActive(float currentTime)
    {
        return currentTime >= startTime && currentTime < GetEndTime();
    }

    public float GetEndTime()
    {
        return startTime + duration;
    }

    public float GetAnimationSpot(float currentTime)
    {
        float clipPercentPassed = (currentTime - startTime) / duration;
        float lerpedVal = Mathf.Lerp(clipAnimationStartPoint, clipAnimationEndPoint, clipPercentPassed);
        lerpedVal = Mathf.Clamp(lerpedVal, clipAnimationStartPoint, clipAnimationEndPoint);
        if (lerpedVal > 0 && lerpedVal % 1f == 0f)
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

    private void OnMouseDown()
    {
        if (TimeLine.Instance.isPlaying)
            return;
        clicked = true;
        startDragMouseXPosition = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition).x;
        lastMouseXPosition = startDragMouseXPosition;
    }

    private void OnMouseDrag()
    {
        if (TimeLine.Instance.isPlaying || !clicked)
            return;
        
        //so you basically have to go over a certain distance for it to count as dragging to differentiate it from clicking
        float mouseXPosition = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition).x;
        if (!isDragging && Mathf.Abs(mouseXPosition - startDragMouseXPosition) >= minMovementToDrag)
        {
            //here we switch to dragging mode
            isDragging = true;
            if(MyCursor.Instance != null)
                MyCursor.Instance.SwitchToHoldingCursor();
        }

        if (!isDragging)
        {
            return;
        }
        //and here we're actually dragging
        float deltaX = mouseXPosition - lastMouseXPosition;
        ClipDragged(deltaX);
        TimeLine.Instance.ClipUpdated();
        lastMouseXPosition = mouseXPosition;
    }

    private void OnMouseUp()
    {
        if (isDragging && MyCursor.Instance != null)
            MyCursor.Instance.SwitchToNormalCursor();
        clicked = false;
        isDragging = false;
    }
}