using System;
using System.Collections;
using UnityEngine;

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
    
    private SpriteRenderer spriteRenderer;
    private float leftEdgeXvalue;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple TimeLines, That's exactly how marvel failed.");
            Destroy(this.gameObject);
        }
        Instance = this;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        leftEdgeXvalue = spriteRenderer.bounds.min.x;
        UpdateMaxTrackLength();
        ResetTimeLine();
    }

    public void StopPlaying()
    {
        isPlaying = false;
        if (sceneCoroutine != null)
            StopCoroutine(sceneCoroutine);
        ResetTimeLine();
    }
    
    public void ResetTimeLine()
    {
        currentTime = 0;
        PositionPointerHead(leftEdgeXvalue);
    }
    
    public void PlayScene()
    {
        if(isPlaying)
            return;
        isPlaying = true;
        ResetTimeLine();
        sceneCoroutine = StartCoroutine(RunScene());
    }

    public void SegmentStretched()
    {
        UpdateMaxTrackLength();
        ApplyTimelinePosition(currentTime);
    }
    
    private void UpdateMaxTrackLength()
    {
        maxTrackLength = 0;
        foreach (var track in tracks)
        {
            float trackLength = track.GetTrackLength();
            if (trackLength > maxTrackLength)
                maxTrackLength = trackLength;
        }
        if (currentTime > maxTrackLength)
        {
            currentTime = maxTrackLength;
            MovePointerHeadX(leftEdgeXvalue + (currentTime * trackLengthFor1Second));
        }
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
    
    private void PositionPointerHead(float xValue)
    {
        CancelInteractions();
        MovePointerHeadX(xValue);
        // Convert the clicked X position back to time
        currentTime = (xValue - leftEdgeXvalue) / trackLengthFor1Second;
        ApplyTimelinePosition(currentTime);
    }

    private void OnMouseDown()
    {
        if(isPlaying)
            return;
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clickXPos = mouseWorldPos.x;
        
        float maxXValue = leftEdgeXvalue + (maxTrackLength * trackLengthFor1Second);
        if(clickXPos < leftEdgeXvalue || clickXPos > maxXValue)
            return;
        
        PositionPointerHead(clickXPos);
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
}
