using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    [SerializeField] ParticleSystem carCollisionEffect;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!TimeLine.Instance.isPlaying || other.gameObject.CompareTag("WinArea"))
            return;
        carCollisionEffect.transform.position = transform.position;

        carCollisionEffect.Play(); //particle!
        StageManager.Instance.StageFailed();
    }
}
