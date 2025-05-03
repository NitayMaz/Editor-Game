using UnityEngine;

public class MyCursor : MonoBehaviour
{
    public static MyCursor Instance;
    [SerializeField] private Camera UICamera;
    [SerializeField] private Sprite normalCursorSprite;
    [SerializeField] private Sprite holdingCursorSprite;
    
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("trying to initalize a second cursor!");
            return;
        }
        Instance = this;
        Cursor.visible = false;
        if (UICamera == null)
        {
            Debug.LogError("Please assign UI/Timeline camera to the cursor or it will not work!");
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normalCursorSprite;
    }
    
    private void Update()
    {
        Vector3 mousePosition = UICamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }
    
    public void SwitchToNormalCursor()
    {
        spriteRenderer.sprite = normalCursorSprite;
    }
    
    public void SwitchToHoldingCursor()
    {
        spriteRenderer.sprite = holdingCursorSprite;
    }
    
    
}
