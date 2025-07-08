using System;
using System.Collections;
using UnityEngine;

public class YogaStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;

    private bool[,] poseEntered;

    [SerializeField] private float
        TimeToCheckYogaPosition = 0.3f; // time to wait before checking if all participants are in the position
    [SerializeField] private Yoga_Girl yogaGirl;
    [SerializeField] private Yoga_Npc yogaNPCs;
    private bool failed = false;
    
    public override void StageReset()
    {
        base.StageReset();
        poseEntered = new bool[Enum.GetValues(typeof(YogaParticipant)).Length, 4]; // 4 poses for each participant, initalized to false
        failed = false;
    }

    public override void TimeLineDone()
    {
        yogaGirl.StartInteraction();
        yogaNPCs.StartInteraction();
        if (!failed)
        {
            SoundManager.Instance.PlayAudio(AudioClips.YogaWin);
        }
        Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }

    public override void StageFailed()
    {
        if (failed) return; // prevent multiple calls
        failed = true;
        base.StageFailed();
        TimeLine.Instance.StopPlaying();
        yogaGirl.StartInteraction();
        yogaNPCs.StartInteraction();
        //girl fail is always the same, npcs fail is different for each pose
        yogaGirl.GetComponent<Animator>().SetBool("Fail", true);
        int lastNpcPose = GetNpcPose();
        yogaNPCs.GetComponent<Animator>().SetBool($"Fail{lastNpcPose}", true);
    }

    public void RegisterPosition(YogaParticipant participant, int positionIndex)
    {
        Debug.Log("Registering position for participant: " + participant + ", position index: " + positionIndex);
        if (positionIndex < 0 || positionIndex >= 4)
        {
            Debug.LogError("Invalid position index: " + positionIndex);
            return;
        }

        poseEntered[(int)participant, positionIndex] = true;
        StartCoroutine(CheckPositionSynch(positionIndex));
    }

    private IEnumerator CheckPositionSynch(int posInd)
    {
        yield return new WaitForSeconds(TimeToCheckYogaPosition);
        //check all participants reached the position
        Debug.Log("Checking if all participants reached position index: " + posInd);
        foreach (YogaParticipant participant in Enum.GetValues(typeof(YogaParticipant)))
        {
            if (!poseEntered[(int)participant, posInd])
                StageFailed();
        }
    }

    private int GetNpcPose()
    {
        int lastNpcPose = 0;
        for (int i = 3; i > 0; i--)
        {
            if(poseEntered[(int)YogaParticipant.NPCs, i])
            {
                lastNpcPose = i;
                break;
            }
        }
        return lastNpcPose + 1; // 1-based index for animator
    }
}

public enum YogaParticipant //kinda useless as for now
{
    Girl = 0,
    NPCs = 1
}