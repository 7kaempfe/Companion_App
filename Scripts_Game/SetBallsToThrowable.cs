using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Used to set balls as throwable for ControllerGrabObject script
public class SetBallsToThrowable : MonoBehaviour
{
    public static bool throwableRight;
    public static bool throwableLeft;

    public void OnTriggerExit(Object controller) 
    {
        if(controller.name == "Controller (right)")
        {
            throwableRight = true;
            //Debug.Log("Throwable right");
        }

        else if (controller.name == "Controller (left)")
        {
            throwableLeft = true;
            //Debug.Log("Throwable left");
        }
    }
}
