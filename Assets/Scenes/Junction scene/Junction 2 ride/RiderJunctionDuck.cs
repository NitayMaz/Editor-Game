using UnityEngine;

public class RiderJunctionDuck : TrackControlled
{
    public override void StartInteraction()
    {
        return; //no interaction for rider for now, he just keeps going while the duck lies there. what a monster.
    }

    public override void StopInteraction()
    {
        base.StopInteraction();
    }
}
