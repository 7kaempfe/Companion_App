using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableBallTrigger : MonoBehaviour
{
    public GameObject gameField;

    private void OnTriggerEnter(Collider ball)
    {
        int instanceID = ball.GetInstanceID();
        //Debug.Log(instanceID.ToString());
        gameField.GetComponent<GameManager>().thrownBalls.Add(instanceID);

    }
}
