using System;
using System.Collections;
using UnityEngine;

public class YogaStageManager : StageManager
{
    [SerializeField] private float timeToShowSuccessUI = 1f;
    
    private bool[,]
        poseEntered =
            new bool[Enum.GetValues(typeof(YogaParticipant)).Length, 4]; // 4 poses for each participant(in the enum)

    [SerializeField] private float
        TimeToCheckYogaPosition = 0.3f; // time to wait before checking if all participants are in the position

    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] private Yoga_Girl yogaGirl;
    [SerializeField] private Yoga_Npc yogaNPCs;

    public override void StageReset()
    {
        base.StageReset();
        Array.Clear(poseEntered, 0, poseEntered.Length); // reset values in array to false
    }

    public override void TimeLineDone()
    {
        yogaGirl.StartInteraction();
        yogaNPCs.StartInteraction();
        Invoke(nameof(ShowStageSuccessUI),timeToShowSuccessUI);
    }

    public override void StageFailed()
    {
        base.StageFailed();
        //particle?
        duckExplodes.Play();
        TimeLine.Instance.StopPlaying();
        yogaGirl.StartInteraction();
        yogaNPCs.StartInteraction();
        yogaGirl.GetComponent<Animator>().SetBool("Fail", true);
        yogaNPCs.GetComponent<Animator>().SetBool("Fail", true);
    }

    public void RegisterPosition(YogaParticipant participant, int positionIndex)
    {
        Debug.Log("Registering position for participant: " + participant + ", position index: " + positionIndex);
        if (positionIndex < 0 || positionIndex >= poseEntered.GetLength(1))
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
        //check for each of the participants in the enum if they reach the position, if they didn't, fail.
        Debug.Log("Checking if all participants reached position index: " + posInd);
        foreach (YogaParticipant participant in Enum.GetValues(typeof(YogaParticipant)))
        {
            if (!poseEntered[(int)participant, posInd])
                StageFailed();
        }
    }
}

public enum YogaParticipant
{
    Girl = 0,
    NPCs = 1
}