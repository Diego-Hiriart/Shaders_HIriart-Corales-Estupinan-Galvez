using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CueStickController : MonoBehaviour
{
    private float force = 0.5f;//Force with with stick is moved to hit the ball
    private float step = 0.001f;//Value of increments and decrements when changing the force
    private float rotationspeed = 0.3f;//Speed with which the stick roatates around the ball
    [SerializeField]
    private GameObject whiteBall;//Target, to where the the stick must be pushed
    [SerializeField]
    private GameObject Canvas;
    private UIController UI;//To change the force display
    private Vector3 posOffset;//Initial position diference between cue tip and white ball
    [SerializeField]
    private GameController gameGontrol;

    [SerializeField]
    private GameObject rayEmitter;//Point from which to emit the ray
    [SerializeField]
    private LayerMask standardBallLayer;//Layer the raycast must hit

    private StandardBall previousBall;//To store which was the previous ball hit by the ray
    private bool firstHit = true;//Used for the first raycast hit
    private bool hitting = false;//Used to highlight again after the raycast moves something else to a ball

    private void Start()
    {
        this.UI = this.Canvas.GetComponent<UIController>();
        this.UpdateForceIndicator();
        this.OffsetCalculation();        
    }

    private void BallHighlight()
    {
        RaycastHit impact = new RaycastHit();
        //Only if "StandardBall" layer is hit
        if (Physics.Raycast(this.rayEmitter.transform.position, this.rayEmitter.transform.forward, out impact, 15, standardBallLayer))//Emit a ray forward from the stick tip
        {
            //If this is the first ball hit, use that ball as previouBall and highlight it
            if (firstHit)
            {
                this.previousBall = impact.collider.gameObject.GetComponent<StandardBall>();//Update previousBall to be this first ball
                this.previousBall.Highlight(true);//Highlight the first ball
                this.firstHit = false;
            }
            StandardBall impactedBall;
            //Impacted ball might have fallen through a hole and gotten destroyed, thats why TryGetComponent is needed to avoid errors
            if (this.previousBall == null)//If the previous ball has been destroyed, dont interact with it, not even in the if statement
            {
                impact.collider.gameObject.TryGetComponent<StandardBall>(out impactedBall);
                if (impactedBall != null)//These impacted balls are always null for some reason, so this is needed
                {
                    impactedBall.Highlight(true);//highlight new ball
                }                
                impact.collider.gameObject.TryGetComponent<StandardBall>(out previousBall);//Update previousBall
            }
            else//This else if separation is needed since TryGetComponent has to be used for the object used in the if comparison
            {
                impact.collider.gameObject.TryGetComponent<StandardBall>(out impactedBall);
                //If a new ball is hit (or it wasnt hitting and enters this raycast if), return previous ball to default material, highlight new ball, and update previousBall field/atribute
                if (impactedBall.GetBallNumber() != this.previousBall.GetBallNumber() || !this.hitting)
                {
                    this.previousBall.Highlight(false);//Remove previousBall highlight            
                    impact.collider.gameObject.TryGetComponent<StandardBall>(out impactedBall);//highlight new ball
                    if (impactedBall != null)//These impacted balls are always null for some reason, so this is needed
                    {
                        impactedBall.Highlight(true);//highlight new ball
                    }
                    impact.collider.gameObject.TryGetComponent<StandardBall>(out previousBall);//Update previousBall
                    this.hitting = true;
                }              
            }
        }
        else//If not hitting a ball, paint nothing
        {
            if (previousBall != null)
            {
                previousBall.Highlight(false);
                this.hitting = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.gameGontrol.IsGameWon())//Do stuff only if the player has not won yet
        {
            //Raycast call
            BallHighlight();

            if (Input.GetKey(KeyCode.W))//Increase force
            {
                if (this.force < 1 && !(this.force + step > 1))//Force cant go over 1
                {
                    this.force += step;
                    this.UpdateForceIndicator();
                }
            }

            if (Input.GetKey(KeyCode.S))//Decrease force
            {
                if (this.force > 0.01 && !(this.force - step < 0.01))//Dont allow negative force
                {
                    this.force -= step;
                    this.UpdateForceIndicator();
                }
            }

            if (Input.GetKey(KeyCode.A))//Rotate stick counterclockwise
            {
                this.transform.RotateAround(this.whiteBall.transform.position, Vector3.up, -rotationspeed);
            }

            if (Input.GetKey(KeyCode.D))//Rotate stick clockwise
            {
                this.transform.RotateAround(this.whiteBall.transform.position, Vector3.up, rotationspeed);
            }

            if (Input.GetKey(KeyCode.Space))//Move stick
            {
                //Move the stick towards the ball to push it
                this.transform.Translate(DirectionCalculator() * force * 2 * Time.deltaTime, Space.World);
            }

            if (Input.GetKey(KeyCode.R))//Reset the stick so that it is in an ideal position to hit the ball
            {
                this.FollowWhiteBall();
            }
        }    
    }

    //Calculate the position the stick should have relative to the white ball when wanting to shoot (move) the ball
    private void OffsetCalculation()
    {
        this.posOffset = this.transform.position - this.whiteBall.transform.position;
    }

    //Calculate what direction the stick must move towards to hit the ball
    private Vector3 DirectionCalculator()
    {
        return new Vector3(this.whiteBall.transform.position.x - this.transform.position.x, 0,
                this.whiteBall.transform.position.z - this.transform.position.z);
    }

    //Move the cue stick to a position where it can hit the ball and be rotated around it
    private void FollowWhiteBall()
    {
        this.transform.rotation = new Quaternion(0.719339788f, 0, 0, 0.694658399f);//Reset stick rotation
        this.transform.position = this.whiteBall.transform.position + posOffset;//Move the cue so it can hit the ball easily
    }

    //Tell the UI to update the force displayed on the screen
    private void UpdateForceIndicator()
    {
        this.UI.UpdateForceDisplay(this.force);
    }
}
