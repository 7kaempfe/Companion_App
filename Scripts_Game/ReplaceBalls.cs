using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceBalls : MonoBehaviour
{
    public GameObject ballSpawn;

    private List<int> ballsRemoved = new List<int>();
    private int ballCounter = 0;
    private BallSpawner ballSpawner;

    private void Start()
    {
        ballSpawner = ballSpawn.GetComponent<BallSpawner>();
        // Debug.Log("Instantiate ReplaceBalls");
        InvokeRepeating("NumberChecker", 5.0f, 5.0f);
    }

    private void OnTriggerExit(Collider ball)
    {
        int ballInstanceID = ball.GetInstanceID();
        if (ball.tag == "ball" && !(ballsRemoved.Contains(ballInstanceID)))
        {
            ballsRemoved.Add(ball.GetInstanceID()); 
            ballCounter += 1;
            // Debug.Log(ball.GetInstanceID().ToString());
        }
    }

    private void NumberChecker()
    {
        // Debug.Log("Spawn " + ballCounter.ToString() + " now");
        ballSpawner.spawnXBalls(ballCounter);
        ballCounter = 0;
    }

    public void emptyBallList()
    {
        ballsRemoved.Clear();
    }
}
