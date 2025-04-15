using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple UIManagers");
            Destroy(gameObject);
        }

        Instance = this;
    }

    public void PlayClicked()
    {
        TimeLine.Instance.PlayScene();
    }


    public void StopButtonClicked()
    {
        TimeLine.Instance.StopPlaying();
    }

    public void CutButtonClicked()
    {
        TimeLine.Instance.CutSelectedSegment();
    }
    
    public void DeleteButtonClicked()
    {
        TimeLine.Instance.DeleteSelectedSegment();
    }

}