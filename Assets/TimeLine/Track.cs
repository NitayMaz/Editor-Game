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
                pace = 1
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

    public void InitClips(List<TrackClipInitData> clipsInitData)
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
            clip.Init(clipData.animationClip, clipColor, clipData.duration, clipData.pace, clipData.startTime,
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
        //for case 1 we need to make sure run the end of the last animation played, or the beginning of the next animation that would play if we didn't play one yet.
        // so we run every animation that start after the given time in reverse, then every animation that starts before(and in) the given time in order.
        //that's why this code looks awful.
        
        foreach (var clip in clips.Where(c => c.startTime > time).OrderByDescending(c => c.startTime))
        {
            connectedObject.SetAnimationFrame(clip.animationClip, clip.GetAnimationSpot(time));
        }
        foreach (var clip in clips.Where(c => c.startTime <= time).OrderBy(c => c.startTime))
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
        var (firstNewClip, secondNewClip) = CreateCutClips(firstPart, secondPart, clipColor, clipInd);
        RemoveSelectedClip(replacedClip);
        PushCutUndo(firstNewClip, secondNewClip, replacedClip);
    }

    private (TrackClip first, TrackClip second) CreateCutClips(TrackClipInitData firstPart, TrackClipInitData secondPart, Color clipColor, int insertIndex)
    {
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
        clips.InsertRange(insertIndex, new[] { firstPartClip, secondPartClip });
        return (firstPartClip, secondPartClip);
    }

    private void RemoveSelectedClip(TrackClip clip)
    {
        if (clips.Count == 1)
        {
            Debug.Log("You can't delete the last clip of a track");
            return;
        }

        clips.Remove(clip);
        clip.transform.parent.gameObject.SetActive(false); //disable but don't delete, otherwise this can cause issues with undo
        TimeLine.Instance.ClipUpdated();
    }

    private void PushCutUndo(TrackClip firstPart, TrackClip secondPart, TrackClip replacedClip)
    {
        UndoManager.Push(() =>
        {
            Debug.Log("Undoing clip cut");
            int insertIndex = clips.IndexOf(firstPart);
            if (insertIndex < 0)
            {
                Debug.LogError("Trying to undo clip cut but can't find first cut part!");
                return;
            }
            replacedClip.transform.parent.gameObject.SetActive(true); //enable the replaced clip
            clips.Insert(insertIndex, replacedClip);
            RemoveSelectedClip(firstPart);
            RemoveSelectedClip(secondPart);
        });
    }
    public TrackInitData GetClipsData()
    {
        List<TrackClipInitData> clipsData = new List<TrackClipInitData>();
        foreach (var clip in clips)
        {
            clipsData.Add(clip.GetClipInitData());
        }
        return  new TrackInitData { clipsInitData = clipsData };
    }

    public void DeleteAllClips()
    {
        foreach (var clip in clips)
        {
            Destroy(clip.transform.parent.gameObject);
        }
        clips.Clear();
    }

    
}

[Serializable]
public class TrackInitData
{
    public List<TrackClipInitData> clipsInitData;
}