using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Setzt den 
public class MusicText : MonoBehaviour
{
    public GameObject musicText;


    // Start is called before the first frame update
    private void Start()
    {
        SetText();
    }
    void SetText()
    {
        Text temp = musicText.GetComponent<Text>();
        temp.text = "Hier finden Sie die Musik, die in die Spiele von " + RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal + " eingebunden werden.";
    }

    // Update is called once per frame
    void Update()
    {
        //SetText();
    }
}
