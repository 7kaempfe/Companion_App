using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Counter : MonoBehaviour
{
    public Text CounterText;
    public static int wuerfe;

    void Start()
    {
        wuerfe = 0;
    }

    /*
    public void increment()
    {
        wuerfe++;
    }
    */

    void OnGUI()
    {
        CounterText.text = wuerfe.ToString();
    }

}