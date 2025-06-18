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
    private Vector3 originalBallPosition;
    private float speedX = 0f;
    private float lastXPosition = 0f;
    private bool kickedBall = false;

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
        if (kickedBall && (ballTransform.position.x > minXForFail || ballRB.linearVelocity.magnitude < ballVelocityForFail))
        {
            animator.SetBool("Fail", true);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Debug.Log("adding force to: " + other.name);
        Vector2 kickDirection = new Vector2(speedX, Mathf.Tan(kickAngleAboveGround * Mathf.Deg2Rad) * speedX);
        ballRB.AddForce(kickDirection * speedX * kickForceModifier, ForceMode2D.Impulse);
        kickedBall = true;
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
        base.StopInteraction();
    }

    public void Goal()
    {
        GetComponent<Animator>().SetBool("Goal", true);
    }

}
