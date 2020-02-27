using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public Rigidbody ballPrefab;
    private Rigidbody firstBall;

    private void Awake()
    {
        firstBall = spawn();
        firstBall.tag = "firstBall";
    }

    void Start()
    {
        StartCoroutine(spawnFifteen());
    }
    public Rigidbody spawn()
    {
        Rigidbody ballInstance;
        ballInstance = Instantiate(ballPrefab);
        ballInstance.transform.position = transform.position;
        return ballInstance;
    }

    public void spawnXBalls(int x)
    {
        // Debug.Log("Coroutine started");
        StartCoroutine(spawnX(x));
    }

    private IEnumerator spawnFifteen()
    {
        for (int i = 0; i < 15; i++)
        {
            spawn();
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }

    private IEnumerator spawnX(int x)
    {
        for (int i = 0; i < x; i++)
        {
            spawn();
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }
}
