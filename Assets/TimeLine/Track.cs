using System;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
    [SerializeField] private Color clipColor = Color.cyan;
    [SerializeField] private float TrackHeight;
    [SerializeField] private TrackControlled connectedObject;
    private List<TrackClip> clips = new List<TrackClip>();
    [SerializeField] private GameObject clipPrefab;
    [SerializeField] private List<TrackClipInitData> clipsInitData;
    


    private void Start()
    {
        InitTrack();
    }

    [ContextMenu("Populate Clips")]
    private void InitTrack()
    {
        float clipStartTime = 0;
        foreach (var clipData in clipsInitData)
        {
            GameObject clipObject = Instantiate(clipPrefab, transform);
            TrackClip clip = clipObject.GetComponentInChildren<TrackClip>();
            clips.Add(clip);
            clip.Init(clipColor, clipData.duration, 1, clipStartTime, 
                clipData.animationStartPoint, clipData.animationEndPoint,TrackHeight, this);
            clipStartTime += clipData.duration;
        }
        ApplyTrackPosition(0);
    }
    public void OrganizeClips()
    {
        if (clips.Count == 0)
            return;
        float xPos = 0;
        float clipStartTime = 0;
        foreach (var clip in clips)
        {
            clip.transform.position =
                new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z);
            clip.startTime = clipStartTime;
            xPos += clip.width;
            clipStartTime += clip.duration;
            clip.GetComponent<TrackClip>().UpdateTextAndHandle(); // it's important this happen here because the positions are messed up if it happens in init.
        }
        TimeLine.Instance.ClipUpdated();
    }

    public float GetTrackDuration()
    {
        float trackDuration = 0;
        foreach (var clip in clips)
        {
            trackDuration += clip.duration;
        }
        return trackDuration;
    }

    public void ApplyTrackPosition(float time)
    {
        //if the timeline handle is beyond this track, stay on the last frame of the track
        if (time > GetTrackDuration())
        {
            TrackClip lastClip = clips[clips.Count - 1];
            connectedObject.SetAnimationFrame(lastClip.GetAnimationSpot(time));
        }

        foreach (var clip in clips)
        {
            if (clip.IsActive(time))
            {
                connectedObject.SetAnimationFrame(clip.GetAnimationSpot(time));
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

    public void ReplaceCutClip(TrackClip replacedClip, TrackClipInitData firstPart, TrackClipInitData secondPart)
    {
        int clipInd = clips.IndexOf(replacedClip);
        if (clipInd < 0)
        {
            Debug.LogError("The clip We're trying to cut is not in the track clips list!");
            return;
        }
        //ugly ass code, but it's better than starting to separate the init function rn
        TrackClip firstPartClip = Instantiate(clipPrefab, transform).GetComponentInChildren<TrackClip>();
        firstPartClip.Init(clipColor, firstPart.duration, firstPart.durationMultiplier, replacedClip.startTime,
            firstPart.animationStartPoint, firstPart.animationEndPoint, TrackHeight, this);
        TrackClip secondPartClip = Instantiate(clipPrefab, transform).GetComponentInChildren<TrackClip>();
        secondPartClip.Init(clipColor, secondPart.duration, firstPart.durationMultiplier, replacedClip.startTime + firstPart.duration,
            secondPart.animationStartPoint, secondPart.animationEndPoint, TrackHeight, this);
        clips.InsertRange(clipInd, new[] { firstPartClip, secondPartClip });
        DeleteSelectedClip(replacedClip);
        TimeLine.Instance.SelectTrackClip(null);
    }
    
    public void DeleteSelectedClip(TrackClip clip)
    {
        if (clips.Count == 1)
        {
            Debug.Log("You can't delete the last clip of a track");
            return;
        }
        clips.Remove(clip);
        Destroy(clip.transform.parent.gameObject);
        OrganizeClips();
    }
    
}

[Serializable]
public class TrackClipInitData
{
    public float duration;
    [HideInInspector]public float durationMultiplier = 1;
    public float animationStartPoint;
    public float animationEndPoint;
}