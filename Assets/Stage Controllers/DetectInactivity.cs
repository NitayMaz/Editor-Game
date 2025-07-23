using UnityEngine;
using UnityEngine.SceneManagement;

public class InactivityReset : MonoBehaviour
{
    [SerializeField] private float inactivityDuration = 150f; // seconds
    private float timer;

    void Update()
    {
        if (IsUserActive())
        {
            timer = 0f;
        }
        else
        {
            timer += Time.unscaledDeltaTime;
            Debug.Log($"Inactivity Timer: {timer} seconds");
            if (timer >= inactivityDuration)
            {
                RestartGame();
            }
        }
    }

    private bool IsUserActive()
    {
        return Input.anyKey
               || Input.GetAxis("Mouse X") != 0
               || Input.GetAxis("Mouse Y") != 0;
    }

    private void RestartGame()
    {
        // Reload the active scene
        SceneManager.LoadScene(0);
    }
}