using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Track : MonoBehaviour
{
    [SerializeField] private Color segmentColor = Color.cyan;
    [SerializeField] private TrackControlled connectedObject;
    private List<TrackSegment> segments = new List<TrackSegment>();
    [SerializeField] private GameObject segmentPrefab;
    [SerializeField] private List<TrackSegmentInitData> segmentsInitData;


    private void Start()
    {
        float segmentStartTime = 0;
        foreach (var segmentData in segmentsInitData)
        {
            GameObject segmentObject = Instantiate(segmentPrefab, transform);
            TrackSegment segment = segmentObject.GetComponent<TrackSegment>();
            segments.Add(segment);
            segment.Init(segmentColor, segmentData.duration, segmentStartTime, 
                            segmentData.animationStartPoint, segmentData.animationEndPoint, this);
            segmentStartTime += segmentData.duration;
        }
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
            TrackSegment lastSegment = segments[segments.Count - 1];
            connectedObject.SetAnimationFrame(lastSegment.GetAnimationSpot(time));
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
}

[Serializable]
public class TrackSegmentInitData
{
    public float duration;
    [Range(0,1)] public float animationStartPoint;
    [Range(0,1)] public float animationEndPoint;
}