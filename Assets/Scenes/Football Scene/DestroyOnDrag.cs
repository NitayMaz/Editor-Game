using UnityEngine;

public class DestroyOnDrag : MonoBehaviour
{
    public static DestroyOnDrag Instance;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to initialize a second DestroyOnDrag instance!");
            return;
        }
        Instance = this;
    }

    public void Destroy()
    {
        spriteRenderer.enabled = false;
    }
}
