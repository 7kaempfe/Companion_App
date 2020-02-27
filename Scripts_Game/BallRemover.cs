using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallRemover : MonoBehaviour
{
    private BoxCollider ballRemover;
    private List<int> ballsToRemove = new List<int>();

    private void OnTriggerEnter(Collider ball)
    {
        int instanceID = ball.GetInstanceID();
        // Debug.Log("script called");
        if (ball.tag == "ball")
        {
            // Debug.Log("entered");
            ballsToRemove.Add(instanceID);
            StartCoroutine(DestroyBall(ball, instanceID));
        }
    }

    private IEnumerator DestroyBall(Collider ball, int instanceID)
    {
        yield return new WaitForSeconds(3);
        if (ballsToRemove.Contains(instanceID))
        {
            ballsToRemove.Remove(instanceID);
            Destroy(ball.gameObject);
        }

    }

    private void OnTriggerExit(Collider ball)
    {
        int instanceID = ball.GetInstanceID();
        if (ballsToRemove.Contains(instanceID))
        {
            ballsToRemove.Remove(instanceID);
        }
    }
}
