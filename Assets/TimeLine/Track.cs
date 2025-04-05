using System;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private Color segmentColor = Color.cyan;
    [SerializeField] private TrackControlled connectedObject;
    [SerializeField] private List<TrackSegment> segments;


    private void Start()
    {
        foreach (var segment in segments)
        {
            segment.Init(segmentColor, segment.duration, segment.startTime, segment.endTime);
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
        if(!connectedObject)
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
        connectedObject.TurnAnimatorOff();
    }
}
