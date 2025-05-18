using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private GameObject playPauseButton;
    private Animator playPauseButtonAnimator;
    private bool playButtonShowing = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple UIManagers");
            Destroy(gameObject);
        }
        Instance = this;
        
        playPauseButtonAnimator = playPauseButton.GetComponent<Animator>();
    }

    public void CutButtonClicked()
    {
        MyCursor.Instance?.CutButtonClicked();
    }

    public void ResetClicked()
    {
        TimeLine.Instance.ResetLevel();
    }

    public void PlayPauseButtonClicked()
    {
        if (playButtonShowing)
        {
            ChangeToPauseButton();
            TimeLine.Instance.PlayScene();
        }
        else
        {
            ChangeToPlayButton();
            TimeLine.Instance.StopPlaying();
        }
    }
    
    public void ChangeToPlayButton()
    {
        playPauseButtonAnimator.SetBool("IsStopped", false);
        playButtonShowing = true;
    }
    public void ChangeToPauseButton()
    {
        playPauseButtonAnimator.SetBool("IsStopped", true);
        playButtonShowing = false;
    }

}