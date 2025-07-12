using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeRuler : MonoBehaviour
{
    [SerializeField] private GameObject smallTickPrefab;
    [SerializeField] private GameObject bigTickPrefab;

    [SerializeField] private float smallTickFrequency;
    [SerializeField] private float bigTickFrequency;

    [SerializeField] private GameObject timeLineBackground;
    [SerializeField] private Transform ticksSceneParent;

    void Start()
    {
        PositionRuler();
        PositionTicks();
    }

    private void PositionRuler()
    {
        Bounds backgroundBounds = timeLineBackground.GetComponent<SpriteRenderer>().bounds;
        Vector3 backgroundTopLeft = new Vector3(backgroundBounds.min.x, backgroundBounds.max.y, transform.position.z);
        transform.position = backgroundTopLeft;

        // Stretch ruler sprite to match the background width
        float backgroundWidth = backgroundBounds.size.x;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.size = new Vector2(backgroundWidth, spriteRenderer.size.y);
        // float spriteOriginalWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        //
        // Vector3 scale = transform.localScale;
        // scale.x = backgroundWidth / spriteOriginalWidth;
        // transform.localScale = scale;
    }

    private void PositionTicks()
    {
        float timeCovered = TimeLine.Instance.timeLineSeconds;
        float rulerBottom = gameObject.GetComponent<SpriteRenderer>().bounds.min.y;
        HashSet<float> bigTickIndices = new HashSet<float>();
        for (float i = bigTickFrequency; i < timeCovered; i += bigTickFrequency)
        {
            float tickXPos = TimeLine.Instance.GetXPositionForTime(i);
            // snapping is for floating point precision errors
            bigTickIndices.Add(TimeLine.SnapTo(i, 0.01f)); 
            Vector3 bigTickPos = new Vector3(tickXPos, rulerBottom, transform.position.z);
            Instantiate(bigTickPrefab, bigTickPos, Quaternion.identity, ticksSceneParent);
        }

        // position small ticks, -0.001f is to avoid floating point precision errors
        for (float i = smallTickFrequency; i < timeCovered - 0.001f; i += smallTickFrequency)
        {
            float tickXPos = TimeLine.Instance.GetXPositionForTime(i);
            if (bigTickIndices.Contains(TimeLine.SnapTo(i, 0.01f)))
            {
                continue;
            }
            Vector3 smallTickPos = new Vector3(tickXPos, rulerBottom, transform.position.z);
            Instantiate(smallTickPrefab, smallTickPos, Quaternion.identity, ticksSceneParent);
        }
    }

    private void OnMouseDrag()
    {
        if (TimeLine.Instance.isPlaying)
        {
            TimeLine.Instance.StopPlaying();
        }
        
        if (MyCursor.Instance != null)
            MyCursor.Instance.SwitchToHoldingCursor();

        Vector3 mouseWorldPos = TimeLine.Instance.timeLineCamera.ScreenToWorldPoint(Input.mousePosition);
        float clickXPos = mouseWorldPos.x;

        TimeLine.Instance.PositionPointerHead(clickXPos);
    }

    private void OnMouseUp()
    {
        if (MyCursor.Instance != null)
            MyCursor.Instance.SwitchToNormalCursor();
        if (Tutorial.Instance != null)
        {
            Tutorial.Instance.RulerDragged();
        }
    }
}