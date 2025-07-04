using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private List<GameObject> objectsToTurnOn;
    private int index = 0;

    private void Start()
    {
        TimeLine.Instance.inTutorial = true;
        index = 0;
        objectsToTurnOn[index].SetActive(true);
    }

    private void OnMouseDown()
    {
        Next();
    }


    private void Next()
    {
        objectsToTurnOn[index].SetActive(false);
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
        Destroy(gameObject, 0.5f);
    }

}
