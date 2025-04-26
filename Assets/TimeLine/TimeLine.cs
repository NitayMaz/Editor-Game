using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeLine : MonoBehaviour
{
    public static TimeLine Instance;
    [SerializeField] private Track[] tracks;
    [SerializeField] private GameObject pointerHead;
    
    public float trackLengthFor1Second = 5f;
    public float minDurationMultiplier = 0.5f;
    public float maxDurationMultiplier = 1.5f;
    
    private float currentTime = 0;
    private float maxTrackLength = 0;
    public bool isPlaying = false;
    private Coroutine sceneCoroutine;
    
    [SerializeField] private SpriteRenderer BGSpriteRenderer;
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
    }

    private void Start()
    {
        leftEdgeXvalue = BGSpriteRenderer.bounds.min.x;
        UpdateMaxTrackLength();
        ResetTime();
    }
    
    private void UpdateMaxTrackLength()
    {
        maxTrackLength = 0;
        foreach (var track in tracks)
        {
            float trackLength = track.GetTrackDuration();
            if (trackLength > maxTrackLength)
                maxTrackLength = trackLength;
        }
        if (currentTime > maxTrackLength)
        {
            currentTime = maxTrackLength;
            MovePointerHeadX(leftEdgeXvalue + (currentTime * trackLengthFor1Second));
        }
    }

    
    private void ResetTime()
    {
        currentTime = 0;
        PositionPointerHead(leftEdgeXvalue);
    }

    public void StopPlaying()
    {
        isPlaying = false;
        if (sceneCoroutine != null)
            StopCoroutine(sceneCoroutine);
        ResetTime();
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
        while (currentTime < maxTrackLength)
        {
            currentTime += Time.deltaTime;
            MovePointerHeadX(leftEdgeXvalue + (currentTime * trackLengthFor1Second));
            ApplyTimelinePosition(currentTime);
            yield return null;
        }
        //make sure it is at the final position at the end(since it would likely miss by a little)
        currentTime = maxTrackLength;
        MovePointerHeadX(leftEdgeXvalue + (currentTime * trackLengthFor1Second));
        ApplyTimelinePosition(currentTime);
        isPlaying = false;
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
        float maxXValue = leftEdgeXvalue + (maxTrackLength * trackLengthFor1Second);
        if(xValue < leftEdgeXvalue)
            return;
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

    private void CancelInteractions()
    {
        foreach (var track in tracks)
        {
            track.CancelObjectInteraction();
        }
    }
    
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void ClipUpdated()
    {
        UpdateMaxTrackLength();
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
