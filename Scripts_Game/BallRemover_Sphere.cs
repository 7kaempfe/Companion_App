using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRemover_Sphere : MonoBehaviour
{

    private void OnTriggerExit(Collider ball)
    {
        Destroy(ball);
    }
}
