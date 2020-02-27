using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.IO;

// In diesem Script sind die Funktionen zur Interaktion mit der Datenbank.
public class RestAPI : MonoBehaviour
{
    public InputField firstNameField;
    public InputField surnameField;
    public InputField passwordField;
    public InputField passwordField2;
    public static string FirstNameGlobal;
    public static string SurnameGlobal;
    public static string PasswordGlobal;
    public static string GenderGlobal;
    public Dropdown dropDownField;
    SceneSwitcher sceneSwitcher;
    private int lastCreatedSong = -1;
    public Song[] songsGlobal;
    public GameObject errorText;
    public GameObject passwordErrorText;
    public Texture2D texture;
    public Texture2D testTexture;
    public RawImage testRawImage;

    public void Start()
    {
        sceneSwitcher = this.GetComponent<SceneSwitcher>();
    }

    // Erzeugt einen Nutzer in der Datenbank, mit den gegebenen Daten aus den Textfeldern.
    public void CreateUser()
    {
        if (passwordField.text == passwordField2.text && passwordField.text != "" && ContainsOnlyAlphaNumericCharacters(passwordField.text))
        {
            StartCoroutine(PostUser(firstNameField.text, surnameField.text, passwordField.text, dropDownField.captionText.text, ProfilePicture.ProfilePictureID));
            sceneSwitcher.goToHomeScreen();
        }
        else
        {
            Debug.Log("Die Passwörter stimmen nicht überein");
            passwordErrorText.SetActive(true);
        }
        if (!ContainsOnlyAlphaNumericCharacters(passwordField.text))
        {
            Debug.Log("Passwort hat Sonderzeichen");
        }
    }

    // Sucht ob es den angefragten Nutzer in der Datenbank giebt.
    public void GetUser()
    {
        StartCoroutine(GetUser(firstNameField.text, surnameField.text, passwordField.text, UserCallback));
    }

    // Wird in GetUser aufgerufen und setzt FirstNameGlobal und SurnameGlobal zu den momentanen vor- und nachnamen des Nutzers.
    public void UserCallback(User user)
    {
        FirstNameGlobal = user.vorname;
        SurnameGlobal = user.nachname;
        sceneSwitcher.goToHomeScreen();
    }

    // Ruft die Funktion GetSongs mit SongCallback als Argument auf.
    public void GetSong()
    {
        StartCoroutine(GetSongs(SongCallback));
    }

    // Wird in GetSong aufgerufen und gibt die Anzahl der gefundenen Songs und deren uris an.
    public void SongCallback(Song[] songs)
    {
        Debug.Log($"Found {songs.Length} songs!");
        foreach (var s in songs)
        {
            Debug.Log(s.song_uri);
        }
    }

    const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string CreateRandomString(int length)
    {
        string s = "";
        for (int i = 0; i < length; i++)
        {
            s += Letters.ToCharArray()[Random.Range(0, Letters.Length)];
        }
        return s;
    }

    // Lädt einen Song auf die Datenbank hoch.
    public IEnumerator PostSong(int uri, int rating)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", FirstNameGlobal);
        form.AddField("name", SurnameGlobal);
        form.AddField("passwort", PasswordGlobal);
        form.AddField("song_uri", uri);
        form.AddField("rating", rating);
        Debug.Log(form);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/songs", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                lastCreatedSong = int.Parse(www.downloadHandler.text);
                Debug.Log($"Song upload complete! ID = {lastCreatedSong}");
            }
        }
    }

    // Holt die Songs für den angegebenen Nutzer von der Datenbank.
    public IEnumerator GetSongs(System.Action<Song[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/songs?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var songs = JsonHelper.getJsonArray<Song>(www.downloadHandler.text);
                songsGlobal = songs;
                Debug.Log(songsGlobal);
                callback(songs);
            }
        }
    }

    // Updated den angegebenen Song.
    public IEnumerator UpdateSong(string vorname, string nachname, string passwort, int songID, int rating)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", vorname);
        form.AddField("name", nachname);
        form.AddField("passwort", passwort);
        form.AddField("songid", songID);
        form.AddField("rating", rating);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/songs/update", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Song update complete!");
            }
        }
    }

    // Löscht den angegebenen Song aus der Datenbank
    public IEnumerator DeleteSong(int songID)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Delete($"http://vitalab.informatik.uni-hamburg.de:3050/songs?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}&songid={songID}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Song Deleted");
            }
        }
    }

    // Erstellt einen User in der Datenbank mit den angegebenen Daten.
    public IEnumerator PostUser(string vorname, string nachname, string passwort, string geschlecht, int bildid)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", vorname);
        form.AddField("name", nachname);
        form.AddField("passwort", passwort);
        form.AddField("geschlecht", geschlecht);
        form.AddField("bildid", bildid);
        FirstNameGlobal = vorname;
        SurnameGlobal = nachname;
        PasswordGlobal = passwort;

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/users", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // Setzt den Errortext inaktiv
    public void SetLoginErrorInactive()
    {
        errorText.SetActive(false);
    }

    // Setzt den Passwort-Error-Text inaktiv
    public void SetPasswordErrorInactive()
    {
        passwordErrorText.SetActive(false);
    }

    // Holt sich die Daten des angegebenen Users von der Datenbank
    public IEnumerator GetUser(string vorname, string nachname, string passwort, System.Action<User> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/users?vorname={vorname}&name={nachname}&passwort={passwort}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Ungültige Einlogdaten");
                errorText.SetActive(true);
            }
            else
            {
                PasswordGlobal = passwort;
                Debug.Log(www.downloadHandler.text);
                var user = JsonUtility.FromJson<User>(www.downloadHandler.text);
                callback(user);
            }
        }
    }


    // Lädt ein Interesse in die Datenbank hoch.
    public IEnumerator PostInterests(string designation, string description)
    {
        Debug.Log(FirstNameGlobal);
        Debug.Log(SurnameGlobal);
        Debug.Log(PasswordGlobal);
        WWWForm form = new WWWForm();
        form.AddField("vorname", FirstNameGlobal);
        form.AddField("name", SurnameGlobal);
        form.AddField("passwort", PasswordGlobal);
        form.AddField("bezeichnung", designation);
        form.AddField("beschreibung", description);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/interests", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // Lädt die Daten aller Interessen eines Users von der Datenbank runter.
    public IEnumerator GetInterests(System.Action<Interest[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/interests?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Ungültige Einlogdaten");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var interest = JsonHelper.getJsonArray<Interest>(www.downloadHandler.text);
                callback(interest);
            }
        }
    }

    // Verändert ein Interesse in der Datenbank.
    public IEnumerator UpdateInterest(int interestID, string designation, string description)
    {
        WWWForm form = new WWWForm();
        form.AddField("name", SurnameGlobal);
        form.AddField("vorname", FirstNameGlobal);
        form.AddField("passwort", PasswordGlobal);
        form.AddField("bezeichnung", designation);
        form.AddField("beschreibung", description);
        form.AddField("interestid", interestID);


        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/interests/update", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Interest update complete!");
            }
        }
    }

    // Löscht ein Interesse von der Datenbank.
    public IEnumerator DeleteInterest(int interestID)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Delete($"http://vitalab.informatik.uni-hamburg.de:3050/interests?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}&interestid={interestID}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Interest Deleted");
                sceneSwitcher.goToInteressen_ListeScreen();
            }
        }
    }

    // Läadt einen Score in die Datenbank hoch.
    public IEnumerator PostScore(string vorname, string nachname, string passwort, string date, string score)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", vorname);
        form.AddField("name", nachname);
        form.AddField("passwort", passwort);
        form.AddField("datum", date);
        form.AddField("score", score);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/scores", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }

    // Lädt die Daten der Scores eines Users von der Datenbank runter.
    public IEnumerator GetScores(string vorname, string nachname, string passwort, System.Action<Score[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/scores?vorname={vorname}&name={nachname}&passwort={passwort}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Ungültige Einlogdaten");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var score = JsonHelper.getJsonArray<Score>(www.downloadHandler.text);
                callback(score);
            }
        }
    }

    // Lädt die Daten aller Bilder eines Nutzers von der Datenbank runter.
    public IEnumerator GetPictures(System.Action<Picture[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                Debug.Log("Ungültige Einlogdaten");
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                var image = JsonHelper.getJsonArray<Picture>(www.downloadHandler.text);
                callback(image);
            }
        }
    }

    // Zeigt die Anzahl der Bilder an und deren ids.
    public void PictureCallback(Picture[] pictures)
    {
        Debug.Log($"Found {pictures.Length} images!");
        foreach (var b in pictures)
        {
            ByteArrayToImage(b.image);
            Debug.Log(b.id);
        }
    }

    // Lädt ein Bild in die Datenbank hoch.
    public IEnumerator PostPicture(byte[] Image)
    {
        WWWForm form = new WWWForm();
        form.AddField("vorname", FirstNameGlobal);
        form.AddField("name", SurnameGlobal);
        form.AddField("passwort", PasswordGlobal);
        form.AddBinaryData("file", Image);

        using (UnityWebRequest www = UnityWebRequest.Post("http://vitalab.informatik.uni-hamburg.de:3050/images/upload", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
                sceneSwitcher.goToFotoListeScreen();
            }
        }
    }

    // Ruft die Funktion PostPicture auf.
    public void PostPictureCall(Texture2D texture)
    {
        StartCoroutine(PostPicture(ImageToByteArray(texture)));
    }

    // Löscht ein Bild von der Datenbank.
    public IEnumerator DeletePicture(int pictureID)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Delete($"http://vitalab.informatik.uni-hamburg.de:3050/images?vorname={FirstNameGlobal}&name={SurnameGlobal}&passwort={PasswordGlobal}&interestid={pictureID}"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Interest Deleted");
                sceneSwitcher.goToFotoListeScreen();
            }
        }
    }

    // Ruft die Funktion DeletePicture auf.
    public void DeletePictureCall(int pictureID)
    {
        StartCoroutine(DeletePicture(pictureID));
    }

    // Wandelt eine Textur in ein ByteArray um.
    public byte[] ImageToByteArray(Texture2D texture)
    {
       /* Debug.Log(texture.isReadable);
        int width = 600;
        int height = 800;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();*/
        byte[] bytes = texture.EncodeToPNG();
        /* Object.Destroy(texture);*/
        Debug.Log(bytes.Length);
        return bytes;
    }

    /*
    public void test()
    {
        testRawImage.texture = ByteArrayToImage(ImageToByteArray(testTexture));
    }
    */

    // Wandelt ein byteArray in ein Image um.
    public Texture2D ByteArrayToImage(byte[] byteArrayIn)
    {
        Debug.Log(byteArrayIn.GetLength(0));
        Debug.Log(byteArrayIn.Length);
        texture.LoadImage(byteArrayIn);
        return texture;
    }

    // Repräsentation eines Songs von der Datenbank.
    [System.Serializable]
    public class Song
    {
        public int userid;
        public string song_uri;
        public int rating;
        public int id;
    }

    // Checkt ob ein string nur Alphanumeric characters hat.
    public bool ContainsOnlyAlphaNumericCharacters(string inputString)
    {
        var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
        return regexItem.IsMatch(inputString);
    }

    // Hilfsfunktion für die Datenbank.
    public class JsonHelper
    {
        public static T[] getJsonArray<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    
    // Repräsentation eines Nutzers von der Datenbank.
    [System.Serializable]
    public class User
    {
        public int id;
        public string vorname;
        public string nachname;
        public string geschlecht;
        public int bildid;
    }

    // Repräsentation eines Interesses von der Datenbank.
    [System.Serializable]
    public class Interest
    {
        public string bezeichnung;
        public string beschreibung;
        public int userid;
        public int id;
    }

    // Repräsentation eines Scores von der Datenbank.
    [System.Serializable]
    public class Score
    {
        public int id;
        public string date;
        public string score;
        public int userid;
    }

    // Repärasentation eines Pictures von der Datenbank.
    [System.Serializable]
    public class Picture
    {
        public int id;
        public byte[] image;
        public Texture2D image2;
        public int userid;
    }


}
