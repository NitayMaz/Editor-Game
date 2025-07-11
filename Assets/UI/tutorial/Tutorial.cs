using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> objectsToTurnOn;
    private int index = 0;
    //hard coding of the century
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private GameObject clipBlocker;
    [SerializeField] private int indToTurnOffBlocker = 5;
    [SerializeField] private GameObject tutorialButtons;

    private void Start()
    {
        TimeLine.Instance.inTutorial = true;
        SoundManager.Instance.PlayAudio(AudioClips.TutorialPop);
        index = 0;
        objectsToTurnOn[index].SetActive(true);
    }

    private void OnMouseDown()
    {
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
        Next();
    }


    private void Next()
    {
        if (index > objectsToTurnOn.Count)
        {
            return;
        }
        objectsToTurnOn[index].SetActive(false);
        if (index == indToTurnOffBlocker)
        {
            clipBlocker.SetActive(false);
        }
        index++;
        if(index == objectsToTurnOn.Count)
        {
            CloseTutorial();
            return;
        }
        objectsToTurnOn[index].SetActive(true);
    }

    private void CloseTutorial()
    {
        animator.SetTrigger("Close");
        TimeLine.Instance.inTutorial = false;
        Destroy(darkPanel, 0.3f);
        Destroy(gameObject, 0.5f);
        Destroy(tutorialButtons, 0.3f);
        
    }

}
