using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleAudioScript : MonoBehaviour
{
    public AudioSource audioSource;
	public GameObject inputfield;

    void Start()
    {
     
    }

    void Update()
    {
        
    }
	
	public void Abspielen()
	{
		var result = JamendoAPI.JamendoController.Instance.SearchTracks("Afro Trap", 5);

        if(result.Tracks.Length > 0)
        {
            foreach(var track in result.Tracks)
            {
                Debug.Log($"Name: {track.Name} by artist: {track.ArtistName}");
            }

            Debug.Log("Loading first track...");
            JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], audioSource);
        }
	}
	
    public void PlayByID()
    {
        var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(1180121);

        if (result.Tracks.Length > 0)
        {
            foreach (var track in result.Tracks)
            {
                Debug.Log($"Name: {track.Name} by artist: {track.ArtistName}");
            }

            Debug.Log("Loading first track...");
            JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], audioSource);
        }
    }
	
	
}
