using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// Dieses Script lädt die Songdaten von der Datenbank runter und instanziiert diese mithilfe der SongContainer-Prefabs.
// Außerdem kann in diesem Script auch ein Song von der Datenbank gelöscht werden.
public class MusicList : MonoBehaviour
{
    public Transform parent;
    public GameObject containerPrefab;
    SceneSwitcher sceneSwitcher;
    public long databaseID;
    public GameObject popUpPanel;
    public GameObject waitHint;
    public GameObject scrollview;
    public GameObject parentGameObject;
    public GameObject arrow;
    public Texture2D oneStar;
    public Texture2D twoStars;
    public Texture2D threeStars;
    public Texture2D noRating;
    RestAPI restAPI;

    // Start is called before the first frame update
    void Start()
    {

        sceneSwitcher = this.GetComponent<SceneSwitcher>();
        restAPI = this.GetComponent<RestAPI>();
        CreateSongList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // setzt die Sterntextur je nach rating auf die richtige Anzahl und ruft die Funktion InstantiateContainer für die mitgegebenen Songs auf.
    public void SongCallback(RestAPI.Song[] songs)
    {
        Texture2D starPicture;
        var Padding = parentGameObject.GetComponent<VerticalLayoutGroup>().padding;
        int yPos = 0;
        Debug.Log($"Found {songs.Length} songs!");
        foreach (var s in songs)
        {
            if (s.rating == 1)
            {
                starPicture = oneStar;
            }
            else if (s.rating == 2)
            {
                starPicture = twoStars;
            }
            else if (s.rating == 3)
            {
                starPicture = threeStars;
            }
            else
            {
                starPicture = noRating;
            }
            Debug.Log(s.song_uri);
            var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(Convert.ToInt32(s.song_uri));
            if (result.Tracks.Length > 0)
            {
                InstantiateContainer(result.Tracks[0].Name, result.Tracks[0].ArtistName, result.Tracks[0].ID, result.Tracks[0].AlbumImage.ToString().Replace("http", "https"), yPos, s.id, starPicture);
            }
            yPos -= 400;
        }
        if (parent.childCount > 4)
        {
            scrollview.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230);
            //Padding.left = 25;
        }
        else
        {
            scrollview.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 230);
            Padding.right = 15;
        }
        if (parent.childCount == 0)
        {
            arrow.SetActive(true);
        }
        waitHint.SetActive(false);
    }

    // Wird bei Start aufgerufen und ist dafür zuständig die Songs von der Datenbank runterzuladen.
    public void CreateSongList()
    {
        waitHint.SetActive(true);
        foreach (Transform child in parent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        StartCoroutine(restAPI.GetSongs(SongCallback));
        //animation


    }

    // Verkürzt den mitgegebenen String umd die mitgegebene Länge
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
    public void InstantiateContainer(string songname, string artist, long id, string picture, int yPos, long databaseID, Texture2D starPicture)
    {
        GameObject prefabInstanz = Instantiate(containerPrefab, new Vector3(0, yPos, 0), Quaternion.identity);
        prefabInstanz.transform.SetParent(parent);
        var songContainer = prefabInstanz.GetComponentInChildren<SongContainerSearch>();
        songContainer.songname.text = StringTrimmer(songname, 19);
        songContainer.artist.text = StringTrimmer(artist, 15);
        songContainer.songID = id;
        songContainer.parent = parent;
        songContainer.databaseID = databaseID;
        songContainer.popUpPanel = popUpPanel;
        songContainer.star.texture = starPicture;
        StartCoroutine(DownloadPicture(picture, songContainer));
        // erst in der Coroutine songContainer.cover = yourRawImage;
        //SongContainerSearch klassenInstanz;
        //klassenInstanz    = prefabInstanz.GetComponent<SongContainerSearch>();


        //klassenInstanz.songname.text = "noodles";
    }

    // Lädt das Bild von der mitgegebenen Url runter und setzt dieses als Cover des Songcontainers.
    public IEnumerator DownloadPicture(string mediaUrl, SongContainerSearch songContainerSearch)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);
        yield return request.SendWebRequest();
        Texture2D tex = ((DownloadHandlerTexture)request.downloadHandler).texture;
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            songContainerSearch.cover.texture = tex;
        }
    }

    // Löscht einen Song
    public void DeleteSong()
    {
        Debug.Log(databaseID);
        StartCoroutine(restAPI.DeleteSong(Convert.ToInt32(databaseID)));
        popUpPanel.SetActive(false);
        sceneSwitcher.goToMusik_ListeScreen();
    }

    // Deaktiviert das popUpPanel
    public void SetPopUpPanelInactive()
    {
        popUpPanel.SetActive(false);
    }
}
