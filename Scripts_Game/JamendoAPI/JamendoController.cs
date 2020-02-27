using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace JamendoAPI
{
    public class JamendoController : MonoBehaviour
    {

        public string cliendID = "bf120eba";
        private AudioSource source;

        public static JamendoController Instance;

        public void Awake()
        {
            if(Instance != null)
            {
                Destroy(this);
                return;
            }
            else
            {
                Instance = this;
            }
        }

        public TrackResult SearchTracksByID(int ID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($@"http://api.jamendo.com/v3.0/tracks/?client_id={cliendID}&format=jsonpretty&limit=1&id={ID}&audioformat=ogg");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();

            var trackResult = TrackResult.FromJson(jsonResponse);

            return trackResult;
        }

        // Start is called before the first frame update
        void Start()
        {
            //var r = SearchGenre("jazz", "hiphop");
            //PlayTrack(r.Tracks[0], null);
        }

        public TrackResult SearchGenre(params string[] tags)
        {
            string tagsString = "";
            foreach (var t in tags)
            {
                tagsString += t + "+";
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($@"https://api.jamendo.com/v3.0/tracks/?client_id={cliendID}&format=jsonpretty&limit=3&fuzzytags={tagsString}&boost=popularity_month&audioformat=ogg");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();

            var trackResult = TrackResult.FromJson(jsonResponse);

            return trackResult;
        }

        public void PlayTrack(Track track, AudioSource audioSource)
        {
            if(audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if(track != null && track.Audio != null)
            {
                StartCoroutine(LoadAudio(track.Audio, audioSource));
            }
        }

        IEnumerator LoadAudio(Uri url, AudioSource audioSource)
        {
            using (var music = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS))
            {
                yield return music.SendWebRequest();
                if (music.isNetworkError)
                {
                    Debug.LogWarning(music.error);
                }
                else
                {
                    if (audioSource.clip != null)
                    {
                        // Remove from memory
                        Destroy(audioSource.clip);
                        yield return null;
                    }
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(music);
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
        }
    }
}