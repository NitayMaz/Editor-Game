using UnityEngine;

public class Duck_DuckScene : TrackControlled
{
    [SerializeField] ParticleSystem duckExplodes;
    [SerializeField] Vector2 duckExplodesOffset;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TimeLine.Instance.isPlaying)
            return;
        if (other.gameObject.CompareTag("WinArea"))
            return;
        duckExplodes.transform.position = transform.position + (Vector3)duckExplodesOffset;

        duckExplodes.Play(); //particle!

        StartInteraction();
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        gameObject.SetActive(false);
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
        gameObject.SetActive(true);
    }
}