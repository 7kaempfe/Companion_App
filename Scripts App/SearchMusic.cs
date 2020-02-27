using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Text;

// Dieses Script wird benutzt um nach Musik zu suchen
public class SearchMusic : MonoBehaviour
{
	
	public InputField searchField;
	public AudioSource radio;
	public GameObject containerPrefab;
    public Transform parent;
    public RawImage yourRawImage;
    public GameObject scrollbar;
    public GameObject searchButton;
    public GameObject popUpPanel;
    public Text hint;
    public long songID;
    RestAPI restAPI;

    // Start is called before the first frame update
    void Start()
    {
        restAPI = this.GetComponent<RestAPI>();

        if (scrollbar != null)
        {
            scrollbar.SetActive(false);
        }

        if (searchButton != null)
        {
            searchButton.SetActive(false);
        }

        if (popUpPanel != null)
        {
            popUpPanel.SetActive(false);
        }
   //     StartCoroutine(GetRequest("https://www.example.com"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    // setzt den searchButton aktiv
    public void SetButtonActive()
    {
        searchButton.SetActive(true);
    }
	
    // sucht nach Musik für den angegebenen Begriff im searchField und ruft für jeden gefundenen Song die Funktion InstantiateContainer auf.
	public void SearchForMusic()
	{
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        string input = searchField.text;
		var result = JamendoAPI.JamendoController.Instance.SearchTracks(input, 20);
		int yPos = 0;

        if(result.Tracks.Length > 0)
        {
            scrollbar.SetActive(true);
            parent.GetComponent<AudioSource>().Stop();
            foreach (var track in result.Tracks)
            {
                Debug.Log($"Name: {track.Name} by artist: {track.ArtistName}");
                //erstelleSongContainer(800, 800, track);
				InstantiateContainer(track.Name, track.ArtistName, track.ID, track.AlbumImage.ToString().Replace("http","https"), yPos);
				yPos -= 400;
            }

            Debug.Log("Loading first track...");
            /*JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], radio);*/
        }

        else
        {
            hint.text = "Keine Treffer gefunden.";
            parent.GetComponent<AudioSource>().Stop();
            scrollbar.SetActive(false);
        }

    }

    // sucht nach dem Song mit der mitgegebenen ID und spielt diesen anschließend ab.
    public void PlayByID(int ID)
    {
        var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(ID);
        if (result.Tracks.Length > 0)
        {
            Debug.Log("Loading track...");
            JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], radio);
        }
    }

    // Schließt das popUpPanel
    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
        //radio.UnPause();
    }

    // Lädt einen Song in die Datenbank hoch.
    public void LoadIntoDatabase()
    {
        Debug.Log(restAPI);
        var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(Convert.ToInt32(songID));

        popUpPanel.SetActive(false);
        Debug.Log(songID);
        //radio.UnPause();

        StartCoroutine(restAPI.PostSong(Convert.ToInt32(songID), 5));
        
    }
    /**public void erstelleSongContainer(int xPos, int yPos, JamendoAPI.Track track)
	{
		GameObject container = Instantiate(containerPrefab, new Vector3 (xPos, yPos, 0), Quaternion.identity);
        container.transform.SetParent(parent);
        Button button1 = container.GetComponent<Button>();
		Debug.Log(button1);
        //container.GetComponent<Coolertext>().text="It works";
        //prefab.Picture = track.Cover;
		//prefab.Name = track.name;
		//prefab.Duration = track.duration;
	}**/

    // kürzt den mitgegebenen String um die angegebene Länge
    public string StringTrimmer(string s, int length)
    {
        if (s.Length > length)
        {
            return s.Remove(length, s.Length - length) + "...";
        }
        else
        {
            return s;
        }
    }

    // Instanziiert einen SongContainer mit den mitgegebenen Daten
    public void InstantiateContainer(string songname, string artist, long id, string picture, int yPos)
    {
        GameObject prefabInstanz = Instantiate(containerPrefab, new Vector3 (0, yPos, 0), Quaternion.identity);
        prefabInstanz.transform.SetParent(parent);
        var test = prefabInstanz.GetComponentInChildren<SongContainerSearch>();
        test.songname.text = StringTrimmer(songname, 22);
        test.artist.text = StringTrimmer(artist, 20);
		test.songID = id;
        test.popUpPanel = popUpPanel;
        test.parent = parent;
        StartCoroutine(DownloadPicture(picture, test));
        // erst in der Coroutine test.cover = yourRawImage;
        //SongContainerSearch klassenInstanz;
        //klassenInstanz    = prefabInstanz.GetComponent<SongContainerSearch>();


        //klassenInstanz.songname.text = "noodles";
    }

    // lädt das Bild von der mitgegebenen Url runter und setzt es auf die Textur des Songcontainers.
    public IEnumerator DownloadPicture(string MediaUrl, SongContainerSearch songContainerSearch)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            songContainerSearch.cover.texture = tex;
        }
    }
    
    /*public void songHochladen(string vorname, string nachname, string passwort, string geschlecht)
    {
        var request = (HttpWebRequest)WebRequest.Create(uri);

        string stringData = "name=Pan&vorname=Peter&passwort=peterpan"; // place body here
        var data = Encoding.Default.GetBytes(stringData); // note: choose appropriate encoding

        request.Method = "PUT";
        request.ContentType = ""; // place MIME type here
        request.ContentLength = data.Length;

        var newStream = request.GetRequestStream(); // get a ref to the request body so it can be modified
        newStream.Write(data, 0, data.Length);
        Debug.Log(newStream);
        newStream.Close();
        UnityWebRequest.EscapeURL(uri);
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name=Pan&vorname=Peter&passwort=peterpan"));
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri)) 
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                Debug.Log(webRequest.downloadHandler.ToString());
            }
        }
    }*/

  /*  IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }
    */
}
