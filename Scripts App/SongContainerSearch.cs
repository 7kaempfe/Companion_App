using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

// Dieses Script liegt auf dem SongContainerPrefab und ist fürs abspielen der Songs verantwortlich
public class SongContainerSearch : MonoBehaviour
{
	public Text songname, artist;
	public Button playButton;
    public Button addButton;
    public Texture2D pauseTexture;
    public Texture2D playTexture;
    public RawImage cover;
	public long songID;
    public AudioSource radio;
    public GameObject popUpPanel;
    SearchMusic searchMusic;
    public Transform parent;
    public long databaseID;
    public RawImage star;
    MusicList musicList;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(songID);
        musicList = this.GetComponentInParent<MusicList>();
        searchMusic = this.GetComponentInParent<SearchMusic>();
        radio = this.GetComponentInParent<AudioSource>();
    }

    // Spielt den Song ab der auf dem SongContainer liegt
    public void PlayByID()
    {
        if (songID == searchMusic.songID)
        {
            if (radio.isPlaying)
            {
                radio.Pause();
                playButton.GetComponent<RawImage>().texture = playTexture;
            }
            else
            {
                radio.UnPause();
                playButton.GetComponent<RawImage>().texture = pauseTexture;
            }

        }

        else
        {
        radio.Stop();
            if (!radio.isPlaying)
            {
                Debug.Log(songID);
                var result = JamendoAPI.JamendoController.Instance.SearchTracksByID(Convert.ToInt32(songID));
                if (result.Tracks.Length > 0)
                {
                    Debug.Log("Loading track...");
                    JamendoAPI.JamendoController.Instance.PlayTrack(result.Tracks[0], radio);
                }
                searchMusic.songID = songID;
                foreach (Transform Songbox in parent)
                {
                    Songbox.GetChild(2).GetComponent<RawImage>().texture = playTexture;
                }
                playButton.GetComponent<RawImage>().texture = pauseTexture;

            }
        }
    }

    // Aktiviert das PopUp, das fragt ob man den Song auf die Datenbank hochladen möchte
    public void OpenPopUp()
    {
        popUpPanel.SetActive(true);
        Debug.Log(songID);
        searchMusic.songID = songID;
        //radio.Pause();
        //SearchMusic.songHochladen();
    }

    // Aktiviert das PopUp, das fragt ob man den Song löschen möchte
    public void DeleteButton()
    {
        Debug.Log(databaseID);
        musicList.databaseID = databaseID;
        popUpPanel.SetActive(true);
    }

}
