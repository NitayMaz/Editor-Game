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
    [SerializeField] private AudioClips StageAmbience = AudioClips.ParkAmbience;
    [SerializeField] private AudioClips StageStartSound = AudioClips.NoClip;
    [SerializeField] private AudioClips StageFailSound = AudioClips.NoClip;
    

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
        MenuUiCordinator.Instance.CloseStageSuccessUI();
        SoundManager.Instance.StopSound(AudioSources.AmbienceMusic);
        SoundManager.Instance.PlayAudio(StageAmbience);
        OnStart();
    }

    protected virtual void OnStart()
    {
    }

    public virtual void OnStagePlay()
    {
        SoundManager.Instance.PlayAudio(StageStartSound);
    }

    public virtual void StageReset()
    {
    }

    public virtual void TimeLineDone()
    {
    }

    public virtual void StageFailed()
    {
        SoundManager.Instance.PlayAudio(StageFailSound);
    }

    public virtual void StageSuccess()
    {
    }

    public void MoveToNextStage()
    {
        Debug.Log("Moving to next stage");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    public void ShowStageSuccessUI()
    {
        // this is so i don't have to change every stage manager now that we have the menu ui cordinator
        SoundManager.Instance.PlayAudio(AudioClips.SceneSuccess);
        MenuUiCordinator.Instance.ShowStageSuccessUI();
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
            Debug.LogWarning("No solution found for this scene at " + solutionFileName);
            return;
        }

        TimeLine.Instance.LoadSavedClips(sceneSolution.data);
        Debug.Log("Scene solution loaded from " + solutionFileName);
        UndoManager.Clear(); // clear undo stack after loading solution
    }

}