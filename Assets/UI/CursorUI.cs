using UnityEngine;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Canvas canvas;
    
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            mousePos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        cursorImage.anchoredPosition = localPoint;
    }
}
