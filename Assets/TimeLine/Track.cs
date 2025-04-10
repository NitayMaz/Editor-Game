using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Track : MonoBehaviour
{
    [SerializeField] private Color segmentColor = Color.cyan;
    [SerializeField] private TrackControlled connectedObject;
    [SerializeField] private List<TrackSegment> segments;


    private void Start()
    {
        float segmentStartTime = 0;
        foreach (var segment in segments)
        {
            segment.Init(Random.ColorHSV(), segment.duration, segmentStartTime, this);
            segmentStartTime += segment.duration;
        }
    }

    public void OrganizeSegments()
    {
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
        if (!connectedObject)
            return;
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