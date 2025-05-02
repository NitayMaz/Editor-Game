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
    }

    private void PositionTicks()
    {
        float timeCovered = gameObject.GetComponent<SpriteRenderer>().bounds.size.x / TimeLine.Instance.trackLengthFor1Second;
        float rulerBottom = gameObject.GetComponent<SpriteRenderer>().bounds.min.y;
        HashSet<float> bigTickPositions = new HashSet<float>();
        // position large ticks
        for (float i = 0; i <= timeCovered; i += bigTickFrequency)
        {
            float tickXPos = transform.position.x + i * TimeLine.Instance.trackLengthFor1Second;
            bigTickPositions.Add(tickXPos);
            Vector3 bigTickPos = new Vector3(tickXPos, rulerBottom, transform.position.z);
            GameObject bigTick = Instantiate(bigTickPrefab, bigTickPos, Quaternion.identity, ticksSceneParent);
                //          if(i == 0) 
//                bigTick.GetComponentInChildren<TextMeshPro>().enabled = false; // no text for first tick
  //          bigTick.GetComponentInChildren<TextMeshPro>().text = i.ToString("0.00"); // format to 2 decimal places
        }
        // position small ticks
        for (float i = 0; i <= timeCovered; i += smallTickFrequency)
        {
            float tickXPos = transform.position.x + i * TimeLine.Instance.trackLengthFor1Second;
            if (bigTickPositions.Contains(tickXPos))
                continue;
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
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float clickXPos = mouseWorldPos.x;
        
        TimeLine.Instance.PositionPointerHead(clickXPos);
    }

    
}
