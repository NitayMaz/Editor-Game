using System.Collections;
using System.Collections.Generic;
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
    
    [Tooltip("keep snapping in 5s so like either 0.5, 0.1, 0.05, 0.01 etc")]
    public float snappingJump = 0.1f;
    public float minDurationForClip = 0.5f;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple TimeLines, That's exactly how marvel failed.");
            Destroy(this.gameObject);
        }
        Instance = this;
        UndoManager.Clear(); // clear undo actions from previous scenes
        trackLengthFor1Second = (bgSpriteRenderer.bounds.max.x - bgSpriteRenderer.bounds.min.x) / timeLineSeconds;
        leftEdgeXvalue = bgSpriteRenderer.bounds.min.x;
    }

    private void Start()
    {
        InitTracks();
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
            tracks[i].InitTrack(connectedObject, unityTrack);
            i++;
        }
    }

    
    private void ResetTime()
    {
        currentTime = 0;
        PositionPointerHead(leftEdgeXvalue);
        if(StageManager.Instance)
            StageManager.Instance.StageReset();
    }

    public void StopPlaying()
    {
        isPlaying = false;
        if (sceneCoroutine != null)
            StopCoroutine(sceneCoroutine);
        TimelineUIManager.Instance.ChangeToPlayButton();
        //snapping
        currentTime = SnapTo(currentTime, snappingJump);
        MovePointerHeadX(GetXPositionForTime(currentTime));
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
            MovePointerHeadX(GetXPositionForTime(currentTime));
            ApplyTimelinePosition(currentTime);
            yield return null;
        }
        //make sure it is at the final position at the end(since it would likely miss by a little)
        currentTime = maxTrackLength;
        MovePointerHeadX(GetXPositionForTime(currentTime));
        ApplyTimelinePosition(currentTime);
        TimelineUIManager.Instance.ChangeToPlayButton();
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
        
        currentTime = (xValue - leftEdgeXvalue) / trackLengthFor1Second;
        //if we're not currently playing, we want to snap. if we are playing we keep it smooth
        if (!isPlaying)
        {
            currentTime = SnapTo(currentTime, snappingJump);
            xValue = GetXPositionForTime(currentTime);
        }
        
        MovePointerHeadX(xValue);
        ApplyTimelinePosition(currentTime);
    }

    private void MovePointerHeadX(float xValue)
    {
        pointerHead.transform.position = new Vector3(xValue, pointerHead.transform.position.y, pointerHead.transform.position.z);
    }

    public float GetXPositionForTime(float time)
    {
        return leftEdgeXvalue + (time * trackLengthFor1Second);
    }
    
    public float GetTimeForXPosition(float xValue)
    {
        return (xValue - leftEdgeXvalue) / trackLengthFor1Second;
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
    
    public static float SnapTo(float value, float snapValue)
    {
        return Mathf.Round(value / snapValue) * snapValue;
    }
    
    public List<TrackInitData> GetClipsData()
    {
        List<TrackInitData> savedClipsData = new List<TrackInitData>();
        foreach (var track in tracks)
        {
            savedClipsData.Add(track.GetClipsData());
        }

        return savedClipsData;
    }
    
    public void LoadSavedClips(List<TrackInitData> savedTracksData)
    {
        if (savedTracksData.Count != tracks.Length)
        {
            Debug.LogError("Saved clips data does not match the number of tracks in the scene.");
            return;
        }
        //stop playing and delete all current clips
        StopPlaying();
        foreach (var track in tracks)
        {
            track.DeleteAllClips();
        }

        //load saved clips
        for (int i = 0; i < tracks.Length; i++)
        {
            tracks[i].InitClips(savedTracksData[i].clipsInitData);
        }
        
        // Reset time after loading clips
        ResetTime();
    }
}
