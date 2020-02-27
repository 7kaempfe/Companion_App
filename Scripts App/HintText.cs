using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Setzt den hintText in der Start Funktion.
public class HintText : MonoBehaviour
{
    public Text hintText;


    // Start is called before the first frame update
    private void Start()
    {
        SetText();
    }
    
    // Setzt den Text des hintText
    void SetText()
    {
        hintText.text = "Hier finden Sie die Musik, die in die Spiele von " + RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal + " eingebunden werden.";
    }

    // Update is called once per frame
    void Update()
    {
        //SetText();
    }
}
