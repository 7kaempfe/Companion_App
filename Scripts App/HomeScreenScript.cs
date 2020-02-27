using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Dieses Script zeigt den Namen und das Bild des Nutzers auf dem Homescreen an.
public class HomeScreenScript : MonoBehaviour
{

    public RawImage profilePicture;
    RestAPI restAPI;
    public Text textField;

    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();
        textField.text = RestAPI.FirstNameGlobal + " " + RestAPI.SurnameGlobal;
        restAPI.GetUser(RestAPI.FirstNameGlobal, RestAPI.SurnameGlobal, RestAPI.PasswordGlobal, UserCallback);
    }

    // Startet den Download des Profilbilds
    public void UserCallback(RestAPI.User user)
    {
        StartCoroutine(DownloadPicture(user.bildid));
    }

    // Lädt ein Bild mit der mitgegebenen ID runter von der Datenbank und setzt dieses als Profilbild 
    public IEnumerator DownloadPicture(int pictureID)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={RestAPI.FirstNameGlobal}&name={RestAPI.SurnameGlobal}&passwort={RestAPI.PasswordGlobal}&pictureID={pictureID}");
        yield return request.SendWebRequest();
        Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            profilePicture.texture = tex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
