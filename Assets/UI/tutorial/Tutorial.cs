using System;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    //hardcoding
    
    [SerializeField] private Animator animator;
    public static Tutorial Instance;
    private int index = 0;
    [SerializeField] private List<ObjectsToTurnOnOff> objectsToTurnOnOff;
    [SerializeField] private bool isFirstTutorial = false;
    [SerializeField] private int rulerPanelIndex = 0;
    [SerializeField] private int dragClipPanelIndex = 0;
    [SerializeField] private int panelIndexToPlayScene = 0;
    [SerializeField] private int panelToPressPlay = 0;
    [SerializeField] private bool isFootballTutorial = false;
    [SerializeField] private bool isDuckTutorial = false;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple Tutorials");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        TimeLine.Instance.inTutorial = true;
        SoundManager.Instance.PlayAudio(AudioClips.TutorialPop);
        index = 0;
        TurnOnObjects(index);
    }

    private void OnMouseDown()
    {
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
        Next();
    }


    public void Next()
    {
        if (index >= objectsToTurnOnOff.Count)
        {
            return;
        }
        TurnOffObjects(index);
        index++;
        if(index == objectsToTurnOnOff.Count)
        {
            CloseTutorial();
            return;
        }
        TurnOnObjects(index);
        if (index == panelIndexToPlayScene)
        {
            TimeLine.Instance.PlayScene();
        }
    }
    
    private void TurnOnObjects(int ind)
    {
        foreach (var obj in objectsToTurnOnOff[ind].objectsToTurnOn)
        {
            obj.SetActive(true);
        }
    }
    
    private void TurnOffObjects(int ind)
    {
        Debug.Log($"Turning off objects for index {ind}");
        foreach (var obj in objectsToTurnOnOff[ind].objectsToTurnOff)
        {
            obj.SetActive(false);
        }
    }

    private void CloseTutorial()
    {
        animator.SetTrigger("Close");
        TimeLine.Instance.inTutorial = false;
        Destroy(gameObject, 0.3f);
    }

    public void RulerDragged()
    {
        if (isFirstTutorial && index == rulerPanelIndex)
        {
            Next();
        }
    }
    
    public void ClipDragged()
    {
        if (isFirstTutorial && index == dragClipPanelIndex)
        {
            Next();
        }
    }

    public void PlayPressedTutorial()
    {
        if(isFirstTutorial && index == panelToPressPlay)
        {
            Next();
            TimelineUIManager.Instance.PlayPauseButtonClicked();
        }
    }

    public void ClipStretched()
    {
        if (isFootballTutorial)
        {
            Next();
        }
    }

    public void ClipCutButtonClicked()
    {
        if (isDuckTutorial)
        {
            Next();
        }
        
    }

    public void ClipCut()
    {
        if (isDuckTutorial)
        {
            Next();
        }
    }
}

[Serializable]
public class ObjectsToTurnOnOff
{
    public List<GameObject> objectsToTurnOn;
    public List<GameObject> objectsToTurnOff;
}
