using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private Color TLColor = Color.cyan;
    [SerializeField] private TrackControlled connectedObject;
    [SerializeField] private List<TrackSegment> segments;

    
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
        foreach (var segment in segments)
        {
            if (segment.IsActive(time))
            {
                connectedObject.SetAnimationFrame(segment.GetAnimationSpot(time));
                break;
            }
        }
        
    }
}
