using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// In diesem Script wird ein Interesse von der Datenbank runtergeladen und deren Bezeichnungstext und Beschreibungstext angezeigt.
public class InterestDescriptionScript : MonoBehaviour
{
    RestAPI restAPI;
    public Text designationText;
    public Text descriptionText;
    public static int ID;
    public GameObject popUpPanel;
   
    // Start is called before the first frame update
    void Start()
    {
        if (designationText != null)
        {
            restAPI = this.GetComponent<RestAPI>();
            StartCoroutine(restAPI.GetInterests(InterestCallback));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Schickt die ID dieses Scripts an das InterestUploadScript (Im Falle einer Bearbeitung).
    public void SendID()
    {
        Debug.Log(ID);
        InterestUploadScript.ID = ID;
        Debug.Log(InterestUploadScript.ID);
    }

    // sucht das gesuchte Interesse aus allen gefundenen heraus und zeigt dessen Inhalt an.
    private void InterestCallback(RestAPI.Interest[] interests)
    {
        foreach (var i in interests)
        {
            if (i.id == ID)
            {
                designationText.text = i.bezeichnung;
                descriptionText.text = i.beschreibung;
            }
        }
    }

    // Deaktiviert das Löschen-PopUp und löscht das Interesse.
    public void DeleteInterest()
    {
        popUpPanel.SetActive(false);
        StartCoroutine(restAPI.DeleteInterest(ID));
    }

    // Aktiviert das Löschen-PopUp
    public void SetPopUpPanelActive()
    {
        popUpPanel.SetActive(true);
    }

    // Deaktiviert das Löschen-PopUp
    public void SetPopUpPanelInactive()
    {
        popUpPanel.SetActive(false);
    }
}
