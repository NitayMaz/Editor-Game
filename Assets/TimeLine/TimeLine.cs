using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class TimeLine : MonoBehaviour
{
    public static TimeLine Instance;
    public Camera timeLineCamera;
    [SerializeField] private PlayableDirector timeLineDirector;
    [SerializeField] private Track[] tracks;
    [SerializeField] private GameObject pointerHead;

    public float timeLineSeconds = 5f;
    [HideInInspector]public float trackLengthFor1Second;
    public float FastetAllowedPace = 0.5f;
    public float SlowestAllowedPace = 2f;
    
    private float currentTime = 0;
    public bool isPlaying = false;
    private Coroutine sceneCoroutine;
    
    [SerializeField] private SpriteRenderer bgSpriteRenderer;
    private float leftEdgeXvalue;
    
    private TrackClip selectedClip;
    public float minDurationForClip = 1;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple TimeLines, That's exactly how marvel failed.");
            Destroy(this.gameObject);
        }
        Instance = this;
        trackLengthFor1Second = (bgSpriteRenderer.bounds.max.x - bgSpriteRenderer.bounds.min.x) / timeLineSeconds;
    }

    private void Start()
    {
        InitTracks();
        leftEdgeXvalue = bgSpriteRenderer.bounds.min.x;
        ResetTime();
    }

    private void InitTracks()
    {
        var timeline = timeLineDirector.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("No Timeline assigned to the timeline object in scene!");
            return;
        }
        var UnityTLTracks = timeline.GetOutputTracks();
        int i = 0;
        foreach (var unityTrack in UnityTLTracks)
        {
            if(i>= tracks.Length)
            {
                Debug.LogError("More tracks in timeline than in scene");
                break;
            }
            tracks[i].transform.position = new Vector2(bgSpriteRenderer.bounds.min.x, tracks[i].transform.position.y);
            Animator connectedAnimator = timeLineDirector.GetGenericBinding(unityTrack) as Animator;
            TrackControlled connectedObject = connectedAnimator?.GetComponent<TrackControlled>();
            if (connectedObject == null)
            {
                Debug.LogWarning($"Track {unityTrack.name} has no connected TrackControlled object, skipping...");
                Debug.LogWarning($"getting: {timeLineDirector.GetGenericBinding(unityTrack)}");
                continue;
            }
            Debug.Log($"Initializing track {unityTrack.name} with object {connectedObject.name}");
            tracks[i].InitTrack(connectedObject, unityTrack);
            i++;
        }
    }

    
    public void ResetTime()
    {
        currentTime = 0;
        PositionPointerHead(leftEdgeXvalue);
    }

    public void StopPlaying()
    {
        isPlaying = false;
        if (sceneCoroutine != null)
            StopCoroutine(sceneCoroutine);
        UIManager.Instance.ChangeToPlayButton();
    }
    
    public void PlayScene()
    {
        if(isPlaying)
            return;
        isPlaying = true;
        ResetTime();
        sceneCoroutine = StartCoroutine(RunScene());
    }
    
    private IEnumerator RunScene()
    {
        float maxTrackLength = GetMaxTrackLength();
        while (currentTime < maxTrackLength)
        {
            currentTime += Time.deltaTime;
            MovePointerHeadX(GetTimelinePositionForTime(currentTime));
            ApplyTimelinePosition(currentTime);
            yield return null;
        }
        //make sure it is at the final position at the end(since it would likely miss by a little)
        currentTime = maxTrackLength;
        MovePointerHeadX(GetTimelinePositionForTime(currentTime));
        ApplyTimelinePosition(currentTime);
        UIManager.Instance.ChangeToPlayButton();
        isPlaying = false;
        if(StageManager.Instance)
            StageManager.Instance.TimeLineDone();
    }
    
    private void ApplyTimelinePosition(float time)
    {
        foreach (var track in tracks)
        {
            track.ApplyTrackPosition(time);
        }
    }
    
    public void PositionPointerHead(float xValue)
    {
        //check input validity
        float maxXValue = leftEdgeXvalue + (timeLineSeconds * trackLengthFor1Second);
        if(xValue < leftEdgeXvalue)
            xValue = leftEdgeXvalue;
        if(xValue > maxXValue)
            xValue = maxXValue;
        
        //move the pointer head and change time accordingly
        CancelInteractions(); //returns all objects to being controlled by the timeline
        MovePointerHeadX(xValue);
        // Convert the clicked X position back to time
        currentTime = (xValue - leftEdgeXvalue) / trackLengthFor1Second;
        ApplyTimelinePosition(currentTime);
    }

    private void MovePointerHeadX(float xValue)
    {
        pointerHead.transform.position = new Vector3(xValue, pointerHead.transform.position.y, pointerHead.transform.position.z);
    }

    private float GetTimelinePositionForTime(float time)
    {
        return leftEdgeXvalue + (time * trackLengthFor1Second);
    }
    
    private void CancelInteractions()
    {
        foreach (var track in tracks)
        {
            track.CancelObjectInteraction();
        }
    }

    private float GetMaxTrackLength()
    {
        float maxLength = 0;
        foreach (var track in tracks)
        {
            float trackLength = track.GetTrackDuration();
            if (trackLength > maxLength)
                maxLength = trackLength;
        }
        return maxLength;
    }
    
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ClipUpdated()
    {
        ApplyTimelinePosition(currentTime);
    }

    public void SelectTrackClip(TrackClip clip) // called from clip outline
    {
        selectedClip?.GetComponent<ClipOutline>().ResetOutline();
        selectedClip = clip;
    }

    public void CutSelectedClip()
    {
        if (!selectedClip || !selectedClip.IsActive(currentTime) || isPlaying)
            return;
        selectedClip.CutClip(currentTime);
    }

    public void DeleteSelectedClip()
    {
        selectedClip?.DeleteClip();
        SelectTrackClip(null);
    }
}
