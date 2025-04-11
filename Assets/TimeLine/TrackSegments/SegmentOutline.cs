using System;
using UnityEngine;

public class SegmentOutline : MonoBehaviour
{
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField] private float outlineWidth = 0.1f;
    [SerializeField] private Color outlineColorClicked = Color.red;
    [SerializeField] private float outlineWidthClicked = 0.2f;
    
    private Material outlineMaterial;
    private bool isClicked = false;

    private void Awake()
    {
        outlineMaterial = GetComponent<SpriteRenderer>().material;
        outlineMaterial.SetColor("_OutlineColor", outlineColor);
        outlineMaterial.SetColor("_ClickedColor", outlineColorClicked);
        outlineMaterial.SetFloat("_OutlineThickness", outlineWidth);
        outlineMaterial.SetFloat("_ClickedThickness", outlineWidthClicked);
        outlineMaterial.SetFloat("_Clicked", 0f); // Not clicked initially
    }

    private void OnMouseDown()
    {
        if (isClicked)
        {
            isClicked = false;
            outlineMaterial.SetFloat("_Clicked", 0f); // Reset clicked state
            TimeLine.Instance.SelectTrackSegment(null);
            return;
        }
        isClicked = true;
        outlineMaterial.SetFloat("_Clicked", 1f); // Set clicked state
        TimeLine.Instance.SelectTrackSegment(GetComponent<TrackSegment>());
    }
    
    public void ResetOutline()
    {
        isClicked = false;
        outlineMaterial.SetFloat("_Clicked", 0f); // Reset clicked state
    }
}
