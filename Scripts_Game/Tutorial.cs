using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    public Material pulsateMaterial;

    public static bool isGrabbed;
    public Material ballMat;
    public static bool newGame;

    private Color pulsateColor;
    private GameObject firstBall;
    private Renderer rend;

    //public static bool isGrabbed;


    void Update()
    {
        if (newGame)
        {
            firstBall = GameObject.FindGameObjectWithTag("firstBall");
            pulsateColor = pulsateMaterial.color;

            //StartCoroutine("MakeBallPulsate");
            rend = firstBall.GetComponent<Renderer>();
            rend.material = pulsateMaterial;

            if (!isGrabbed)
            {
            rend.sharedMaterial.color = 
                Color.Lerp(pulsateColor, 
                new Color(pulsateColor.r, pulsateColor.g, pulsateColor.b, 0.3f + Mathf.PingPong(Time.time, 0.7f)), 
                6.0f);
            }
            else
            {
                rend.sharedMaterial = ballMat;
                //new Color(pulsateColor.r, pulsateColor.g, pulsateColor.b, 1.0f);
                newGame = false;
            }
        }
    }

    /*private IEnumerator MakeBallPulsate()
    {
        rend = firstBall.GetComponent<Renderer>();
        rend.material = pulsateColor;
        for (int i = 0; i++; i<=50)
        {
        yield return new WaitForSeconds(0.1f);
            float alpha = pulsateColor.color.a;

        }

        yield return null;
    }*/
}
