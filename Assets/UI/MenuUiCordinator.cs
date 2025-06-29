using UnityEngine;
using UnityEngine.UI;

public class MenuUiCordinator : MonoBehaviour
{
    public static MenuUiCordinator Instance;
    [SerializeField] private GameObject nextStageButton;
    [SerializeField] private GameObject pauseMenuButton;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private GameObject UIcursor;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple MenuUiCordinator, That's exactly how marvel failed.");
            Destroy(this.gameObject);
        }
        Instance = this;
        if(!nextStageButton || !pauseMenuButton || !pauseMenu || !musicVolumeSlider || !sfxVolumeSlider || !UIcursor)
        {
            Debug.LogError("MenuUiCordinator is missing some UI elements, please assign them in the inspector.");
        }
        nextStageButton.SetActive(false);
        pauseMenu.SetActive(false);
        pauseMenuButton.SetActive(true);
        UIcursor.SetActive(false);
    }

    public void NextStageButtonClicked() // temporary
    {
        StageManager.Instance.MoveToNextStage();
    }
    
    public void OpenPauseMenu()
    {
        //pause play, activate pause menu, activate ui cursor
        if (pauseMenu.activeSelf)
            return;
        TimeLine.Instance.StopPlaying();
        enableUICursor();
        pauseMenu.SetActive(true);
        pauseMenuButton.SetActive(false);
        Debug.Log("Pause menu opened");
        Debug.Log("Music volume: " + SoundManager.Instance.musicVolume);
        musicVolumeSlider.value = SoundManager.Instance.musicVolume;
        Debug.Log("SFX volume: " + SoundManager.Instance.soundEffectsVolume);
        sfxVolumeSlider.value = SoundManager.Instance.soundEffectsVolume;
    }
    
    public void ClosePauseMenu()
    {
        //return to normal cursor and close pause menu
        if(!pauseMenu.activeSelf)
            return;
        disableUICursor();
        pauseMenu.SetActive(false);
        pauseMenuButton.SetActive(true);
        
    }
    
    
    public void MusicSliderMoved(float value)
    {
        SoundManager.Instance.ChangeMusicVolume(value);
    }
    
    public void SFXSliderMoved(float value)
    {
        SoundManager.Instance.ChangeSoundEffectsVolume(value);
    }
    
    public void ShowSolutionButtonClicked()
    {
        StageManager.Instance.LoadSceneSolution();
        ClosePauseMenu();
    }
    
    public void RestartGameButtonClicked()
    {
        StageManager.Instance.ResetGame();
        ClosePauseMenu();
    }
    
    
    public void ShowStageSuccessUI()
    {
        nextStageButton.SetActive(true);
    }
    
    public void CloseStageSuccessUI()
    {
        nextStageButton.SetActive(false);
    }

    private void enableUICursor()
    {
        UIcursor.SetActive(true);
        // MyCursor.Instance.gameObject.SetActive(false);
        MyCursor.Instance.Hide();
    }
    
    private void disableUICursor()
    {
        UIcursor.SetActive(false);
        // MyCursor.Instance.gameObject.SetActive(true);
        MyCursor.Instance.Show();
    }

    public void PlayClickSound()
    {
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
    }
}
