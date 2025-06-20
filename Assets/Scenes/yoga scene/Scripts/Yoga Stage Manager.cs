using System;
using System.Collections;
using UnityEngine;

public class YogaStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;

    private int[] poseEntered;

    [SerializeField] private float
        TimeToCheckYogaPosition = 0.3f; // time to wait before checking if all participants are in the position

    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] private Yoga_Girl yogaGirl;
    [SerializeField] private Yoga_Girl yogaGirl2;
    [SerializeField] private Yoga_Npc yogaNPCs;
    [SerializeField] private int peopleInScene = 2;
    
    public override void StageReset()
    {
        base.StageReset();
        poseEntered = new int[4]; // 4 poses for each participant, all initialized to 0
    }

    public override void TimeLineDone()
    {
        yogaGirl.StartInteraction();
        yogaGirl2?.StartInteraction();
        yogaNPCs.StartInteraction();
        Invoke(nameof(ShowStageSuccessUI), timeToShowSuccessUI);
    }

    public override void StageFailed()
    {
        base.StageFailed();
        //particle?
        duckExplodes.Play();
        TimeLine.Instance.StopPlaying();
        yogaGirl.GetComponent<Animator>().SetBool("Fail", true);
        yogaGirl2?.GetComponent<Animator>().SetBool("Fail", true);
        yogaNPCs.GetComponent<Animator>().SetBool("Fail", true);
        yogaGirl.StartInteraction();
        yogaGirl2?.StartInteraction();
        yogaNPCs.StartInteraction();
    }

    public void RegisterPosition(YogaParticipant participant, int positionIndex)
    {
        Debug.Log("Registering position for participant: " + participant + ", position index: " + positionIndex);
        if (positionIndex < 0 || positionIndex >= 4)
        {
            Debug.LogError("Invalid position index: " + positionIndex);
            return;
        }

        poseEntered[positionIndex]++;
        StartCoroutine(CheckPositionSynch(positionIndex));
    }

    private IEnumerator CheckPositionSynch(int posInd)
    {
        yield return new WaitForSeconds(TimeToCheckYogaPosition);
        //check all participants reached the position
        Debug.Log("Checking if all participants reached position index: " + posInd + "pos values is: " + poseEntered[posInd]);
        if (poseEntered[posInd] != peopleInScene)
            StageFailed();
        
    }
}

public enum YogaParticipant //kinda useless as for now
{
    Girl = 0,
    NPCs = 1
}