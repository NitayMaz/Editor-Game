using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Track : MonoBehaviour
{
    [SerializeField] private Color segmentColor = Color.cyan;
    [SerializeField] private float TrackHeight;
    [SerializeField] private TrackControlled connectedObject;
    private List<TrackClip> segments = new List<TrackClip>();
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private List<TrackSegmentInitData> segmentsInitData;
    


    private void Start()
    {
        InitTrack();
    }

    [ContextMenu("Populate Clips")]
    private void InitTrack()
    {
        float segmentStartTime = 0;
        foreach (var segmentData in segmentsInitData)
        {
            GameObject segmentObject = Instantiate(segmentPrefab, transform);
            TrackClip clip = segmentObject.GetComponentInChildren<TrackClip>();
            segments.Add(clip);
            clip.Init(segmentColor, segmentData.duration, 1, segmentStartTime, 
                segmentData.animationStartPoint, segmentData.animationEndPoint,TrackHeight, this);
            segmentStartTime += segmentData.duration;
        }
        ApplyTrackPosition(0);
    }
    public void OrganizeSegments()
    {
        if (segments.Count == 0)
            return;
        float xPos = 0;
        float segmentStartTime = 0;
        foreach (var segment in segments)
        {
            segment.transform.position =
                new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z);
            segment.startTime = segmentStartTime;
            xPos += segment.width;
            segmentStartTime += segment.duration;
            segment.GetComponent<TrackClip>().UpdateTextAndHandle(); // it's important this happen here because the positions are messed up if it happens in init.
        }
    }

    public float GetTrackLength()
    {
        float length = 0;
        foreach (var segment in segments)
        {
            length += segment.duration;
        }
        return length;
    }

    public void ApplyTrackPosition(float time)
    {
        //if the timeline handle is beyond this track, stay on the last frame of the track
        if (time > GetTrackLength())
        {
            TrackClip lastClip = segments[segments.Count - 1];
            connectedObject.SetAnimationFrame(lastClip.GetAnimationSpot(time));
        }

        foreach (var segment in segments)
        {
            if (segment.IsActive(time))
            {
                connectedObject.SetAnimationFrame(segment.GetAnimationSpot(time));
                break;
            }
        }
    }

    public void CancelObjectInteraction()
    {
        if (!connectedObject)
            return;
        connectedObject.StopInteraction();
    }

    public void ReplaceCutSegment(TrackClip replacedClip, TrackSegmentInitData firstPart, TrackSegmentInitData secondPart)
    {
        int segmentInd = segments.IndexOf(replacedClip);
        if (segmentInd < 0)
        {
            Debug.LogError("The Segment We're trying to cut is not in the track segments list!");
            return;
        }
        //ugly ass code, but it's better than starting to separate the init function rn
        TrackClip firstPartClip = Instantiate(segmentPrefab, transform).GetComponentInChildren<TrackClip>();
        firstPartClip.Init(segmentColor, firstPart.duration, firstPart.durationMultiplier, replacedClip.startTime,
            firstPart.animationStartPoint, firstPart.animationEndPoint, TrackHeight, this);
        TrackClip secondPartClip = Instantiate(segmentPrefab, transform).GetComponentInChildren<TrackClip>();
        secondPartClip.Init(segmentColor, secondPart.duration, firstPart.durationMultiplier, replacedClip.startTime + firstPart.duration,
            secondPart.animationStartPoint, secondPart.animationEndPoint, TrackHeight, this);
        segments.InsertRange(segmentInd, new[] { firstPartClip, secondPartClip });
        DeleteSelectedSegment(replacedClip);
        TimeLine.Instance.SelectTrackSegment(null);
    }
    
    public void DeleteSelectedSegment(TrackClip clip)
    {
        if (segments.Count == 1)
        {
            Debug.Log("You can't delete the last segment of a track");
            return;
        }
        segments.Remove(clip);
        Destroy(clip.transform.parent.gameObject);
        OrganizeSegments();
    }
    
}

[Serializable]
public class TrackSegmentInitData
{
    public float duration;
    public float durationMultiplier;
    [Range(0,1)] public float animationStartPoint;
    [Range(0,1)] public float animationEndPoint;
    
}