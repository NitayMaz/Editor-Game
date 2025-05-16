using UnityEngine;

public class MyCursor : MonoBehaviour
{
    public static MyCursor Instance;
    [SerializeField] private Camera UICamera;
    private Animator animator;
    
    
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
        animator = GetComponent<Animator>();
    }
    
    private void Update()
    {
        Vector3 mousePosition = UICamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
    }
    
    public void SwitchToNormalCursor()
    {
        animator.SetBool("Holding", false);
    }
    
    public void SwitchToHoldingCursor()
    {
        animator.SetBool("Holding", true);
    }

    public void ButtonClicked()
    {
        animator.SetTrigger("click");
    }
    
    
}
