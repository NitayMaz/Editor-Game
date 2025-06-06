using System;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        StageManager.Instance.StageSuccess();
    }
}
