using System;
using System.Collections.Generic;
using UnityEngine;

public class Yoga_Npc : TrackControlled
{
    HashSet<String> animationsReached = new HashSet<string>();

    public override void SetAnimationFrame(AnimationClip animationClip, float time)
    {
        base.SetAnimationFrame(animationClip, time);
        //if this is a new position, register it. all other conditions are because of quirks of setting the timeline time
        if (!animationsReached.Contains(animationClip.name) && TimeLine.Instance.isPlaying && time !=0 && time != 1f)
        {
            ((YogaStageManager)StageManager.Instance).RegisterPosition(YogaParticipant.NPCs, animationsReached.Count);
            animationsReached.Add(animationClip.name);
        }
    }

    public override void StopInteraction()
    {
        animator.SetBool("Fail", false);
        base.StopInteraction();
        animationsReached.Clear(); // clear positions reach when resetting/restrating timeline
    }
}
