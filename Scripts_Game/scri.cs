using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class scri : MonoBehaviour
{
    public Text Zeit;
    public float startTime = 0;
    public int stageTime = 0;
    private int min;
    private int sec;
    public bool gameFinished = false;
    public bool gameStarted = false;

    private int startcount = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if (gameStarted && startcount == 0)
        {
            startcount += 1;
            //Debug.Log("enter gameStarted");
            startTime = Time.time;
        }
        else if (gameStarted && startcount != 0 && !gameFinished)
        {
            stageTime = (int)(Time.time - startTime);
            //Debug.Log("enter gameStarted + running");
            min = Mathf.FloorToInt(stageTime / 60);
            sec = Mathf.FloorToInt(stageTime % 60);
        }
    }

    void OnGUI()
    {
        Zeit.text = min.ToString() +":"+ sec.ToString("00");
    }

}