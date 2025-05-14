using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple StageManagers");
            Destroy(gameObject);
        }
        Instance = this;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
