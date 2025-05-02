using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class Track : MonoBehaviour
{
    [SerializeField] private Color clipColor = Color.cyan;
    private TrackControlled connectedObject;
    private List<TrackClip> clips = new List<TrackClip>();
    [SerializeField] private GameObject clipPrefab;


    public void InitTrack(TrackControlled connectedObject, TrackAsset UnityTLTrack)
    {
        this.connectedObject = connectedObject;
        Debug.Log(
            $"Initializing track {UnityTLTrack.name} for {connectedObject.name} with {UnityTLTrack.GetClips().Count()} clips");

        List<TrackClipInitData> clipsInitData = new List<TrackClipInitData>();
        foreach (var clip in UnityTLTrack.GetClips())
        {
            var animPlayable = clip.asset as AnimationPlayableAsset;
            if (animPlayable == null)
            {
                Debug.LogError("The clip is not an AnimationPlayableAsset");
                continue;
            }

            AnimationClip animationClip = animPlayable.clip;
            if (animationClip == null)
            {
                Debug.LogError("The clip is not an AnimationClip");
                continue;
            }

            TrackClipInitData clipData = new TrackClipInitData
            {
                animationClip = animationClip,
                duration = (float)clip.duration,
                animationStartPoint = (float)(clip.clipIn * clip.timeScale) / animationClip.length,
                animationEndPoint = (float)((clip.clipIn + clip.duration) * clip.timeScale) / animationClip.length,
                startTime = (float)clip.start,
            };
            if (clipsInitData.Count !=
                0) // check if the following clip is a duplicate of the last one, if so extend the last one instead of adding.
            {
                TrackClipInitData lastClipData = clipsInitData[^1];
                if (lastClipData.animationClip == animationClip &&
                    lastClipData.animationEndPoint % 1f == clipData.animationStartPoint % 1f)
                {
                    lastClipData.duration += clipData.duration;
                    lastClipData.animationEndPoint += (clipData.animationEndPoint - clipData.animationStartPoint);
                    continue;
                }
            }

            clipsInitData.Add(clipData);
        }

        Debug.Log("off to init clips");
        InitClips(clipsInitData);
    }

    private void InitClips(List<TrackClipInitData> clipsInitData)
    {
        float clipStartTime = 0;
        foreach (var clipData in clipsInitData)
        {
            Debug.Log("start time: " + clipStartTime);
            Vector2 clipPos = new Vector2(
                transform.position.x + TimeLine.Instance.trackLengthFor1Second * clipData.startTime,
                transform.position.y);
            Debug.Log("clip pos: " + clipPos);
            GameObject clipObject = Instantiate(clipPrefab, clipPos, Quaternion.identity, transform);
            TrackClip clip = clipObject.GetComponentInChildren<TrackClip>();
            clips.Add(clip);
            clip.Init(clipData.animationClip, clipColor, clipData.duration, 1, clipData.startTime,
                clipData.animationStartPoint, clipData.animationEndPoint, this);
        }

        ApplyTrackPosition(0);
    }

    public float GetTrackDuration()
    {
        if (clips.Count == 0)
            return 0;
        return clips[^1].endTime; //last clip endtime
    }

    public void ApplyTrackPosition(float time)
    {
        //if the timeline handle is beyond this track, stay on the last frame of the track
        if (time > GetTrackDuration())
        {
            TrackClip lastClip = clips[clips.Count - 1];
            connectedObject.SetAnimationFrame(lastClip.animationClip, lastClip.GetAnimationSpot(time));
        }

        foreach (var clip in clips)
        {
            if (clip.IsActive(time))
            {
                connectedObject.SetAnimationFrame(clip.animationClip, clip.GetAnimationSpot(time));
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
        firstPartClip.Init(firstPart.animationClip, clipColor, firstPart.duration, firstPart.durationMultiplier,
            firstPart.startTime,
            firstPart.animationStartPoint, firstPart.animationEndPoint, this);

        TrackClip secondPartClip = Instantiate(clipPrefab, transform).GetComponentInChildren<TrackClip>();
        secondPartClip.Init(secondPart.animationClip, clipColor, secondPart.duration, secondPart.durationMultiplier,
            secondPart.startTime,
            secondPart.animationStartPoint, secondPart.animationEndPoint, this);

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
        TimeLine.Instance.ClipUpdated();
    }
}

public class TrackClipInitData
{
    public AnimationClip animationClip;
    public float duration;
    public float durationMultiplier = 1;
    public float animationStartPoint;
    public float animationEndPoint;
    public float startTime;
}