using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Diese Script sorgt dafür das auch durch Betätigung der Enter-Taste gesucht werden kann in der MusikSuch-Scene.
public class OnKeyDownScript : MonoBehaviour
{
    public GameObject scriptholder;
    public InputField inputfield;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Detect when the Return key is pressed down
        if (Input.GetKeyDown(KeyCode.Return))
        {
            var searchMusicScript = scriptholder.GetComponent<SearchMusic>();
            if (inputfield.text == "")
            {
                Debug.Log("Bitte gib etwas in die Suchleiste ein");
            }
            else
            {
                searchMusicScript.SearchForMusic();
            }
        }

        //Detect when the Return key has been released
        if (Input.GetKeyUp(KeyCode.Return))
        {
            Debug.Log("Return key was released.");
        }
    }
}
