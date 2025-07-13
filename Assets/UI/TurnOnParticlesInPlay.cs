using UnityEngine;

public class TurnOnParticlesInPlay : MonoBehaviour
{

    private ParticleSystem particleSystem;

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (TimeLine.Instance.isPlaying)
        {
            if (!particleSystem.isPlaying)
            {
                Debug.Log("Playing particles");
                particleSystem.Play();
            }
        }
        else
        {
            if (particleSystem.isPlaying)
            {
                Debug.Log("Stopping particles");
                particleSystem.Stop();
            }
        }
    }
}
