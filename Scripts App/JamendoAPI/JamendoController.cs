﻿using System;
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

        // Start is called before the first frame update
        void Start()
        {
            //var r = SearchTracks("jazz", "hiphop");
            //PlayTrack(r.Tracks[0], null);
        }

        public TrackResult SearchTracks(string input, int anzahl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($@"http://api.jamendo.com/v3.0/tracks/?client_id={cliendID}&format=jsonpretty&limit={anzahl}&fuzzytags={input}&boost=popularity_month&audioformat=ogg");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string jsonResponse = reader.ReadToEnd();

            var trackResult = TrackResult.FromJson(jsonResponse);

            return trackResult;
        }

        public TrackResult SearchTracksByID(int id)
        {
            Debug.Log(id);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($@"http://api.jamendo.com/v3.0/tracks/?client_id={cliendID}&format=jsonpretty&limit=1&id={id}&audioformat=ogg");
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
            using (var music = UnityWebRequestMultimedia.GetAudioClip(new Uri(url.AbsoluteUri), AudioType.OGGVORBIS))
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
					PlayAudio(audioSource);

                }
            }
        }	

		public void PlayAudio(AudioSource audioSource)
		{
					float playPosition= audioSource.clip.length / 2;
					audioSource.time = playPosition;
					Debug.Log(audioSource.time);
					audioSource.Play();
		}
    }
}