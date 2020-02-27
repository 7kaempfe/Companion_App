using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAudioScript : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        var result = JamendoAPI.JamendoController.Instance.SearchGenre("jazz", "hiphop");

        if(result.Tracks.Length > 0)
        {
            foreach(var track in result.Tracks)
            {
                Debug.Log($"Name: {track.Name} by Artist: {track.ArtistName}");
            }

            Debug.Log("Loading first track...");
            JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], audioSource);
        }
    }

    void Update()
    {
        
    }
}
