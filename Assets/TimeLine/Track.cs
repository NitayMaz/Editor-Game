using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Timeline;

public class Track : MonoBehaviour
{
    private TrackControlled connectedObject;
    private List<TrackClip> clips = new List<TrackClip>();
    [SerializeField] private GameObject clipPrefab;
    [SerializeField] private Color defaultColor = Color.magenta;
    [SerializeField] private List<Color> clipColors = new List<Color>();
    


    public void InitTrack(TrackControlled connectedObject, TrackAsset UnityTLTrack)
    {
        this.connectedObject = connectedObject;

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
        InitClips(clipsInitData);
    }

    private void InitClips(List<TrackClipInitData> clipsInitData)
    {
        for (int i = 0; i<clipsInitData.Count; i++)
        {
            var clipData = clipsInitData[i];
            Vector2 clipPos = new Vector2(
                transform.position.x + TimeLine.Instance.trackLengthFor1Second * clipData.startTime,
                transform.position.y);
            GameObject clipObject = Instantiate(clipPrefab, clipPos, Quaternion.identity, transform);
            TrackClip clip = clipObject.GetComponentInChildren<TrackClip>();
            clips.Add(clip);
            Color clipColor = clipColors.Count > i ? clipColors[i] : defaultColor;
            clip.Init(clipData.animationClip, clipColor, clipData.duration, 1, clipData.startTime,
                clipData.animationStartPoint, clipData.animationEndPoint, this);
        }

        ApplyTrackPosition(0);
    }

    public float GetTrackDuration()
    {
        if (clips.Count == 0)
            return 0;
        return clips[^1].GetEndTime(); //last clip endtime
    }

    public void ApplyTrackPosition(float time)
    {
        //k so there are 2 cases here:
        //1.the track has multiple animation running the same object
        //2. the track has multiple animations running different objects which are children of the object with the animator(like rush hour)
        //for case 2 we need to run all animations, and if they're not active then put them at the first/last frame according to whether the given time is before/after them.
        //for case 1 we need to check that we don't activate an animation for the same object twice.
        //since if we have 2 upcoming clips for the same object, we would default to the start of the second one, skipping the first already.
        //that's why this code looks awful.
        HashSet<Transform> alreadyMovedTransforms = new HashSet<Transform>();
        //basically we don't want to move a transform that was already moved, if the time is before the clip start time.
        foreach (var clip in clips)
        {
            connectedObject.SetAnimationFrame(clip.animationClip, clip.GetAnimationSpot(time));
        }
    }
    
    public float GetNextClipStartTime(TrackClip currentClip)
    {
        int index = clips.IndexOf(currentClip);
        if (index == -1)
        {
            Debug.LogError("Clip not found in track.");
            return TimeLine.Instance.timeLineSeconds; // fallback value if not found
        }

        if (index + 1 >= clips.Count)
            return TimeLine.Instance.timeLineSeconds;

        return clips[index + 1].startTime;
    }

    public float GetPreviousClipEndTime(TrackClip currentClip)
    {
        int index = clips.IndexOf(currentClip);
        if (index == -1)
        {
            Debug.LogError("Clip not found in track.");
            return 0f; // fallback value if not found
        }

        if (index == 0)
            return 0f;

        return clips[index - 1].GetEndTime();
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
        Color clipColor = replacedClip.GetComponentInChildren<SpriteRenderer>().color;
        //ugly ass code, but it's better than starting to separate the init function rn
        Vector2 firstClipPos = new Vector2(
            transform.position.x + TimeLine.Instance.trackLengthFor1Second * firstPart.startTime,
            transform.position.y);
        TrackClip firstPartClip = Instantiate(clipPrefab, firstClipPos, quaternion.identity, transform).GetComponentInChildren<TrackClip>();
        firstPartClip.Init(firstPart.animationClip, clipColor, firstPart.duration, firstPart.pace,
            firstPart.startTime,
            firstPart.animationStartPoint, firstPart.animationEndPoint, this);
        
        Vector2 secClipPos = new Vector2(
            transform.position.x + TimeLine.Instance.trackLengthFor1Second * secondPart.startTime,
            transform.position.y);
        TrackClip secondPartClip = Instantiate(clipPrefab, secClipPos, quaternion.identity, transform).GetComponentInChildren<TrackClip>();
        secondPartClip.Init(secondPart.animationClip, clipColor, secondPart.duration, secondPart.pace,
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
    public float pace;
    public float animationStartPoint;
    public float animationEndPoint;
    public float startTime;
}