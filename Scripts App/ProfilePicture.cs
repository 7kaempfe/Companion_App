using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dieses Script sorgt dafür, dass ein Profilbild in die Datenbank hochgeladen wird.
// !Work in progress! Da man im Moment keine User auf der Datenbank verändern kann gestaltet sich Implementation schwierig.
public class ProfilePicture : MonoBehaviour
{
    RestAPI restAPI;
    FotoListeScript fotoListeScript;
    public static int ProfilePictureID;
    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();
        fotoListeScript = this.GetComponent<FotoListeScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Geht mit der Funktion PickImage in die Gallerie, das ausgewählte Bild wird in die Datenbank hochgeladen.
    public void PostProfilePicture()
    {
        fotoListeScript.PickImage();
        StartCoroutine(restAPI.GetPictures(ImageCallback));
    }

    // Setzt ProfilePictureID auf den Wert des zuletzt hochgeladenen Bildes, also des Profilbildes in diesem Fall.
    public void ImageCallback(RestAPI.Picture[] pictures)
    {
        ProfilePictureID = pictures[pictures.Length-1].id;
    }
}
