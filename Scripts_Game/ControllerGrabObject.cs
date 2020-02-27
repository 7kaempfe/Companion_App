using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ControllerGrabObject : MonoBehaviour
{

    public BallSpawner ballSpawner;
    public Counter counter;
    public GameObject ballPosition;
    public GameObject ballBox;
    // for setting the speed threshold when the ball can be released at all
    public float controllerSpeed_Threshold;
    public float speedBoost;
    // number of last measures speeds which are used for averaging
    public int averageOver;
    public GameObject player;
    // for defining r = release automatically or t = trigger release as controller mode
    public string controllerMode;
    public static bool gewonnen = false;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private SteamVR_TrackedObject trackedObj;
    private Rigidbody ballRigidBody;
    private ReplaceBalls replaceBalls;

    private int previousInstanceID;

    // measure speed of controller from previous + current frame
    private float previousSpeed;
    private float currentSpeed;
    private List<float> lastSpeeds;

    private GameObject collidingObject;
    private GameObject objectInHand;

    // -----------------

    private LinkedList<Vector3> previousPositions;



    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        replaceBalls = ballBox.GetComponent<ReplaceBalls>();
        lastSpeeds = new List<float>();
        objectInHand = null;
        // set trigger release as default controller mode
        controllerMode = "r";

        previousPositions = new LinkedList<Vector3>();
    }

    private void SetCollidingObject(Collider col)
    {
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "firstBall" || other.tag == "ball")
        {
            Tutorial.isGrabbed = true;
            SetCollidingObject(other);
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.tag == "firstBall" || other.tag == "ball")
            SetCollidingObject(other);
    }

    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        objectInHand = collidingObject;
        collidingObject = null;

        // deactivate Rigidbody to avoid buggy collisions with other balls
        ballRigidBody = objectInHand.GetComponent<Rigidbody>();
        //StartCoroutine(temporarilyDisableRigidbody(ballRigidBody));

        objectInHand.transform.position = ballPosition.transform.position;
        //var joint = AddFixedJoint();
        //joint.connectedBody = objectInHand.GetComponent<Rigidbody>();


        ballRigidBody.isKinematic = true;
        objectInHand.transform.parent = transform;


    }

    private IEnumerator temporarilyDisableRigidbody(Rigidbody ballRigidBody)
    {
        ballRigidBody.detectCollisions = false;
        yield return new WaitForSeconds(0.5f);
        ballRigidBody.detectCollisions = true;
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    private void ReleaseObject(Vector3 ReleaseVelocity)
    {
        objectInHand.transform.parent = null;
        ballRigidBody.isKinematic = false;
        ballRigidBody.velocity = ReleaseVelocity * speedBoost;

        objectInHand = null;
        // leere Ballliste in ReplaceBalls, wenn Ball geworfen wurde (spart Speicherplatz)
        replaceBalls.emptyBallList();
        // ballSpawner.spawn();
        if (!gewonnen)
        {
            Counter.wuerfe += 1;
        }

        // start the timer only at first throw
        if (Counter.wuerfe == 1)
        {
            GameManager.startTimer = true;
            //Debug.Log("ein Wurf");
        }
    }

    void FixedUpdate()
    {

        if (controllerMode == "t")
        {
            if (Controller.GetHairTriggerDown())
            {
                if (collidingObject && !objectInHand)
                {
                    GrabObject();
                }

                else if (objectInHand)
                {
                    ReleaseObject(transform.parent.rotation * Controller.velocity);
                }
            }

            if (Controller.GetHairTriggerUp())
            {
                if (objectInHand)
                {
                    ReleaseObject(transform.parent.rotation * Controller.velocity);
                }
            }
        }

        else if (controllerMode == "r")
        {
            // Take up ball on contact
            if (collidingObject && !objectInHand && collidingObject.GetInstanceID() != previousInstanceID)
            {
                //Debug.Log("Grab Object");

                if (collidingObject.transform.parent != null)
                    return;

                GrabObject();
                InitSpeedlist();
                previousInstanceID = objectInHand.GetInstanceID();

            }

            if (objectInHand)
            {

                if (Controller.GetHairTriggerDown())
                {
                    ReleaseObject(Controller.velocity);
                }
                /// TODO Hier fehlen noch die Time.deltaTime Werte...
                /// Velocity muss mit CameraRig Rotation verrechnet werden
                int j = 0; float sum = 0;
                var item = previousPositions.First.Next;
                do
                {
                    sum += Vector3.Magnitude(item.Previous.Value - item.Value);
                    j++;
                }
                while ((item = item.Next) != null && j < 10);

                previousSpeed = sum / j;

                var velocity = (objectInHand.transform.position - previousPositions.First.Value);

                previousPositions.AddFirst(objectInHand.transform.position);

                currentSpeed = Vector3.Magnitude(velocity);

                // Throw ball when speed diminishes (without controller press)
                // speed has to be higher than the set threshold
                // ball has to be throwable, i.e. not in ball box collider

                Debug.DrawRay(transform.position, Controller.velocity, Color.cyan);
                Debug.DrawRay(objectInHand.transform.position, velocity, Color.cyan);

                if (currentSpeed < previousSpeed && (previousSpeed + currentSpeed) / 2 > controllerSpeed_Threshold)
                {
                    if (BallIsThrowable())
                    {

                        // Velocity nach CameraRig anpassen

                        ReleaseObject(transform.parent.rotation * Controller.velocity);                          

                        ToggleThrowable();
                        //InitSpeedlist();
                    }
                }
            }
        }
    }

    private void InitSpeedlist()
    {
        lastSpeeds.Clear();
        previousPositions.Clear();
        float initialSpeed = Controller.velocity.magnitude;
        // fill list with given number of speeds (averageOver parameter)
        for (int i = 0; i < averageOver; i++)
        {
            lastSpeeds.Add(initialSpeed);
            previousPositions.AddFirst(objectInHand.transform.position);
        }
        previousSpeed = AverageSpeed();
    }

    private Boolean BallIsThrowable()
    {
        if (ControllerDirectionRight())
        {
            if (this.name == "Controller (left)")
            {
                if (SetBallsToThrowable.throwableLeft)
                {
                    return true;
                }
            }
            else if (this.name == "Controller (right)")
            {
                if (SetBallsToThrowable.throwableRight)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // check if the Controller moves in the right direction for ball release
    private bool ControllerDirectionRight()
    {
        Vector3 lookVector = player.transform.forward;
        //Debug.Log("look vector: " + lookVector.ToString());
        Vector3 throwVector = transform.parent.rotation * Controller.velocity;
        Debug.Log("Controller: " + gameObject.name);
        //Debug.Log("throw vector: " + throwVector.ToString());
        if (Vector3.Dot(lookVector, throwVector) > 0.4)// && Controller.velocity.x <= -Controller.velocity.z)
        {
            return true;
        }
        else
        {
            return false;
        }
        //return true;
    }


    private void ToggleThrowable()
    {
        if (this.name == "Controller (left)")
        {
            if (SetBallsToThrowable.throwableLeft)
            {
                SetBallsToThrowable.throwableLeft = false;
            }
        }
        else if (this.name == "Controller (right)")
        {
            if (SetBallsToThrowable.throwableRight)
            {
                SetBallsToThrowable.throwableRight = false;
            }
        }
    }

    // Calculate Average Speed over paramter averageOver
    private float AverageSpeed()
    {
        float averageSpeed = 0.0f;
        for (int i = 0; i < averageOver; i++)
        {
            averageSpeed += lastSpeeds[i];
            //Debug.Log(lastSpeeds[i].ToString());
        }
        return (averageSpeed / averageOver);
    }
}

