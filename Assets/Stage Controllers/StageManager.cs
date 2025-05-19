using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;
    [SerializeField] private GameObject stageSuccessUI;

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
        if (stageSuccessUI == null)
        {
            Debug.LogError("Stage Success UI is not assigned in the inspector");
        }
        else
        {
            stageSuccessUI.SetActive(false);
        }
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
        stageSuccessUI.SetActive(true);
        Debug.Log("Stage Success UI shown");
    }

    public void ResetLevel()
    {
        Debug.Log("Resetting Level");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }
}
