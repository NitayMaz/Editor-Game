using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    [SerializeField] private ClipHandle clipHandle;

    [SerializeField] private TextMeshPro text;
    [SerializeField] [Range(0, 1)] private float textHeight;
    private float textOffsetLeft;
    private float originaltextWidth;

    public bool applyColor = true;
    private float clipHeight;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(AnimationClip animationClip, Color color, float duration, float durationMultiplier, float clipStartTime,
        float animationStartPoint, float animationEndPoint, float clipHeight, Track parentTrack)
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
        this.clipHeight = clipHeight;
        clipAnimationStartPoint = animationStartPoint;
        clipAnimationEndPoint = animationEndPoint;
        SetHeight(clipHeight);
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
        clipHandle.transform.localScale = handleScale;
    }
    private void InitalizeTextHeight()
    {
        text.ForceMeshUpdate();
        float targetHeight = textHeight * clipHeight;
        float currentHeight = text.bounds.size.y;
        text.transform.localScale *= targetHeight / currentHeight;
        textOffsetLeft = spriteRenderer.bounds.max.x - text.transform.position.x;
        text.ForceMeshUpdate(); 
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
        parentTrack.OrganizeClips();
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
        TimeLine.Instance.ClipUpdated();
    }

    public void UpdateTextAndHandle()
    {
        clipHandle.transform.position = new Vector3(spriteRenderer.bounds.max.x, spriteRenderer.transform.position.y,
            spriteRenderer.transform.position.z);
        text.ForceMeshUpdate();
        text.transform.position = new Vector2(spriteRenderer.bounds.max.x - textOffsetLeft, transform.position.y);
        text.text = "X" + (1f / durationMultiplier).ToString("0.##"); //show up to 2 decimal spots, don't irrelevant 0s
        //only show the text if the clip is long enough
        Debug.Log($"spriteRenderer.bounds.size.x: {spriteRenderer.bounds.size.x}, text.bounds.size.x: {text.bounds.size.x}");
        text.enabled = (spriteRenderer.bounds.size.x > text.bounds.size.x);
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