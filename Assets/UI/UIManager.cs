using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private Button playAndPauseButton;
    [SerializeField] private Sprite playSprite;
    [SerializeField] private Sprite pauseSprite;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple UIManagers");
            Destroy(this.gameObject);
        }
        Instance = this;
    }

    public void PlayAndPauseClicked()
    {
        if (TimeLine.Instance.isPlaying)
        {
            TimeLine.Instance.PauseScene();
        }
        else
        {
            TimeLine.Instance.PlayScene();
        }
    }
    
    public void ChangeToPauseSprite()
    {
        playAndPauseButton.image.sprite = pauseSprite;
    }
    
    public void ChangeToPlaySprite()
    {
        playAndPauseButton.image.sprite = playSprite;
    }

    public void StopButtonClicked()
    {
        TimeLine.Instance.ResetTimeLine();
    }
}
