using UnityEngine;

public class MyCursor : MonoBehaviour
{
    public static MyCursor Instance;
    [SerializeField] private LayerMask uiLayerMask;
    private bool canCut = false;
    private bool isCutting = false;
    private Animator animator;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("trying to initialize a second cursor!");
            return;
        }
        Instance = this;
        Cursor.visible = false;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (TimeLine.Instance.timeLineCamera == null)
        {
            Debug.LogError("Please assign UI/Timeline camera to the Timeline");
        }
    }

    private void Update()
    {
        Vector3 mousePosition = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        transform.position = mousePosition;
        if(isCutting)
            HandleCutting(mousePosition);
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
    
    public void CutButtonClicked()
    {
        if (isCutting)
            return;
        animator.SetBool("Cutting", true);
        isCutting = true;
        canCut = false;
        Invoke(nameof(EnableCuttingAfterDelay), 0.1f); // hardcoded since the only point is to prevent it happening on the same frame as the button click
    }

    private void EnableCuttingAfterDelay()
    {
        canCut = true;
    }
    
    private void StopCutting()
    {
        animator.SetBool("Cutting", false);
        isCutting = false;
    }

    private void HandleCutting(Vector2 mousePos)
    {
        TrackClip hoveredClip = GetHoveredClip(mousePos);
        animator.SetBool("HoverClip", hoveredClip != null); //true if hovering clip, false otherwise
        if (Input.GetMouseButtonDown(0) && canCut)
        {
            if (hoveredClip)
            {
                animator.SetTrigger("CutClip");
                hoveredClip.CutClip(TimeLine.Instance.GetTimeForXPosition(mousePos.x));
            }
            StopCutting(); //regardless of wheter we cut or not, on click we return to the regular cursor
        }
    }

    private TrackClip GetHoveredClip(Vector2 MousePos)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(MousePos, Vector2.zero, 0f, uiLayerMask);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("clip"))
            {
                return hit.collider.GetComponent<TrackClip>();
            }
        }
        return null;
    }
}
