using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [SerializeField] private GameObject stageSuccessUI;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuButton;
    [SerializeField] private GameObject UIcursor;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple StageManagers");
            Destroy(gameObject);
        }

        Instance = this;
    }


    private void Start()
    {
        if (stageSuccessUI == null || pauseMenu == null || pauseMenuButton == null || UIcursor == null)
        {
            Debug.LogError("StageManager is not properly initialized. Please assign all UI elements in the inspector.");
        }
        stageSuccessUI.SetActive(false);
        pauseMenu.SetActive(false);
        pauseMenuButton.SetActive(true);
        UIcursor.SetActive(false);
        OnStart();
    }

    public virtual void OnStart()
    {
        
    }

    public virtual void OnStagePlay()
    {
    }

    public virtual void StageReset()
    {
    }

    public virtual void TimeLineDone()
    {
    }

    public virtual void StageFailed()
    {
    }

    public virtual void StageSuccess()
    {
    }

    public void MoveToNextStage()
    {
        Debug.Log("Moving to next stage");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    protected void ShowStageSuccessUI()
    {
        SoundManager.Instance.PlayAudio(AudioClips.SceneSuccess);
        stageSuccessUI.SetActive(true);
        enableUICursor();
        Debug.Log("Stage Success UI shown");
    }

    public void ResetLevel()
    {
        Debug.Log("Resetting Level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    [ContextMenu("Save Scene Solution")]
    public void SaveSceneSolution()
    {
        #if UNITY_EDITOR // without this build crashes
        
        List<TrackInitData> solution = TimeLine.Instance.GetClipsData();
        string save_path = "Assets/Resources/" + SceneManager.GetActiveScene().name + "_solution.asset";
        if (AssetDatabase.LoadAssetAtPath<SceneSolution>(save_path) != null)
        {
            AssetDatabase.DeleteAsset(save_path);
        }

        SceneSolution sceneSolution = ScriptableObject.CreateInstance<SceneSolution>();
        sceneSolution.data = solution;
        AssetDatabase.CreateAsset(sceneSolution, save_path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Scene solution saved to " + save_path);
        
        #endif
    }

    public void LoadSceneSolution()
    {
        string solutionFileName = SceneManager.GetActiveScene().name + "_solution";
        SceneSolution sceneSolution = Resources.Load<SceneSolution>(solutionFileName);
        if (sceneSolution == null)
        {
            Debug.LogError("No solution found for this scene at " + solutionFileName);
            return;
        }

        TimeLine.Instance.LoadSavedClips(sceneSolution.data);
        Debug.Log("Scene solution loaded from " + solutionFileName);
        HidePauseMenu(); // exit pause menu
        UndoManager.Clear(); // clear undo stack after loading solution
    }

    // ##################### UI BS ########################
    public void ShowPauseMenu()
    {
        if (pauseMenu.activeSelf)
            return;
        
        TimeLine.Instance.StopPlaying();
        enableUICursor();
        stageSuccessUI.SetActive(false);
        pauseMenu.SetActive(true);
        pauseMenuButton.SetActive(false);

    }

    public void HidePauseMenu()
    {
        if(!pauseMenu.activeSelf)
            return;
        disableUICursor();
        pauseMenu.SetActive(false);
        pauseMenuButton.SetActive(true);
        
    }
    
    public void CloseStageSuccessUI()
    {
        stageSuccessUI.SetActive(false);
        disableUICursor();
    }

    private void enableUICursor()
    {
        UIcursor.SetActive(true);
        MyCursor.Instance.gameObject.SetActive(false);
    }
    
    private void disableUICursor()
    {
        UIcursor.SetActive(false);
        MyCursor.Instance.gameObject.SetActive(true);
    }

    public void PlayClickSound()
    {
        SoundManager.Instance.PlayAudio(AudioClips.MouseClick);
    }
}