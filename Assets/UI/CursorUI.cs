using UnityEngine;

public class CursorUI : MonoBehaviour
{
    [SerializeField] private RectTransform cursorImage;
    [SerializeField] private Vector2 offset = new Vector2(0, 0);
    
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        cursorImage.position = mousePos + offset;
    }
}
