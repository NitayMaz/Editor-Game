using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FixedAspectRatio : MonoBehaviour
{
    private int screenWidth;
    private int screenHeight;
    
    private void Start()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        AdjustResolution();
    }

    private void Update()
    {
        if (Screen.width != screenWidth || Screen.height != screenHeight)
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            AdjustResolution();
        }
    }

    private void AdjustResolution()
    {
        float targetAspect = 16f / 9f;
        float windowAspect = (float)Screen.width / Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        Camera camera = GetComponent<Camera>();

        if (scaleHeight < 1.0f)
        {
            // Add letterbox (horizontal black bars)
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            camera.rect = rect;
        }
        else
        {
            // Add pillarbox (vertical black bars)
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = camera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}