using System;
using System.Collections;
using UnityEngine;

public class TimeLine : MonoBehaviour
{
    public static TimeLine Instance;
    [SerializeField] private Track[] tracks;
    [SerializeField] private GameObject pointerHead;
    
    public float trackLengthFor1Second = 5f;
    
    private float currentTime = 0;
    private bool isPlaying = false;
    private Coroutine sceneCoroutine;
    
    private SpriteRenderer spriteRenderer;
    private float leftEdgeXvalue;
    
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Trying to Initialize Multiple TimeLines, That's exactly how marvel failed.");
            Destroy(this.gameObject);
        }
        Instance = this;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        leftEdgeXvalue = spriteRenderer.bounds.min.x;
        ResetTimeLine();
    }
    
    private void ResetTimeLine()
    {
        isPlaying = false;
        currentTime = 0;
        MovePointerHeadX(leftEdgeXvalue);
    }
    
    public void PlayScene()
    {
        if(isPlaying)
            return;
        isPlaying = true;
        sceneCoroutine = StartCoroutine(RunScene());
    }
    
    public void PauseScene()
    {
        StopCoroutine(sceneCoroutine);
        ResetTimeLine();
    }

    private IEnumerator RunScene()
    {
        float maxTrackLength = 0;
        foreach (var track in tracks)
        {
            float trackLength = track.GetTrackLength();
            if (trackLength > maxTrackLength)
                maxTrackLength = trackLength;
        }
        currentTime = 0;
        while (currentTime < maxTrackLength)
        {
            MovePointerHeadX(leftEdgeXvalue + (currentTime * trackLengthFor1Second));
            currentTime += Time.deltaTime;
            foreach (var track in tracks)
            {
                track.ApplyTrackPosition(currentTime);
            }
            yield return null;
        }
        isPlaying = false;
    }
    
    private void MovePointerHeadX(float xValue)
    {
        pointerHead.transform.position = new Vector3(xValue, pointerHead.transform.position.y, pointerHead.transform.position.z);
    }
}
