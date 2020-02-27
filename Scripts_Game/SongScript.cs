using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Random = System.Random;
//using UnityEngine.CoreModule;


public class SongScript : MonoBehaviour
{
    public string vornameEintragen;
    public static string vorname;
    public string nachnameEintragen;
    public static string nachname;
    public string passwortEintragen;
    public static string passwort;
    public int songID;
    public AudioSource radio;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetSongs(SongCallback));
        vorname = vornameEintragen;
        nachname = nachnameEintragen;
        passwort = passwortEintragen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GetSongs(System.Action<Song[]> callback)
    {
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get($"http://vitalab.informatik.uni-hamburg.de:3050/songs?vorname={vorname}&name={nachname}&passwort={passwort}"))
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
                callback(songs);
            }
        }
    }

    public void SongCallback(Song[] songs)
    {
        var random = new Random();
        int randomNumber = random.Next(songs.Length);
        random.Next(songs.Length);
        Debug.Log(random.Next(songs.Length));
        Debug.Log($"Found {songs.Length} songs!");
        AbspielenByID(Convert.ToInt32(songs[randomNumber].song_uri));
        songID = songs[randomNumber].id;
    }

    [System.Serializable]
    public class Song
    {
        public int userid;
        public string song_uri;
        public int rating;
        public int id;

    }

    public void AbspielenByID(int ID)
    {
        var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(ID);
        if (result.Tracks.Length > 0)
        {
            Debug.Log("Loading track...");
            JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], radio);
        }
    }

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
}