using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// In diesem Script wird ein Interesse in die Datenbank hochgeladen und es wird auch benutzt um Interessen zu bearbeiten.
public class InterestUploadScript : MonoBehaviour
{
    public InputField designationField;
    public InputField descriptionField;
    public static int ID;
    RestAPI restAPI;

    // Start is called before the first frame update
    // Sollte eine ID zugewiesen worden sein will der Nutzer ein Interesse bearbeiten, also wird dieses aufgerufen.
    void Start()
    {
        Debug.Log(InterestUploadScript.ID);
        restAPI = this.GetComponent<RestAPI>();
        if(ID != 0)
        {
            StartCoroutine(restAPI.GetInterests(InterestCallback));
                
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Dem designationField und dem descriptionField werden die Daten aus der Datenbank zugewiesen.
    private void InterestCallback(RestAPI.Interest[] interests)
    {
        foreach (var i in interests)
        {
            Debug.Log(ID);
            Debug.Log(i.id);
            if (i.id == ID)
            {
                Debug.Log(i.beschreibung);
                Debug.Log(i.bezeichnung);
                designationField.text = i.bezeichnung;
                descriptionField.text = i.beschreibung;
            }
        }
    }

    // Ein Interesse wird hochgeladen, sollte ID == 0 sein, bedeutet dies dass ein neues hochgeladen wird und ansonsten wird eines bearbeitet.
    public void UploadInterest()
    {
        if (ID == 0)
        {
            Debug.Log(designationField.text);
            Debug.Log(descriptionField.text);
            StartCoroutine(restAPI.PostInterests(designationField.text, descriptionField.text));
        }
        else
        {
            StartCoroutine(restAPI.UpdateInterest(ID, designationField.text, descriptionField.text));
            ID = 0;
        }
    }


}
