using System;
using System.Collections;
using UnityEngine;

public class SoccerKid : TrackControlled
{
    [SerializeField]private float kickAngleAboveGround = 30f;
    [SerializeField]private float kickForceModifier = 1f;
    [SerializeField]private Transform ballTransform;
    [SerializeField]private Rigidbody2D ballRB;
    [SerializeField]private float minXForFail;
    [SerializeField] private float ballVelocityForFail;
    [SerializeField] private float delayBeforeWinAnim = 0.5f;
    private Vector3 originalBallPosition;
    private float speedX = 0f;
    private float lastXPosition = 0f;
    private bool kickedBall = false;
    private bool scored = false;

    private void Start()
    {
        originalBallPosition = ballTransform.position;
        lastXPosition = transform.position.x; 
    }

    private void LateUpdate()
    {
        speedX = (transform.position.x - lastXPosition) / Time.deltaTime;
        lastXPosition = transform.position.x;
        
        //check failure
        if (kickedBall && !scored && (ballTransform.position.x > minXForFail || ballRB.linearVelocity.magnitude < ballVelocityForFail))
        {
            animator.SetBool("Fail", true);
            StageManager.Instance.StageFailed();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log("adding force to: " + other.name);
        Vector2 kickDirection = new Vector2(speedX, Mathf.Tan(kickAngleAboveGround * Mathf.Deg2Rad) * speedX);
        ballRB.AddForce(kickDirection * speedX * kickForceModifier, ForceMode2D.Impulse);
        kickedBall = true;
        SoundManager.Instance.PlayAudio(AudioClips.BallKick);
        base.StartInteraction();
    }
    
    public override void StopInteraction()
    {
        if(originalBallPosition != Vector3.zero) //if for some reason this is called before start, than ball position would be zero and we obv don't want that
            ballTransform.position = originalBallPosition; 
        ballRB.linearVelocity = Vector2.zero;
        ballRB.angularVelocity = 0f;
        ballRB.Sleep();
        GetComponent<Animator>().SetBool("Goal", false);
        kickedBall = false;
        scored = false;
        base.StopInteraction();
    }

    public void Goal()
    {
        scored = true;
        Invoke(nameof(PlayWinAnimation), delayBeforeWinAnim);
    }

    private void PlayWinAnimation()
    {
        GetComponent<Animator>().SetBool("Goal", true);
    }

}
